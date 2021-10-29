using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipAI : MonoBehaviour {
    public enum MoveMode { Stationary, Waypoint, FlyatTarget, Orbit, EnterHyperspace, ExitHyperspace, Destroyed };

    // primary properties
    public int shipLevel;
    public DataManager.Race race;
    public int shipTypeID;
    
    // ship stats and current status
    public DataManager.ShipData shipData;
    public bool isBoss;
    public float turnSpeed;
    public float baseMoveSpeed;
    public float actualMoveSpeed;

    public int hyperspaceTargetSystem;
    public int hyperspaceTargetPlanet;

    // max quantities
    public int maxHull;
    public int maxShield;
    public int maxPower;

    // quantities in use
    public int usedPower;
    public int shieldPower;
    public int weaponPower;
    public int usedWeaponPower;
    public int speedPower;

    // actual shield/hull quantities
    public int hull;
    public int shield;

    public List<EquippedWeapons> equippedWeapons = new List<EquippedWeapons>();

    // in combat is true if there are still enemies in the current area
    public bool inCombat;
    // infected is whether the replicators have infected this ship currently
    public bool infected;
    // infected cooldown is used to track the cooldown till the release of the effect
    public float infectedCooldown;
    // has the ship been hit by a player attack
    public bool taggedByPlayer;
    // heal on timer
    public float healCooldown = 1.0f;

    // targetting variables
    public bool attackTarget;
    public GameObject target;
    public Vector3 pointTarget;
    public MoveMode moveMode;
    public float rotateProgress;
    public bool hyperspaceWindowSpawned;
    private float checkProximityCooldown;

    public AIController controller;
    public GameController game;

    private float targetUpdateCycle;
    private float targetUpdateTime = 10;

    // TODO: remove this in real game
    bool godMode = false;

	// Use this for initialization
	void Start () {
        //equippedWeapons = new List<EquippedWeapons>();
        targetUpdateCycle = targetUpdateTime;
        //gameObject.transform.Rotate(new Vector3(90,0,0));
	}
	
	// Update is called once per frame
	void Update () {
        if (race == DataManager.Race.Human && Input.GetKeyDown(KeyCode.F12))
        {
            godMode = !godMode;
        }
        
        if (moveMode == MoveMode.EnterHyperspace && !hyperspaceWindowSpawned)
        {
            // spawn a hyperspace window
            Vector3 point = game.spawnHyperspaceWindow(transform.position, false);
            initiateMove(ShipAI.MoveMode.EnterHyperspace, point);
            hyperspaceWindowSpawned = true;
        }

        if (controller == null)
        {
            return;
        }

        if (canRetreat())
        {
            game.setAIHyperspaceTarget(gameObject);
        }

        // update infection
        if (infected)
        {
            if (race != DataManager.Race.Human || (race == DataManager.Race.Human && infectedCooldown != -1))
            {
                infectedCooldown -= Time.deltaTime;

                if (infectedCooldown <= 0)
                {
                    infected = false;
                }
            }
        }

        updateAttackTarget();

        updateAIMovement();

        updateMovement();

        updateWeapons();

        // heal out of combat
        if (!inCombat)
        {
            healCooldown -= Time.deltaTime;

            if (healCooldown <= 0)
            {
                healShip();
                healCooldown = 1.0f;
            }
        }
        else
        {
            healCooldown = 1.0f;
        }

	}

    public void healShip()
    {
        if (hull < maxHull)
        {
            hull += (int)(0.05 * maxHull);
        }
        else if(shield < maxShield)
        {
            shield += (int)(0.05 * maxShield);
        }
            
        if (hull > maxHull) hull = maxHull;
        if (shield > maxShield) shield = maxShield;
    }

    public void updateAttackTarget()
    {
        if (controller.getEnemyCountofRace(race) == 0)
        {
            inCombat = false;
        }
        else
        {
            inCombat = true;
        }
        
        if (targetUpdateCycle > 0)
        {
            targetUpdateCycle -= Time.deltaTime;
            return;
        }

        ShipAI targetData = null;
        if (target == null)
        {
            attackTarget = false;
        }
        else if (target.GetComponent("ShipAI") == null)
        {
            attackTarget = false;
        }
        else
        {
            targetData = (ShipAI)target.GetComponent("ShipAI");
            if (DataManager.isAlly(race, targetData.race))
            {
                attackTarget = false;
            }
            else
            {
                attackTarget = true;
            }
        }

        if (infected)
        {
            target = controller.selectRandomTarget(gameObject);
            if (target != null) attackTarget = true;
        }
        else if (race != DataManager.Race.Human)
        {
            aiTargetUpdate();
        }
    }

    private void aiTargetUpdate()
    {
        if (attackTarget == true)
        {
            // check if there is an ally in need
            List<GameObject> allies = controller.getAllies(race);
            foreach (GameObject ally in allies)
            {
                ShipAI shipData = (ShipAI)ally.GetComponent("ShipAI");
                if (shipData.getHullPercent() < 40)
                {
                    List<GameObject> attackers = controller.getAllTargettingShip(ally);
                    if (attackers.Count == 0) continue;

                    target = controller.selectObjectFromList(attackers);
                    if (target != null) attackTarget = true;
                    return;
                }
            }

            if (Random.Range(0.0f, 1.0f) < 0.999f) return;
        }

        target = controller.getNewTarget(gameObject);
        if (target != null) attackTarget = true;
    }

    public void updateMovement()
    {
        if (moveMode == MoveMode.Destroyed || moveMode == MoveMode.Stationary) return;

       

        // set the updated target location
        if (moveMode == MoveMode.Orbit || moveMode == MoveMode.FlyatTarget)
        {
            if (target == null) return;
            pointTarget = target.transform.position;
        }

        // update the movement speed

        if (moveMode == MoveMode.EnterHyperspace || moveMode == MoveMode.ExitHyperspace)
            actualMoveSpeed = 50;
        else
            actualMoveSpeed = baseMoveSpeed * calculateSpeedMultiplier();

        if (actualMoveSpeed < 0) actualMoveSpeed = 0;

        // update movement
        Vector3 velocity = pointTarget - transform.position;
        if (moveMode == MoveMode.Waypoint && Vector3.Distance(transform.position, pointTarget) < 30)
        {
            if (race == DataManager.Race.Human)
                setMoveMode(MoveMode.Stationary);
            rigidbody.velocity = Vector3.zero;
            return;
        }
        else if (moveMode == MoveMode.FlyatTarget && Vector3.Distance(transform.position, pointTarget) < 100)
        {
            if (race == DataManager.Race.Human)
                setMoveMode(MoveMode.Stationary);
            rigidbody.velocity = Vector3.zero;
            return;
        }
        else if (moveMode == MoveMode.EnterHyperspace && Vector3.Distance(transform.position, pointTarget) < 10)
        {
            setMoveMode(MoveMode.Stationary);
            rigidbody.velocity = Vector3.zero;
            game.performHyperspaceJump(gameObject, hyperspaceTargetSystem, hyperspaceTargetPlanet);
            return;
        }
        else if (moveMode == MoveMode.ExitHyperspace && Vector3.Distance(transform.position, pointTarget) < 10)
        {
            setMoveMode(MoveMode.Stationary);
            rigidbody.velocity = Vector3.zero;
            return;
        }

        // if movement to the point is required or movement to within a range.
        if (moveMode != MoveMode.Orbit || Vector3.Distance(target.transform.position, transform.position) > 75)
        {
            // rotate to face target
            Vector3 direction = pointTarget - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed*Time.deltaTime);

            velocity.Normalize();
            velocity *= actualMoveSpeed;
            rigidbody.velocity = velocity;
        }
        else
        {
            transform.RotateAround(target.transform.position, Vector3.right, turnSpeed * Time.deltaTime);
        }
    }

    private void updateAIMovement()
    {
        if (race == DataManager.Race.Human) return;

        if (moveMode == MoveMode.Stationary)
        {
            checkProximityCooldown -= Time.deltaTime;
            if (checkProximityCooldown <= 0)
            {
                if (controller.findShipsWithInRangeOf(transform.position, 50).Count > 1)
                {
                    Vector3 newTarget = controller.getVector3InRangeOfPoint(transform.position, 50,80);
                    setMoveMode(MoveMode.Waypoint);
                    pointTarget = newTarget;
                    return;
                }
            }
        }

        if (moveMode == MoveMode.Stationary && target == null) return;

        if(moveMode == MoveMode.EnterHyperspace || moveMode == MoveMode.ExitHyperspace || moveMode == MoveMode.Destroyed) return;
        
        float distanceTarget = -1;
        if(target != null)
        {
             distanceTarget = Vector3.Distance(transform.position, target.transform.position);
        }

        if(target == null || (moveMode == MoveMode.FlyatTarget && distanceTarget < 75))
        {
            setMoveMode(MoveMode.Stationary);
            rigidbody.velocity = Vector3.zero;
            return;
        }

        if(moveMode == MoveMode.Waypoint && Vector3.Distance(transform.position, pointTarget) <= 1)
        {
            setMoveMode(MoveMode.Stationary);
            rigidbody.velocity = Vector3.zero;
            return;
        }

        if(moveMode == MoveMode.Stationary && target != null)
        {
            setMoveMode(MoveMode.FlyatTarget);
        }
    }

    public void initiateMove(MoveMode mode, Vector3 target)
    {
        if (infected)
        {
            displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
            msgHandle.texter("You have no control, you are infected!", 1);
        }

        setMoveMode(mode);
        if (mode == MoveMode.EnterHyperspace)
            hyperspaceWindowSpawned = false;
        pointTarget = target;
    }

    public void initiateMove(MoveMode mode, GameObject target)
    {
        if (infected)
        {
            displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
            msgHandle.texter("You have no control, you are infected!", 1);
        }

        if (target.tag == "Supergate" && mode == MoveMode.EnterHyperspace)
        {
            initiateMove(MoveMode.EnterHyperspace, target.transform.position);
            hyperspaceWindowSpawned = true;
            return;
        }

        setMoveMode(mode);
        if (mode == MoveMode.EnterHyperspace)
            hyperspaceWindowSpawned = false;
        this.target = target;
    }

    public void enterSupergate()
    {
        GameObject supergate = GameObject.FindGameObjectWithTag("Supergate");

        if (supergate != null)
        {
            hyperspaceTargetSystem = (controller.systemID == 15) ? 16 : 15;
            hyperspaceTargetPlanet = (hyperspaceTargetSystem == 15) ? 4 : 0; 
            initiateMove(MoveMode.EnterHyperspace, supergate);
        }
    }

    private float calculateSpeedMultiplier()
    {
        float enginePercent = speedPower * 10.0f / maxPower;
        float multiplier = 0.5f * Mathf.Log(0.25f * Mathf.Pow(enginePercent + 0.1f, 5f)) + 0.7f;
        multiplier *= (1f + 0.1f * shipLevel);
        return multiplier;
    }

    public void updateWeapons()
    {
        if(moveMode == MoveMode.Destroyed || moveMode == MoveMode.EnterHyperspace || moveMode == MoveMode.ExitHyperspace)
            return;

        foreach (EquippedWeapons equippedWeapon in equippedWeapons)
        {
            for (int i = 0; i < equippedWeapon.weaponCount; i++)
            {
                if (equippedWeapon.coolDown[i] > 0)
                {
                    equippedWeapon.coolDown[i] -= Time.deltaTime;
                }
                else if(equippedWeapon.enabled[i] && attackTarget)
                {
                    if (Vector3.Distance(target.transform.position, transform.position) > 250) continue;
                    fireWeapon(equippedWeapon);
                    equippedWeapon.coolDown[i] = equippedWeapon.weaponData.fireRate * Random.Range(3.0f, 5.0f);
                }
            }
        }
    }

    private void fireWeapon(EquippedWeapons weapon)
    {
        GameObject projectile = (GameObject)Instantiate(game.getPrefabWeapon(weapon.weaponData.prefabID), transform.position, Quaternion.identity);
        Physics.IgnoreCollision(projectile.collider, collider);
        ProjectileData data = projectile.GetComponent<ProjectileData>();
        data.configureProjectile(gameObject, weapon.weaponData, target, race == DataManager.Race.Human);
        //ShipAI targetData = (ShipAI)target.GetComponent("ShipAI");
        //targetData.damageShip(weapon.weaponData, race == DataManager.Race.Human);
    }

    public void damageShip(DataManager.WeaponData weapon, bool isPlayerShot)
    {
        if (godMode) return;

        if (moveMode == MoveMode.Destroyed || moveMode == MoveMode.EnterHyperspace || moveMode == MoveMode.ExitHyperspace)
            return;

        float multiplier = 1.0f;

        switch (weapon.damageType)
        {
            case DataManager.DamageType.Replicator:
                if (shield == 0 && Random.Range(0.0f, 1.0f) <= 0.5f)
                {
                    infectShip();
                }
                return;
            case DataManager.DamageType.Energy:
                if (shield > 0) multiplier = 1.25f;
                break;
            case DataManager.DamageType.Projectile:
                if (shield == 0) multiplier = 1.25f;
                break;
        }

        if (shield > 0)
        {
            float shieldMultiplier = (-0.5f * Mathf.Log(0.25f * (shieldPower * 10.0f / maxPower + 0.2f))) * (1 + 0.1f * shipLevel);
            if(shieldMultiplier < 0) shieldMultiplier = 0;

            multiplier += shieldMultiplier;

            damageShield((int)(weapon.damageValue * multiplier));
        }
        else
        {
            damageHull((int)(weapon.damageValue * multiplier));
        }

        if (isPlayerShot) taggedByPlayer = true;
    }

    private void infectShip()
    {
        
        infected = true;

        if (race == DataManager.Race.Human)
        {
            infectedCooldown = -1;
        }
        else
        {
            // 10 to 40 seconds for AI infection duration
            infectedCooldown = Random.Range(10000.0f, 40000.0f);
        }
    }

    public void beginPlayerCancelInfection()
    {
        if(!infected) return;
        infectedCooldown = Random.Range(10000.0f, 40000.0f);
    }

    private void damageShield(int damage)
    {
        shield -= damage;
        if (shield < 0)
        {
            if(race == DataManager.Race.Human) {
                displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
                msgHandle.texter("Warning! Shields have failed!", 1);
            }

            damageHull(shield * -1);
            shield = 0;
        }
    }

    private void damageHull(int damage)
    {
        hull -= damage;

        if (hull <= 0)
        {
            if (race == DataManager.Race.Human)
            {
                displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
                msgHandle.texter("Hull has failed! Retreating to Earth!", 1);
                hull = 1;
                setMoveMode(MoveMode.EnterHyperspace);
                hyperspaceTargetPlanet = 0;
                hyperspaceTargetSystem = 0;
            }
            else
            {
                setMoveMode(MoveMode.Destroyed);
                //Create Explosion, Spawn Wreck, Populate wreck
                GameObject wreck = (GameObject)Instantiate(game.getPrefabWreck(), transform.position, Random.rotation);
                controller.addWreck(wreck);
                LootableShip lootData = wreck.GetComponent<LootableShip>();

                int[] loot = new int[shipData.resources.Length];
                for(int i = 0; i < shipData.resources.Length; i++)
                {
                    loot[i] = shipData.resources[i]*(shipLevel+1)*shipData.resourceMultiplier;
                }

                lootData.setResources(loot);

                int explosionCount = Random.Range(2, 4);
                for (int i = 0; i < explosionCount; i++)
                {
                    Instantiate(game.getPrefabExplosion(), controller.getVector3InRangeOfPoint(transform.position, 0, 20), Quaternion.identity);
                }

                // give exp
                if (taggedByPlayer)
                {
                    game.increasePlayerExp(50 + 50 * shipLevel);
                }

                // remove this ship, remove from lists in AI Controller
                controller.removeShip(gameObject);
                Destroy(gameObject);
            }
        }
    }

    public void setMoveMode(MoveMode moveMode)
    {
        this.moveMode = moveMode;
    }

    public int getShieldPercent()
    {
        return (int)Mathf.Round(shield * 100.0f / maxShield);
    }

    public int getHullPercent()
    {
        return (int)Mathf.Round(hull * 100.0f / maxHull);
    }

    public int getShieldPowerPercent()
    {
        return (int)Mathf.Round(shieldPower * 100.0f / maxPower);
    }

    public int getWeaponPowerPercent()
    {
        return (int)Mathf.Round(weaponPower * 100.0f / maxPower);
    }

    public int getEnginePowerPercent()
    {
        return (int)Mathf.Round(speedPower * 100.0f / maxPower);
    }

    public int getPowerUsePercent()
    {
        return (int)Mathf.Round(usedPower * 100.0f / maxPower);
    }

    public bool powerIncreaseAllowed(int amount)
    {
        return (usedPower + amount) <= maxPower;
    }
    /* CHeck weapon power was added and update power was chnaged*/
    /***************************************************************************************************************/
    public float checkWeaponPower(float wPower)
    {
        displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
        int tmpPower = (int)(wPower / 100.0 * maxPower);

        if (tmpPower < usedWeaponPower)
        {
            msgHandle.texter("Too many Active Weapons. Shutdown Some To Reduce Power!", 1);
            return (wPower + 1);
        }
        else
            return wPower;
    }

    public void updatePowerStatus(int shieldPower, int weaponPower, int speedPower)
    {
        // translate values
        shieldPower = (int)(shieldPower / 100.0 * maxPower);
        weaponPower = (int)(weaponPower / 100.0 * maxPower);
        speedPower = (int)(speedPower / 100.0 * maxPower);

        if (shieldPower + weaponPower + speedPower > maxPower)
        {
            displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
            msgHandle.texter("Something very very bad has happened :S!", 1);
            return;
        }

        this.speedPower = speedPower;
        this.shieldPower = shieldPower;
        usedPower = shieldPower + weaponPower + speedPower;
        this.weaponPower = weaponPower;
    }
