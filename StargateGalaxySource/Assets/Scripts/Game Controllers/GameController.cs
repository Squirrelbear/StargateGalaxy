using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

public class GameController : MonoBehaviour
{
    public enum GameState { Loading, Playing };

    public DataManager.SystemData[] systems;
    public DataManager.PlayerData player;
    public DataManager.ShipDatabase shipDatabase;


    public List<GameObject> prefabShips;
    public List<GameObject> prefabPlanets;
    public List<GameObject> prefabWeapons;
    public GameObject prefabHyperspace;
    public GameObject prefabExplosion;
    public GameObject prefabWreckedShip;
    public GameObject controllerPrefab;
    public List<Material> skyboxMaterials;
    public DataLoader loader;

    private List<AIController> controllers;
    private GameObject tmpPlayer;

    private int currentSystem;
    private int currentPlanet; // determined by proximity
    private GameState gameState;
    private float loadCompleteCooldown;

    public float countDownToBattle;
    public bool battleSimEnabled;
    public float timeBetweenBattles;

    public float callAlliesCooldown;
    public float switchPlayerAreaCooldown;

    public AudioClip[] music;

    public bool isDebugMode = false;

    // keep this object alive between scenes
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Use this for initialization
    void Start()
    {
        if (GameObject.FindGameObjectsWithTag("GameController").Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        loader = (DataLoader)gameObject.AddComponent("DataLoader");

        DataManager.SaveData saveData = loader.loadPlayerData(false);
        systems = (DataManager.SystemData[])saveData.systems.Clone();

        player = saveData.player;

        shipDatabase = loader.loadShipData();

        /*if (isDebugMode)
        {
            printOutShipDatabase();
            printOutPlayerSaveData();
        }*/

        if (player.currentShip == -1)
        {
            buyShip(0);
            player.currentShip = 0;
        }

        timeBetweenBattles = 60;
        callAlliesCooldown = 60;
        switchPlayerAreaCooldown = 10;
        countDownToBattle = timeBetweenBattles;
        battleSimEnabled = false;

        currentSystem = 0;
        currentPlanet = 0;

        controllers = new List<AIController>();

        hyperspaceJumpTo(0, 0);
    }

    /// <summary>
    /// This is the method that needs to be called by the map to trigger the player
    /// to enter hyperspace to the location.
    /// </summary>
    /// <param name="systemID"></param>
    /// <param name="planetID"></param>
    public void setHyperspaceTarget(int systemID, int planetID)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        ShipAI playerShipData = player.GetComponent<ShipAI>();
        playerShipData.hyperspaceTargetPlanet = planetID;
        playerShipData.hyperspaceTargetSystem = systemID;
        playerShipData.setMoveMode(ShipAI.MoveMode.EnterHyperspace);
    }

    /// <summary>
    /// Jumps into hyperspace by switching to the loading screen.
    /// </summary>
    /// <param name="systemID">The target system.</param>
    /// <param name="planetID">The target planet.</param>
    public void hyperspaceJumpTo(int systemID, int planetID)
    {
        gameState = GameState.Loading;
        loadCompleteCooldown = Random.Range(5, 10);

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            tmpPlayer = GameObject.FindGameObjectWithTag("Player");
            updateStatusPlayerVision(currentSystem, currentPlanet);
            storePlayerShipState();

            if (systemID != currentSystem)
            {
                controllers.Clear();
            }
        }

        currentSystem = systemID;
        currentPlanet = planetID;

        //if(GetComponent("MainHUD") != null)
        //  Destroy(gameObject.GetComponent("MainHUD"));