/*****************************************************************************************************************************************/
    public void configureShip(int level, DataManager.Race race, DataManager.ShipData shipData, AIController controller, GameController game)
    {
        if(level > 9 || level < 0) Debug.LogError("Level was invalid!!");

        this.controller = controller;
        this.game = game;
        this.shipData = shipData;
        shipLevel = level;
        this.race = race;
        shipTypeID = shipData.shipID;

        isBoss = shipData.isBoss;
        turnSpeed = shipData.turnSpeed;
        
        DataManager.ShipStats stats =  shipData.AIStats[level];
        baseMoveSpeed = stats.speed;
        actualMoveSpeed = stats.speed;

        hull = maxHull = stats.hull / 3;
        shield = maxShield = stats.shield / 3;
        maxPower = stats.power;

        usedPower = 0;
        shieldPower = 0;
        weaponPower = 0;
        usedWeaponPower = 0;
        speedPower = 0;

        inCombat = false;
        infected = false;
        taggedByPlayer = false;

        attackTarget = false;
        target = null;
        pointTarget = Vector3.zero;
        moveMode = MoveMode.Stationary;

        equippedWeapons = new List<EquippedWeapons>();
        if (race != DataManager.Race.Human)
        {
            shieldPower = (int)(maxPower * Random.Range(0.4f, 0.6f));
            speedPower = (int)(maxPower * 0.2f);
            usedPower += shieldPower + speedPower;
            weaponPower = maxPower - usedPower;
            usedPower = maxPower;

            for (int i = 0; i < stats.maxWeapons.Length; i++)
            {
                equipWeapon(i, stats.maxWeapons[i]);
                for(int j = 0; j < stats.maxWeapons[i]; j++)
                    enableWeapon(i);
            }
        }
        else
        {
            DataManager.PlayerShips shipconfig = game.player.ships[game.player.currentShip];
            weaponPower = shipconfig.weaponPower;
            usedPower += weaponPower;

            for (int i = 0; i < shipconfig.shipWeapons.Length; i++)
            {
                equipWeapon(i, shipconfig.shipWeapons[i]);
            }

            shieldPower = shipconfig.shieldPower;
            usedPower += shieldPower;

            speedPower = shipconfig.speedPower;
            usedPower += speedPower;
        }
    }

    /// <summary>
    /// Add the specified weapon and disable it initially
    /// </summary>
    public void equipWeapon(int weaponID, int count)
    {
        if (count <= 0) return;

        // check if weapon is already existing once in the set
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            if (equippedWeapons[i].weaponData.weaponID == weaponID)
            {
                DataManager.WeaponData weaponData = equippedWeapons[i].weaponData;
                float[] tempCoolDown = equippedWeapons[i].coolDown;
                bool[] tempEnabled = equippedWeapons[i].enabled;
                int weaponCount = equippedWeapons[i].weaponCount;

                equippedWeapons.Remove(equippedWeapons[i]);
                EquippedWeapons updatedWeapon = new EquippedWeapons();
                updatedWeapon.weaponCount = weaponCount + count;
                updatedWeapon.weaponData = weaponData;
                updatedWeapon.coolDown = new float[weaponCount + count];
                updatedWeapon.enabled = new bool[weaponCount + count];

                for (int j = 0; j < weaponCount; j++)
                {
                    updatedWeapon.coolDown[i] = tempCoolDown[i];
                    updatedWeapon.enabled[i] = tempEnabled[i];
                }

                for (int j = weaponCount; j < weaponCount + count; j++)
                {
                    updatedWeapon.coolDown[j] = 0;
                    updatedWeapon.enabled[j] = false;
                }

                equippedWeapons.Add(updatedWeapon);
                return;
            }
        }

        // weapon didn't already exist, so create a new one
        EquippedWeapons newWeapons = new EquippedWeapons();
        newWeapons.weaponCount = count;
        newWeapons.weaponData = game.getWeaponData(weaponID); ;
        newWeapons.coolDown = new float[count];
        newWeapons.enabled = new bool[count];
        for (int i = 0; i < count; i++)
        {
            newWeapons.coolDown[i] = 0;
            newWeapons.enabled[i] = false;
        }

        equippedWeapons.Add(newWeapons);
    }

    /// <summary>
    /// Find the first disabled instance of an equipped weapon and enable it
    /// </summary>
    public void enableWeapon(int weaponID)
    {
        // find weapon/s
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            if (equippedWeapons[i].weaponData.weaponID == weaponID)
            {
                if (usedWeaponPower + equippedWeapons[i].weaponData.powerValue > weaponPower)
                {
                    displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
                    msgHandle.texter("Warning! Insufficient power!", 1);
                    return;
                }

                for (int j = 0; j < equippedWeapons[i].weaponCount; j++)
                {
                    if (!equippedWeapons[i].enabled[j])
                    {
                        usedWeaponPower += equippedWeapons[i].weaponData.powerValue;
                        equippedWeapons[i].coolDown[j] = 0;
                        equippedWeapons[i].enabled[j] = true;

                        return;
                    }
                }
                break;
            }
        }
    }

    /// <summary>
    /// Find the first enabled instance of an equipped weapon and disable it
    /// </summary>
    public void disableWeapon(int weaponID)
    {
        // find weapon/s
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            if (equippedWeapons[i].weaponData.weaponID == weaponID)
            {
                for (int j = 0; j < equippedWeapons[i].weaponCount; j++)
                {
                    if (equippedWeapons[i].enabled[j])
                    {
                        usedWeaponPower -= equippedWeapons[i].weaponData.powerValue;
                        equippedWeapons[i].coolDown[j] = 0;
                        equippedWeapons[i].enabled[j] = false;
                        return;
                    }
                }
                break;
            }
        }
    }

    public int getNumberOfFiring(int weaponID)
    {
        int count = 0;
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            if (equippedWeapons[i].weaponData.weaponID == weaponID)
            {
                for (int j = 0; j < equippedWeapons[i].weaponCount; j++)
                {
                    if (equippedWeapons[i].enabled[j])
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private bool canRetreat()
    {
        if (!isBoss) return false;
        int hullPercent = getHullPercent();
        if (hullPercent > 60) return false;

        int numberOfPlanets = 0;
        for (int i = 0; i < controller.game.systems[controller.systemID].planets.Length; i++)
        {
            if (controller.game.systems[controller.systemID].planets[i].controller == race)
            {
                numberOfPlanets++;
            }
        }

        if (numberOfPlanets <= 1) return false;

        if (hullPercent > 40)
        {
            if (Random.Range(1, 3) == 1) return true;
            return false;
        }
        else if (hullPercent > 20)
        {
            if (Random.Range(1, 5) <= 3) return true;
            return false;
        }
        else
        {
            if (Random.Range(1, 6) <= 5) return true;
            return false;
        }
    }

    public int[] getCountsofWeaponsEnabled()
    {
        int[] result = new int[game.shipDatabase.weapons.Length];
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            result[equippedWeapons[i].weaponData.weaponID] = equippedWeapons[i].weaponCount;
        }
        return result;
    }

    public struct EquippedWeapons 
    {
        public DataManager.WeaponData weaponData;
        public int weaponCount;
        public float[] coolDown;
        public bool[] enabled;
    }
}