        Application.LoadLevel("LoadingScreen");
    }

    /// <summary>
    /// Completes the loading of the next system and resuming of gameplay.
    /// </summary>
    private void completeHyperspaceJump()
    {
        controllers = new List<AIController>();

        // update the skybox
        setSkyboxMat(systems[currentSystem].skyboxID);

        // configure where a boss will default spawn if needed
        int bossSpawn = -1;
        if (systems[currentSystem].hasBoss)
        {
            int controlCount = 0;
            DataManager.Race race = systems[currentSystem].owner;
            // Asgard and human never have bosses
            for (int i = 0; i < systems[currentSystem].planets.Length; i++)
            {
                if (race == systems[currentSystem].planets[i].controller)
                {
                    controlCount++;
                }
            }

            if (controlCount > 0)
            {
                for (int i = 0; ; i++)
                {
                    if (race == systems[currentSystem].planets[i].controller)
                    {
                        controlCount--;
                    }

                    if (controlCount == 0)
                    {
                        bossSpawn = i;
                        break;
                    }
                }
            }
            else
            {
                // system has been wiped meaning boss should actually no longer exist so purge it
                systems[currentSystem].hasBoss = false;
            }
        }

        for (int i = 0; i < systems[currentSystem].planets.Length; i++)
        {
            GameObject obj = (GameObject)Instantiate(controllerPrefab);
            AIController controller = (AIController)obj.GetComponent("AIController");
            int planetPrefabID = systems[currentSystem].planets[i].textureID;
            Vector3 position = systems[currentSystem].planets[i].location * 800;
            GameObject planet = (GameObject)Instantiate(getPrefabPlanet(planetPrefabID), position, Quaternion.identity);
            controller.configureController(currentSystem, i, this, planet, bossSpawn == i);

            controllers.Add(controller);


            if (i == currentPlanet)
            {
                if (tmpPlayer != null)
                {
                    controller.spawnShip(tmpPlayer, false);
                }
                else
                {
                    setCurrentShip(player.currentShip);
                }
            }

        }

        updateStatusPlayerVision(currentSystem, currentPlanet);

        // gameObject.AddComponent("MainHUD");
        gameState = GameState.Playing;
    }

    /// <summary>
    /// This should be called when the ship has travelled to the required point and needs to actually jump
    /// </summary>
    /// <param name="obj">The object that wants to jump. Typically the player.</param>
    /// <param name="targetSystem">The target system to jump to.</param>
    /// <param name="targetPlanet">The target planet in the target system.</param>
    public void performHyperspaceJump(GameObject obj, int targetSystem, int targetPlanet)
    {
        if (obj.tag == "Player")
        {
            hyperspaceJumpTo(targetSystem, targetPlanet);
        }
        else
        {
            ShipAI shipData = obj.GetComponent<ShipAI>();
            if (currentSystem != targetSystem)
            {
                shipData.controller.removeShip(obj);
                DestroyObject(obj);
            }
            else
            {
                shipData.controller.removeShip(obj);
                controllers[targetPlanet].spawnShip(obj, false);
            }
        }
    }

    /// <summary>
    /// Set the hyperspace target for the AI
    /// </summary>
    /// <param name="obj">The object to choose a target for.</param>
    public void setAIHyperspaceTarget(GameObject obj)
    {
        ShipAI shipData = obj.GetComponent<ShipAI>();
        int cPlanet = shipData.controller.planetID;

        int bestPlanet = -1;
        float quotient = -1;
        DataManager.Race race = shipData.race;

        foreach (AIController c in controllers)
        {
            if (c.planetID == cPlanet) continue;

            float inc = 0;
            float dec = 0;

            DataManager.ForceSize[] force = c.getRaceForceSizes();
            for (int i = 0; i < 6; i++)
            {
                if (DataManager.isAlly((DataManager.Race)i, race)) inc += (int)force[i];
                else dec += (int)force[i];
            }

            float nquotient = 0;
            if (dec == 0) nquotient = inc;
            else nquotient = inc / dec;

            if (nquotient > quotient)
            {
                bestPlanet = c.planetID;
                quotient = nquotient;
            }
        }

        shipData.hyperspaceTargetSystem = currentSystem;
        shipData.hyperspaceTargetPlanet = bestPlanet;
        shipData.setMoveMode(ShipAI.MoveMode.EnterHyperspace);
    }

    public Vector3 spawnHyperspaceWindow(Vector3 location, bool atLocation)
    {
        if (!atLocation)
        {
            location = getVector3InRangeOfPoint(location, 50, 80);
        }

        Instantiate(prefabHyperspace, location, Quaternion.identity);

        return location;
    }

    public Vector3 getVector3InRangeOfPoint(Vector3 pos, float minDistance, float maxDistance)
    {
        Vector3 location = new Vector3(pos.x, pos.y, pos.z);
        float rand1 = Random.Range(minDistance, maxDistance);
        float rand2 = Random.Range(minDistance, maxDistance);
        float rand3 = Random.Range(minDistance, maxDistance);

        location.x += (Random.value > 0.5) ? rand1 : -rand1;
        location.y += (Random.value > 0.5) ? rand2 : -rand2;
        location.z += (Random.value > 0.5) ? rand3 : -rand3;

        return location;
    }

    /// <summary>
    /// Initiates the completion of hyperspace jumps when the level has loaded up.
    /// </summary>
    /// <param name="level"></param>
    void OnLevelWasLoaded(int level)
    {
        if (level == 2)
        {
            completeHyperspaceJump();
        }
    }

    /// <summary>
    /// Switch the scene to the main menu. This will destory this object in the process.
    /// </summary>
    public void exitToMainMenu()
    {
        Destroy(gameObject);
        Application.LoadLevel("MainMenu");
    }

    // Update is called once per frame
    void Update()
    {
        updateMusic();

        if (gameState == GameState.Loading)
        {
            loadCompleteCooldown -= Time.deltaTime;
            if (loadCompleteCooldown <= 0)
            {
                Application.LoadLevel("Game");
            }

            return;
        }
        updateSimulateBattles();

        if (callAlliesCooldown > 0)
        {
            callAlliesCooldown -= Time.deltaTime;
        }

        // check if the current area the ship is set to is still valid
        switchPlayerAreaCooldown -= Time.deltaTime;
        if (switchPlayerAreaCooldown <= 0)
        {
            GameObject playerShip = GameObject.FindGameObjectWithTag("Player");
            ShipAI shipData = (ShipAI)playerShip.GetComponent("ShipAI");
            Vector3 playerLocation = playerShip.transform.position;

            AIController currentController = shipData.controller;
            float currentDistance = Vector3.Distance(playerLocation, currentController.planet.transform.position);

            AIController closerController = null;
            float minDistance = 0;

            foreach (AIController c in controllers)
            {
                if (c == currentController) continue;

                float distance = Vector3.Distance(playerLocation, c.planet.transform.position);
                if (distance / currentDistance <= 0.8f && (distance < minDistance || closerController == null))
                {
                    closerController = c;
                    minDistance = distance;
                }
            }

            if (closerController != null)
            {
                updateStatusPlayerVision(currentSystem, currentPlanet);
                currentController.removePlayerShip();
                currentPlanet = closerController.planetID;
                closerController.addShip(playerShip);
                updateStatusPlayerVision(currentSystem, currentPlanet);
            }

            switchPlayerAreaCooldown = 10.0f;
        }
    }

    private void updateMusic()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        /*if (gameState == GameState.Loading)
        {
            if(!audioSource.isPlaying) return;

            audioSource.Stop();
            return;
        }*/

        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            if(audioSource.isPlaying) return;
            audioSource.clip = music[Random.Range(2, music.Length - 1)];
            audioSource.Play();
            return;
        }

        ShipAI playerData = GameObject.FindGameObjectWithTag("Player").GetComponent<ShipAI>();

        if(playerData.inCombat)
        {
            if(!audioSource.isPlaying || ( audioSource.clip != music[0] && audioSource.clip != music[1] ))
            {
                audioSource.Stop();
                audioSource.clip = music[Random.Range(0,1)];
                audioSource.Play();
            }
        } 
        else {
            if( audioSource.clip == music[0] || audioSource.clip == music[1] )
            {
                audioSource.Stop();
                audioSource.clip = music[Random.Range(2, music.Length - 1)];
                audioSource.Play();
            } 
            else if(!audioSource.isPlaying)
            {
                audioSource.clip = music[Random.Range(0, music.Length - 1)];
                audioSource.Play();
            }
        }

       /* if(!audioSource.isPlaying)
        {
            audioSource.clip = music[Random.Range(0, music.Length - 1)];
            audioSource.Play();
        }     */
    }

    private void updateSimulateBattles()
    {
        if (!battleSimEnabled) return;

        countDownToBattle -= Time.deltaTime;
        if (countDownToBattle > 0) return;
        countDownToBattle = timeBetweenBattles;

        List<DataManager.Battle> battles = new List<DataManager.Battle>();
        List<DataManager.Battle> delayedBattles = new List<DataManager.Battle>();

        // get list of battles
        for (int i = 0; i < 6; i++)
        {
            if (i == (int)DataManager.Race.Human) continue;

            List<DataManager.Battle> tempBattles = getPossibleBattles((DataManager.Race)i);
            foreach (DataManager.Battle battle in tempBattles)
            {
                if (battle.systemID == currentSystem)
                    delayedBattles.Add(battle);
                else
                    battles.Add(battle);
            }
        }

        List<Vector2> positions = new List<Vector2>();
        // populate all the areas with the correct number of things.
        foreach (DataManager.Battle battle in battles)
        {
            DataManager.ForceSize oldSize = systems[battle.systemID].planets[battle.planetID].raceForceSizes[(int)battle.race];
            int count = DataManager.getShipCountFromForceSize(oldSize);
            count += DataManager.getShipCountFromForceSize(battle.forceSize);
            systems[battle.systemID].planets[battle.planetID].raceForceSizes[(int)battle.race] = DataManager.getForceSizeFromCount(count);

            Vector2 pos = new Vector2(battle.systemID, battle.planetID);
            positions.Add(pos);
        }

        // sim the battles
        foreach (Vector2 pos in positions)
        {
            simulateBattle((int)pos.x, (int)pos.y);
        }

        // spawn all the enemies in the current system if any
        for(int planetID = 0; planetID < 5; planetID++)
        {
            DataManager.ForceSize[] shipForceToSpawn = new DataManager.ForceSize[6];
            for(int i = 0; i < 6; i++)
            {
                shipForceToSpawn[i] = DataManager.ForceSize.None;
            }

            foreach (DataManager.Battle battle in delayedBattles)
            {
                if(battle.planetID != planetID) continue;

                DataManager.ForceSize oldSize = shipForceToSpawn[(int)battle.race];
                int count = DataManager.getShipCountFromForceSize(oldSize);
                count += DataManager.getShipCountFromForceSize(battle.forceSize);
                shipForceToSpawn[(int)battle.race] = DataManager.getForceSizeFromCount(count);
            }

            controllers[planetID].setupShipCollection(shipForceToSpawn, false, false);
        }
    }

    private List<DataManager.Battle> getPossibleBattles(DataManager.Race race)
    {
        List<DataManager.Battle> possibleBattles = new List<DataManager.Battle>();

        List<int> systemIDs = new List<int>();

        for (int i = 1; i < systems.Length; i++)
        {
            bool canAdd = false;

            if (systems[i].isNeutralZone)
            {
                // if there is an adjacent cell that is controlled add to list
                for (int j = 0; j < systems[i].linkedNodes.Length; j++)
                {
                    if (isOccupiedBy(systems[i].linkedNodes[j], race))
                    {
                        canAdd = true;
                        break;
                    }
                }
            }
            else if (isOccupiedBy(i, race))
            {
                canAdd = true;
            }

            if (canAdd)
            {
                int highLevelCount = 0;
                for (int j = 0; j < systems[i].planets.Length; j++)
                {
                    if ((int)systems[i].planets[j].raceForceSizes[(int)race] >= (int)DataManager.ForceSize.Medium)
                        highLevelCount++;
                }

                if (Random.Range(0, 5) > highLevelCount)
                {
                    systemIDs.Add(i);
                }
            }
        }

        int battleCount = Random.Range(0, 3);

        if (systemIDs.Count == 0) battleCount = 0;

        for (int i = 0; i < battleCount; i++)
        {
            DataManager.Battle battle = new DataManager.Battle();
            battle.forceSize = DataManager.getForceSizeFromCount(Random.Range(1, 4));
            battle.race = race;
            battle.systemID = systemIDs[Random.Range(0, systemIDs.Count - 1)];
            battle.planetID = Random.Range(0, 4);

            possibleBattles.Add(battle);
        }
        return possibleBattles;
    }

    private bool isOccupiedBy(int systemID, DataManager.Race race)
    {
        for (int i = 0; i < systems[i].planets.Length; i++)
        {
            if (systems[systemID].planets[i].raceForceSizes[(int)race] != DataManager.ForceSize.None)
            {
                return true;
            }
        }
        return false;
    }

    private void simulateBattle(int systemID, int planetID)
    {
        int round = Random.Range(2, 5);


        for (int i = 0; i < round; i++)
        {
            DataManager.Race race = DataManager.Race.Human;
            int count = 0;
            for (int j = 0; j < 6; j++)
            {
                if (systems[systemID].planets[planetID].raceForceSizes[j] != DataManager.ForceSize.None)
                {
                    if (race == DataManager.Race.Human)
                    {
                        race = (DataManager.Race)j;
                    }
                    count++;
                }
            }

            if (count == 1)
            {
                claimPlanet(systemID, planetID, race);
                break;
            }
            else
            {
                int decID = Random.Range(0, count - 1);
                for (int j = 0; j < 6; j++)
                {
                    if (systems[systemID].planets[planetID].raceForceSizes[j] != DataManager.ForceSize.None)
                    {
                        if (decID == 0)
                        {
                            DataManager.ForceSize oldSize = systems[systemID].planets[planetID].raceForceSizes[j];
                            int c = DataManager.getShipCountFromForceSize(oldSize);
                            c -= Random.Range(0, 2);
                            systems[systemID].planets[planetID].raceForceSizes[j] = DataManager.getForceSizeFromCount(c);
                        }
                        else
                        {
                            decID--;
                        }
                    }
                }
            }

        }

    }

    public void storePlayerShipState()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        // for all cases except for entering the game the setting will 
        if (playerObj != null)
        {
            ShipAI oldplayerShipData = (ShipAI)playerObj.GetComponent("ShipAI");

            // store the old ships configuration
            player.ships[player.currentShip].shieldPower = oldplayerShipData.shieldPower;
            player.ships[player.currentShip].weaponPower = oldplayerShipData.weaponPower;
            player.ships[player.currentShip].speedPower = oldplayerShipData.speedPower;
            player.ships[player.currentShip].enabledWeapons = oldplayerShipData.getCountsofWeaponsEnabled();
        }
    }

    public void increasePlayerExp(int amount)
    {
        if (player.ships[player.currentShip].shipLevel == 9) return;

        player.ships[player.currentShip].shipExp += amount;
        displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
        msgHandle.texter("Gained " + amount + " experience!", 3);
        if (player.ships[player.currentShip].shipExp > 100 * Mathf.Exp(player.ships[player.currentShip].shipLevel))
        {
            player.ships[player.currentShip].shipLevel++;
            msgHandle.texter("Ship has leveled up to level " + player.ships[player.currentShip].shipLevel + "!", 3);
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            ShipAI playerShipData = (ShipAI)playerObj.GetComponent("ShipAI");
            playerShipData.configureShip(player.ships[player.currentShip].shipLevel,
                    DataManager.Race.Human, playerShipData.shipData, playerShipData.controller, this);
        }
    }

    public bool purchaseWeapon(int weaponID)
    {
        if (!canBuyWeapon(weaponID))
        {
            print("failed to buy weapon?");
            return false;
        }

        player.ships[player.currentShip].shipWeapons[weaponID]++;
        payResources(shipDatabase.weapons[weaponID].resources);
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        ShipAI playerShipData = (ShipAI)playerObj.GetComponent("ShipAI");
        playerShipData.equipWeapon(weaponID, 1);
        return true;
    }

    public bool buyShip(int shipID)
    {
        if (!canBuyShip(shipID))
        {
            print("failed to buy ship?");
            return false;
        }

        // configure the initial configuration
        player.ships[shipID].owned = true;

        for (int i = 0; i < shipDatabase.weapons.Length; i++)
            player.ships[shipID].shipWeapons[i] = shipDatabase.ships[shipID].AIStats[0].maxWeapons[i];

        payResources(shipDatabase.ships[player.ships[shipID].shipID].resources);
        return true;
    }

    public bool canBuyShip(int shipID)
    {
        if (!haveResourcesFor(shipDatabase.ships[player.ships[shipID].shipID].resources)) return false;
        if (player.ships[shipID].owned) return false;

        return true;
    }

    public bool canBuyWeapon(int weaponID)
    {
        if (!haveResourcesFor(shipDatabase.weapons[weaponID].resources)) return false;
        if (player.ships[player.currentShip].shipWeapons[weaponID] + 1 > shipDatabase.ships[player.ships[player.currentShip].shipID].AIStats[player.ships[player.currentShip].shipLevel].maxWeapons[weaponID]) return false;

        return true;
    }

    public int[] getPurchasableWeapons()
    {
        int[] maxPurchaseCount = new int[shipDatabase.weapons.Length];
        for (int i = 0; i < shipDatabase.weapons.Length; i++)
        {
            maxPurchaseCount[i] = shipDatabase.ships[player.ships[player.currentShip].shipID].AIStats[player.ships[player.currentShip].shipLevel].maxWeapons[i] - player.ships[player.currentShip].shipWeapons[i];
        }
        return maxPurchaseCount;
    }

    private bool haveResourcesFor(int[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (player.resources[i] < resources[i]) return false;
        }

        return true;
    }

    private void payResources(int[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            player.resources[i] -= resources[i];
        }
    }

    public void addResources(int[] resources)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            player.resources[i] += resources[i];
        }
    }

    public void setCurrentShip(int playerShipID)
    {
        // Should never happen - but handles error by negating
        if (!player.ships[playerShipID].owned) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        // for all cases except for entering the game the setting will 
        if (playerObj != null)
        {
            ShipAI oldplayerShipData = (ShipAI)playerObj.GetComponent("ShipAI");

            oldplayerShipData.controller.removePlayerShip();

            // create the new ship and switch
            player.currentShip = playerShipID;
            GameObject newPlayerShip = (GameObject)Instantiate(getPrefabShip(player.ships[player.currentShip].shipID), playerObj.transform.position, playerObj.transform.rotation);
            ShipAI playerShipData = (ShipAI)newPlayerShip.GetComponent("ShipAI");

            // configure the new ship
            playerShipData.configureShip(player.ships[player.currentShip].shipLevel,
                DataManager.Race.Human, getShipData(player.ships[player.currentShip].shipID), controllers[currentPlanet], this);
            oldplayerShipData.controller.addShip(newPlayerShip);

            // remove the old one
            Destroy(playerObj);
        }
        else
        {
            // create the new ship and switch
            player.currentShip = playerShipID;
            GameObject newPlayerShip = (GameObject)Instantiate(getPrefabShip(player.ships[player.currentShip].shipID));
            ShipAI playerShipData = (ShipAI)newPlayerShip.GetComponent("ShipAI");

            // configure the new ship
            playerShipData.configureShip(player.ships[player.currentShip].shipLevel,
                DataManager.Race.Human, getShipData(player.ships[player.currentShip].shipID), controllers[currentPlanet], this);

            controllers[currentPlanet].spawnShip(newPlayerShip, false);
        }
    }

    public void attemptCallAllies()
    {
        if (callAlliesCooldown > 0)
        {
            // trigger decline AlliesCall message too soon
            displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
            msgHandle.texter("Don\'t try to call your allies so often!", 2);
            return;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        ShipAI playerShipData = (ShipAI)playerObj.GetComponent("ShipAI");

        if ((int)playerShipData.controller.getRaceForceSizes()[(int)DataManager.Race.Asgard] <= (int)DataManager.ForceSize.Medium && Random.Range(0.0f, 1.0f) <= 0.2f)
        {
            // spawn 1 to 3 Asgard Motherships with levels less than or equal to that of the player
            int count = Random.Range(1, 3);
            for (int i = 0; i < count; i++)
            {
                playerShipData.controller.spawnShip(Random.Range(0, playerShipData.shipLevel), 3, DataManager.Race.Asgard, false);
            }

            displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
            msgHandle.texter("Allies are on their way!", 4);
        }
        else
        {
            // trigger decline AlliesCall message rejected
            displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
            msgHandle.texter("Allies have declined your request for assistance!", 2);
        }

        callAlliesCooldown = 60;
    }

    public void claimPlanet(int systemID, int planetID, DataManager.Race race)
    {
        if (systems[systemID].planets[planetID].status == DataManager.PlanetStatus.Homeworld
            || systems[systemID].planets[planetID].status == DataManager.PlanetStatus.Tradeworld)
            return;

        bool updateVisionAfter = false;

        // if the status is changing from or to friendly
        if (systems[systemID].planets[planetID].status == DataManager.PlanetStatus.Friendly
             || DataManager.isAlly(race, DataManager.Race.Human))
        {
            // update the last seen
            updateVisionAfter = true;
        }

        systems[systemID].planets[planetID].controller = race;
        systems[systemID].planets[planetID].status = (DataManager.isAlly(race, DataManager.Race.Human)) ? DataManager.PlanetStatus.Friendly : DataManager.PlanetStatus.Hostile;
        systems[systemID].planets[planetID].raceForceSizes = controllers[planetID].getRaceForceSizes();

        if (updateVisionAfter)
        {
            updateStatusPlayerVision(systemID, planetID);
        }
    }

    public void updateStatusPlayerVision(int systemID, int planetID)
    {
        int max = 0;
        int maxIndex = -1;

        if (systemID >= systems.Length || planetID >= systems[systemID].planets.Length)
        {
            Debug.Log("WARNING: ids are out of range!!" + systemID + " " + planetID);
        }

        for (int i = 0; i < systems[systemID].planets[planetID].raceForceSizes.Length; i++)
        {
            if (i == (int)DataManager.Race.Human) continue;

            if ((int)systems[systemID].planets[planetID].raceForceSizes[i] > max)
            {
                maxIndex = i;
                max = (int)systems[systemID].planets[planetID].raceForceSizes[i];
            }
        }

        if (maxIndex == -1)
        {
            systems[systemID].planets[planetID].lastController = systems[systemID].planets[planetID].controller;
        }
        else
        {
            systems[systemID].planets[planetID].lastController = (DataManager.Race)maxIndex;
        }

        systems[systemID].planets[planetID].lastMaxForceSize = (DataManager.ForceSize)max;
        systems[systemID].planets[planetID].lastStatus = systems[systemID].planets[planetID].status;
    }

    public List<GameObject> getTargetableObjects()
    {
        List<GameObject> collection = new List<GameObject>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        foreach (AIController c in controllers)
        {
            collection = orderedInsertbyLocation(collection, c.planet, player.transform.position);
        }

        List<GameObject> others = controllers[currentPlanet].getTargetableObjects();
        foreach (GameObject other in others)
            collection.Add(other);

        return collection;
    }

    private List<GameObject> orderedInsertbyLocation(List<GameObject> list, GameObject insert, Vector3 playerLocation)
    {
        if (insert == null) return list;
        float distance = Vector3.Distance(playerLocation, insert.transform.position);

        int i = 0;
        for (; i < list.Count; i++)
        {
            if (Vector3.Distance(playerLocation, list[i].transform.position) > distance)
            {
                break;
            }
        }

        list.Insert(i, insert);

        return list;
    }

    public List<GameObject> getPlanetList()
    {
        List<GameObject> planets = new List<GameObject>();
        foreach (AIController controller in controllers)
        {
            planets.Add(controller.planet);
        }
        return planets;
    }

    public GameObject getPrefabShip(int shipPrefabID)
    {
        if (shipPrefabID >= prefabShips.Count)
        {
            Debug.LogError("ERROR: Invalid shipPrefabID was referenced in GameController! " + "PrefabID: " + shipPrefabID);
        }

        return prefabShips[shipPrefabID];
    }

    public GameObject getPrefabPlanet(int planetPrefabID)
    {
        if (planetPrefabID >= prefabPlanets.Count)
        {
            Debug.LogError("ERROR: Invalid planetPrefabID was referenced in GameController!");
        }

        return prefabPlanets[planetPrefabID];
    }

    public GameObject getPrefabWeapon(int weaponPrefabID)
    {
        if (weaponPrefabID >= prefabWeapons.Count)
        {
            Debug.LogError("ERROR: Invalid weaponPrefabID was referenced in GameController!");
        }

        return prefabWeapons[weaponPrefabID];
    }

    public DataManager.ShipData getShipData(int shipID)
    {
        for (int i = 0; i < shipDatabase.ships.Length; i++)
        {
            if (shipDatabase.ships[i].shipID == shipID)
            {

                return shipDatabase.ships[i];
            }
        }

        Debug.LogError("ERROR: No ship with that id was found.");
        return new DataManager.ShipData(); ;
    }

    public DataManager.WeaponData getWeaponData(int weaponID)
    {
        for (int i = 0; i < shipDatabase.weapons.Length; i++)
        {
            if (shipDatabase.weapons[i].weaponID == weaponID)
            {
                return shipDatabase.weapons[i];
            }
        }

        Debug.LogError("ERROR: No weapon with that id was found.");
        return new DataManager.WeaponData(); ;
    }

    public GameObject getPrefabWreck()
    {
        return prefabWreckedShip;
    }

    public GameObject getPrefabExplosion()
    {
        return prefabExplosion;
    }

    public void setSkyboxMat(int id)
    {
        Skybox s = Camera.main.GetComponent<Skybox>();
        s.material = skyboxMaterials[id];
    }

    public void saveGame()
    {
        DataManager.SaveData saveData = new DataManager.SaveData();
        saveData.player = player;
        saveData.systems = systems;

        loader.saveData(saveData);
    }

    public void loadGame()
    {
        DataManager.SaveData saveData = loader.loadPlayerData(false);
        systems = (DataManager.SystemData[])saveData.systems.Clone();

        player = saveData.player;

        if (player.currentShip == -1)
        {
            buyShip(0);
            player.currentShip = 0;
        }

        timeBetweenBattles = 60;
        callAlliesCooldown = 60;
        switchPlayerAreaCooldown = 10;
        countDownToBattle = timeBetweenBattles;
        battleSimEnabled = false;

        currentSystem = 0;
        currentPlanet = 0;

        hyperspaceJumpTo(0, 0);
    }

    public void printOutShipDatabase()
    {
        StreamWriter writer = new StreamWriter("c:\\Ships.txt");
        writer.WriteLine("*******************************");
        writer.WriteLine("******  SHIP DATABASE  ********");
        writer.WriteLine("*******************************\n");

        for (int z = 0; z < shipDatabase.ships.Length; z++)
        {
            DataManager.ShipData ship = shipDatabase.ships[z];

            writer.WriteLine("Ship Name: " + ship.shipName);
            writer.WriteLine("Ship ID: " + ship.shipID);
            writer.WriteLine("Prefab ID: " + ship.prefabID);
            writer.WriteLine("Race: " + ship.shipRace);
            writer.WriteLine("Is Boss: " + ship.isBoss);
            writer.WriteLine("Turn Speed: " + ship.turnSpeed);

            if (ship.shipID < 3)
                writer.Write("Resource Cost: ");
            else
                writer.Write("Drop Quantities: ");


            for (int i = 0; i < ship.resources.Length; i++)
                writer.Write(ship.resources[i] + " ");

            writer.WriteLine("\nResource Multiplier: " + ship.resourceMultiplier);

            writer.WriteLine("\nPrimary Stats");
            string level_line = "Level:  ";
            string hull_line = "Hull:   ";
            string shield_line = "Shield: ";
            string power_line = "Power:  ";

            string[] WeaponLines = new string[10];
            string weaponheader = "Level: ";
            for (int i = 0; i < 10; i++)
            {
                weaponheader += string.Format("{0,4}|", i);
                WeaponLines[i] = string.Format("{0, 7}", i);
            }

            for (int i = 0; i < ship.AIStats.Length; i++)
            {
                level_line += string.Format("{0, 8}", (i + 1));
                hull_line += string.Format("{0, 8}", ship.AIStats[i].hull);
                shield_line += string.Format("{0, 8}", ship.AIStats[i].shield);
                power_line += string.Format("{0, 8}", ship.AIStats[i].power);

                for (int j = 0; j < ship.AIStats[i].maxWeapons.Length; j++)
                {
                    WeaponLines[i] += string.Format("{0,4}|", ship.AIStats[i].maxWeapons[j]);
                }
            }

            writer.WriteLine(level_line);
            writer.WriteLine(hull_line);
            writer.WriteLine(shield_line);
            writer.WriteLine(power_line);

            writer.WriteLine("\n" + weaponheader);
            for (int i = 0; i < 10; i++)
            {
                writer.WriteLine(WeaponLines[i]);
            }

            writer.WriteLine();
        }

        writer.WriteLine("*******************************");
        writer.WriteLine("*****  WEAPON DATABASE  *******");
        writer.WriteLine("*******************************\n");

        foreach (DataManager.WeaponData weapon in shipDatabase.weapons)
        {
            writer.WriteLine("Weapon ID: " + weapon.weaponID);
            writer.WriteLine("Weapon Name: " + weapon.weaponName);
            writer.WriteLine("Prefab ID: " + weapon.prefabID);
            writer.WriteLine("Damage Type: " + weapon.damageType);
            writer.WriteLine("Base Damage: " + weapon.damageValue);
            writer.WriteLine("Effect Type: " + weapon.effectType);
            writer.WriteLine("Fire rate: " + weapon.fireRate);
            writer.WriteLine("Power cost: " + weapon.powerValue);
            writer.Write("Cost: ");
            for (int i = 0; i < weapon.resources.Length; i++)
                writer.Write("{0,6}", weapon.resources[i]);

            writer.WriteLine();
        }

        writer.Close();
    }

    public void printOutPlayerSaveData()
    {
        StreamWriter writer = new StreamWriter("c:\\Player.txt");
        writer.WriteLine("*******************************");
        writer.WriteLine("*******  System Data  *********");
        writer.WriteLine("*******************************\n");

        for (int z = 0; z < systems.Length; z++)
        {
            DataManager.SystemData system = systems[z];

            writer.WriteLine("System ID: " + system.systemID);
            writer.WriteLine("Owner: " + system.owner);
            writer.WriteLine("Is Neutral Zone: " + system.isNeutralZone);
            writer.WriteLine("Has Boss: " + system.hasBoss);
            writer.WriteLine("Skybox ID: " + system.skyboxID);
            writer.WriteLine("Map Grid Location: (" + system.mapGridLocation.x + "," + system.mapGridLocation.y + ")");
            writer.Write("Linked Nodes: ");

            for(int i = 0; i < system.linkedNodes.Length; i++)
            {
                writer.Write(system.linkedNodes[i] + " ");
            }
             writer.WriteLine();
             writer.WriteLine();

            for(int i = 0; i < system.planets.Length; i++)
            {
               writer.WriteLine("Planet ID: " + i);
                writer.WriteLine("Controller: " + system.planets[i].controller);
                writer.WriteLine("Planet Status: " + system.planets[i].status);
                writer.WriteLine("Location: (" + system.planets[i].location.x + "," + system.planets[i].location.y + "," + system.planets[i].location.z + ")");
                writer.WriteLine("Map Location: (" + system.planets[i].mapLocation.x + "," + system.planets[i].mapLocation.y + ")");
                writer.WriteLine("Texture ID: " + system.planets[i].textureID);
                writer.WriteLine("Last Controller: " + system.planets[i].lastController);
                writer.WriteLine("Last ForceSize: " + system.planets[i].lastMaxForceSize);
                writer.WriteLine("Last Status: " + system.planets[i].lastStatus);

                for(int  j = 0; j < system.planets[i].raceForceSizes.Length; j++)
                {
                    writer.Write((DataManager.Race)j + ": " + system.planets[i].raceForceSizes[j] + " ");
                }
                writer.WriteLine();
            }
            writer.WriteLine();
        }

        writer.WriteLine("*******************************");
        writer.WriteLine("********  Player Save *********");
        writer.WriteLine("*******************************\n");

        writer.WriteLine("Current ship: " + player.currentShip);
        writer.Write("Resources: ");
        for(int i = 0; i < player.resources.Length; i++)
        {
            writer.Write(player.resources[i] + " ");
        }
        writer.WriteLine();

        for(int i = 0; i < player.ships.Length; i++)
        {
            writer.WriteLine();
            writer.WriteLine("Ship ID: " + player.ships[i].shipID);
            writer.WriteLine("Ship Level: " + player.ships[i].shipLevel);
            writer.WriteLine("Owned: " + player.ships[i].owned);
            writer.WriteLine("Exp: " + player.ships[i].shipExp);

            writer.Write("Weapons Equipped: ");
            for(int j = 0; j < player.ships[i].shipWeapons.Length; j++)
                writer.Write(player.ships[i].shipWeapons[j] + " ");
            writer.WriteLine();

            writer.WriteLine("Shield Power: " + player.ships[i].shieldPower);
            writer.WriteLine("Speed Power: " + player.ships[i].speedPower);
            writer.WriteLine("Weapon Power: " + player.ships[i].weaponPower);
            
            writer.Write("Weapons Enabled: ");
            for(int j = 0; j < player.ships[i].enabledWeapons.Length; j++)
                writer.Write(player.ships[i].enabledWeapons[j] + " ");
            writer.WriteLine();
        }
    
        writer.Close();
    }
}
