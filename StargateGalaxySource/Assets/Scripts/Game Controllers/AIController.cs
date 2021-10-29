using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : MonoBehaviour {
	public GameController game;

    public int systemID;
    public int planetID;

    private RaceShips[] raceShips;
	private List<GameObject> wreckedships;

    public GameObject planet;

    bool begunCapture;
    float captureProgress;
    DataManager.Race raceCapturing;

    // Use this for initialization
	void Start () {
	}

    public void configureController(int systemID, int planetID, GameController game, GameObject planet, bool boss)
    {
        this.game = game;
        this.systemID = systemID;
        this.planetID = planetID;
        this.planet = planet;

        raceShips = new RaceShips[6];

        for (int i = 0; i < 6; i++)
        {
            raceShips[i] = new RaceShips();
            raceShips[i].count = 0;
            raceShips[i].numberOfEnemies = 0;
            raceShips[i].ships = new List<GameObject>();
        }

        begunCapture = false;
        captureProgress = 0.0f;

        /*systemID = 0;
        planetID = 0;*/

        wreckedships = new List<GameObject>();

        // TODO UNCOMMENT ME:
        //setupShipCollection(game.systems[systemID].planets[planetID].raceForceSizes, true, boss);

        if (game.isDebugMode)
        {
            /*spawnShip(0, 3, DataManager.Race.Asgard, true);
            spawnShip(0, 3, DataManager.Race.Asgard, true);
            spawnShip(0, 3, DataManager.Race.Asgard, true);
            spawnShip(0, 8, DataManager.Race.Wraith, true);
            spawnShip(0, 8, DataManager.Race.Wraith, true);
            spawnShip(0, 8, DataManager.Race.Wraith, true);
            spawnShip(0, 4, DataManager.Race.Goauld, true);
            spawnShip(0, 4, DataManager.Race.Goauld, true);
            spawnShip(0, 4, DataManager.Race.Goauld, true);
            */


            //spawnShip(0, 11, DataManager.Race.Ori, true);
            //spawnShip(0, 11, DataManager.Race.Ori, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateCaptureStatus();

        if(!isPlayerShipHere()) 
        {
            if (wreckedships.Count > 0)
            {
                foreach (GameObject ship in wreckedships)
                    DestroyObject(ship);

                wreckedships.Clear();
            }
        }
	}

    public void updateCaptureStatus()
    {
        // if the capturing of a planet has begun
        if (begunCapture)
        {
            // if the current progress is complete
            if (captureProgress >= 10.0f)
            {
                game.claimPlanet(systemID, planetID, raceCapturing);
                begunCapture = false;

                if (raceCapturing == DataManager.Race.Human)
                {
                    displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
                    msgHandle.texter("Planet now under your control!", 3);
                }
            }
            else
            {
                // check if a stop condition has been met
                //for (int i = 0; i < (int)DataManager.Race.Ori; i++)
                //{
                    //if (raceShips[i].count > 0 && !DataManager.isAlly((DataManager.Race)i, raceCapturing))
                    if(raceShips[(int)raceCapturing].numberOfEnemies > 0)
                    {
                        begunCapture = false;
                        captureProgress = 0.0f;

                        if (raceCapturing == DataManager.Race.Human)
                        {
                            displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
                            msgHandle.texter("Capturing planet has been interupted!", 2);
                        }
                    }
               // }

                if (begunCapture) captureProgress += Time.deltaTime;
            }
        }
        else
        {
            // check if there is a single controller to allow capturing to begin
            bool singleController = true;
            bool isRaceFound = false;
            int race = -1;
            for (int i = 0; i < raceShips.Length; i++)
            {
                if (raceShips[i].count > 0)
                {
                    isRaceFound = true;

                    if (race == -1)
                    {
                        race = i;
                    }
                    else if (DataManager.isAlly((DataManager.Race)i, (DataManager.Race)race))
                    {
                        race = (int)DataManager.Race.Human;
                    }
                    else
                    {
                        singleController = false;
                    }
                }
            }

            // check there is a single controller and the new controller is not already friendly with the current one.
            // also only let human/asgard capture or if race is the owner of the system but let anyone if system is neutral
            if (isRaceFound && singleController && !DataManager.isAlly(game.systems[systemID].planets[planetID].controller, (DataManager.Race)race)
                && (game.systems[systemID].isNeutralZone
                    || DataManager.isAlly(DataManager.Race.Human, (DataManager.Race)race)
                    || (DataManager.Race)race == game.systems[systemID].owner))
            {
                begunCapture = true;
                raceCapturing = (DataManager.Race)race;

                if (raceCapturing == DataManager.Race.Human)
                {
                    displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
                    msgHandle.texter("Begun capturing planet!", 3);
                }
            }
        }
    }

	struct RaceShips
	{
		// precomputed count of the list
		public int count;
		// precomputer number based on 
		public int numberOfEnemies; 
		public List<GameObject> ships;
	}
	
    /// <summary>
    /// Choose a random target non-inclusive of allies.
    /// </summary>
    /// <param name="race">The friendly race modifier.</param>
    /// <returns></returns>
	public GameObject selectRandomTarget(DataManager.Race race)
	{
		int targetid = Random.Range(0, raceShips[(int)race].numberOfEnemies);
		
		for(int i = 0; i <= (int)DataManager.Race.Ori; i++)
		{
			if(DataManager.isAlly(race, (DataManager.Race)i))
			 {
				continue;
			 }
			 
			 if(targetid - raceShips[i].count < 0) 
			 {
			 	return (GameObject)(raceShips[i].ships[targetid]);
			 } else {
			 	targetid -= raceShips[i].count;
			 }
		}

        // no target
        return null;
	}

    /// <summary>
    /// Choose a random target that can be anything other than the ship itself
    /// </summary>
    /// <param name="race">The ship not to select</param>
    /// <returns></returns>
    public GameObject selectRandomTarget(GameObject notthis)
    {
        if(raceShips[(int)((ShipAI)notthis.GetComponent("ShipAI")).race].numberOfEnemies == 0 
            && raceShips[(int)((ShipAI)notthis.GetComponent("ShipAI")).race].count == 1) 
        {
            return null;
        }

        int targetid = Random.Range(0, raceShips[(int)DataManager.Race.Ori].numberOfEnemies + raceShips[(int)DataManager.Race.Ori].count);
        GameObject target = null;

        do
        {
            for (int i = 0; i <= (int)DataManager.Race.Ori; i++)
            {
                if (targetid - raceShips[i].count < 0)
                {
                    target = (GameObject)(raceShips[i].ships[targetid]);
                }
                else
                {
                    targetid -= raceShips[i].count;
                }
            }
        } while (target == notthis);

        // no target
        return target;
    }

    public int getEnemyCountofRace(DataManager.Race race)
    {
        return raceShips[(int)race].numberOfEnemies;
    }

    /// <summary>
    /// Get all the targets of friendlies that are enemies
    /// </summary>
    /// <param name="race">The race that is friendly</param>
    /// <returns></returns>
    public List<GameObject> getEnemyTargetOfFriends(DataManager.Race race)
    {
        List<GameObject> result = new List<GameObject>();
        
        for (int i = 0; i <= (int)DataManager.Race.Ori; i++)
        {
            if (!DataManager.isAlly(race, (DataManager.Race)i))
            {
                continue;
            }

            foreach (GameObject ship in raceShips[i].ships)
            {
                ShipAI shipData = (ShipAI)ship.GetComponent("ShipAI");
                if (!shipData.attackTarget) continue;

                if (shipData.target == null) continue; // a fix
                ShipAI targetData = (ShipAI)shipData.target.GetComponent("ShipAI");

                if (!DataManager.isAlly(race, targetData.race))
                {
                    if (!result.Contains(ship))
                        result.Add(ship);
                }
            }
        }

        return result;
    }
    
    /// <summary>
    /// Get all the enemies that are targetting the object specified.
    /// </summary>
    /// <param name="ship"></param>
    /// <returns></returns>
    public List<GameObject> getAllTargettingShip(GameObject ship)
    {
        List<GameObject> result = new List<GameObject>();

        ShipAI shipData = (ShipAI)ship.GetComponent("ShipAI");

        for (int i = 0; i <= (int)DataManager.Race.Ori; i++)
        {
            if (DataManager.isAlly(shipData.race, (DataManager.Race)i))
            {
                continue;
            }

            foreach (GameObject s in raceShips[i].ships)
            {
                ShipAI sData = (ShipAI)ship.GetComponent("ShipAI");
                if (!sData.attackTarget) continue;

                if (sData.target == ship)
                {
                    result.Add(s);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Get all the enemies that are targetting friendly ships
    /// </summary>
    /// <param name="race"></param>
    /// <returns></returns>
    public List<GameObject> getEnemyTargetingFriends(DataManager.Race race)
    {
        List<GameObject> result = new List<GameObject>();

        foreach (GameObject ship in raceShips[(int)race].ships)
        {
            List<GameObject> resultadd = getAllTargettingShip(ship);

            foreach (GameObject o in resultadd)
            {
                if (!result.Contains(o))
                    result.Add(o);
            }
        }

        if (race == DataManager.Race.Human)
        {
            foreach (GameObject ship in raceShips[(int)DataManager.Race.Asgard].ships)
            {
                List<GameObject> resultadd = getAllTargettingShip(ship);

                foreach (GameObject o in resultadd)
                {
                    if (!result.Contains(o))
                        result.Add(o);
                }
            }
        }
        else if (race == DataManager.Race.Asgard)
        {
            foreach (GameObject ship in raceShips[(int)DataManager.Race.Human].ships)
            {
                List<GameObject> resultadd = getAllTargettingShip(ship);

                foreach (GameObject o in resultadd)
                {
                    if (!result.Contains(o))
                        result.Add(o);
                }
            }
        }

        return result;
    }

    public GameObject selectObjectFromList(List<GameObject> objects)
    {
        int r = Random.Range(0, objects.Count-1);
        return objects[r];
    }

    public List<GameObject> getAllies(DataManager.Race race)
    {
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i <= (int)DataManager.Race.Ori; i++)
        {
            if(!DataManager.isAlly((DataManager.Race)i, race)) continue;

            foreach (GameObject g in raceShips[i].ships)
            {
                result.Add(g);
            }
        }

        return result;
    }

    public List<GameObject> getEnemies(DataManager.Race race)
    {
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i <= (int)DataManager.Race.Ori; i++)
        {
            if (DataManager.isAlly((DataManager.Race)i, race)) continue;

            foreach (GameObject g in raceShips[i].ships)
            {
                result.Add(g);
            }
        }

        return result;
    }

    public GameObject getNewTarget(GameObject ship)
    {
        ShipAI shipInfo = (ShipAI)ship.GetComponent("ShipAI");
        DataManager.Race race = shipInfo.race;

        if (raceShips[(int)race].numberOfEnemies == 0)
        {
            return null;
        }
        else if (raceShips[(int)race].numberOfEnemies == 1)
        {
            return selectRandomTarget(race);
        }

        List<GameObject> V = getAllTargettingShip(ship);
        if (raceShips[(int)race].numberOfEnemies <= V.Count * 2 && Random.Range(0.0f, 1.0f) >= 0.8f)
        {
            return selectObjectFromList(V);
        }

        List<GameObject> W = getEnemyTargetOfFriends(race);
        List<GameObject> Z = getEnemyTargetingFriends(race);

        if ((W.Count > 0 || Z.Count > 0) && Random.Range(0.0f, 1.0f) >= 0.6f)
        {
            if (W.Count == 0)
            {
                return selectObjectFromList(Z);
            }
            else if (Z.Count == 0)
            {
                return selectObjectFromList(W);
            }
            else if (Random.Range(0.0f, 1.0f) >= 0.5f)
            {
                return selectObjectFromList(Z);
            }
            else
            {
                return selectObjectFromList(W);
            }
        }
        else
        {
            return selectRandomTarget(race);
        }

    }

    public void spawnShip(int level, int shipID, DataManager.Race race, bool instant)
    {
        Vector3 location = getVector3InRangeOfPoint(planet.transform.position, (int)(planet.transform.localScale.x * 0.8), (int)(planet.transform.localScale.x * 1.2));

        DataManager.ShipData shipData = game.getShipData(shipID);
        GameObject newShip = (GameObject)Instantiate(game.getPrefabShip(shipData.prefabID), location, Quaternion.identity);
        ShipAI shipConfig = (ShipAI)newShip.GetComponent("ShipAI");
        shipConfig.configureShip(level, race, shipData, this, game);

        if (!instant)
        {
            // spawn hyperspace window
            game.spawnHyperspaceWindow(location, true);
            ShipAI shipAI = newShip.GetComponent<ShipAI>();
            Vector3 targetLocation = getVector3InRangeOfPoint(location, 50, 80);

            while (Vector3.Distance(targetLocation, planet.transform.position) < planet.transform.localScale.x * 1.2)
            {
                targetLocation = getVector3InRangeOfPoint(location, 50, 80);
            }

            shipAI.initiateMove(ShipAI.MoveMode.ExitHyperspace, targetLocation);
            newShip.transform.LookAt(targetLocation);
        }

        // add to race collection:
        raceShips[(int)race].ships.Add(newShip);
        raceShips[(int)race].count++;

        for (int i = 0; i <= (int)DataManager.Race.Ori; i++)
        {
            if (DataManager.isAlly(race, (DataManager.Race)i))
            {
                continue;
            }

            raceShips[i].numberOfEnemies++;
        }

        if (isPlayerShipHere())
            updateVisionStatus();
    }

    public void spawnShip(GameObject ship, bool instant)
    {
        addShip(ship);

        Vector3 location = getVector3InRangeOfPoint(planet.transform.position, 50, 100);
        ship.transform.position = location;

        if (!instant)
        {
            // spawn hyperspace window
            game.spawnHyperspaceWindow(location, true);
            ShipAI shipData = ship.GetComponent<ShipAI>();
            Vector3 targetLocation = getVector3InRangeOfPoint(location, 50, 80);
            shipData.initiateMove(ShipAI.MoveMode.ExitHyperspace, targetLocation);
            ship.transform.LookAt(targetLocation);
        }
    }

    public void addShip(GameObject ship)
    {
        ShipAI shipData = (ShipAI)ship.GetComponent("ShipAI");
        if (shipData == null) return;

        // add to race collection:
        raceShips[(int)shipData.race].ships.Add(ship);
        raceShips[(int)shipData.race].count++;
        shipData.controller = this;

        for (int i = 0; i <= (int)DataManager.Race.Ori; i++)
        {
            if (DataManager.isAlly(shipData.race, (DataManager.Race)i))
            {
                continue;
            }

            raceShips[i].numberOfEnemies++;
        }

        if (isPlayerShipHere())
            updateVisionStatus();
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

    public void removePlayerShip()
    {
        if (!isPlayerShipHere()) return;

        removeShip(GameObject.FindGameObjectWithTag("Player"));
    }

    public void removeShip(GameObject ship)
    {
        ShipAI shipData = (ShipAI)ship.GetComponent("ShipAI");
        if (shipData == null) return;

        raceShips[(int)shipData.race].count--;

        for (int i = 0; i <= (int)DataManager.Race.Ori; i++)
        {
            if (DataManager.isAlly(shipData.race, (DataManager.Race)i))
            {
                continue;
            }

            raceShips[i].numberOfEnemies--;
        }

        raceShips[(int)shipData.race].ships.Remove(ship);

        if (isPlayerShipHere())
            updateVisionStatus();
    }

    public bool isPlayerShipHere()
    {
        return raceShips[(int)DataManager.Race.Human].count == 1;
    }

    public DataManager.ForceSize[] getRaceForceSizes()
    {
        DataManager.ForceSize[] result = new DataManager.ForceSize[(int)DataManager.Race.Ori+1];
        for(int i = 0; i <= (int)DataManager.Race.Ori; i++) {
            if (i == (int)DataManager.Race.Human)
            {
                result[i] = 0;
                continue;
            }
            result[i] = DataManager.getForceSizeFromCount(raceShips[i].count);
        }
        return result;
    }

    /// <summary>
    /// Spawn a collection of ships into this sector
    /// </summary>
    /// <param name="ForceSizes">The sizes of </param>
    /// <param name="instant"></param>
    /// <param name="boss"></param>
    public void setupShipCollection(DataManager.ForceSize[] forceSizes, bool instant, bool boss)
    {
        int playerLevel = game.player.ships[game.player.currentShip].shipLevel;
        int minLevel = (playerLevel - 2 < 0) ? 0 : playerLevel - 2;
        int maxLevel = (playerLevel + 2 > 9) ? 9 : playerLevel + 2;

        for (int i = 0; i < 6; i++)
        {
            if(i == (int)DataManager.Race.Human) continue;

            int shipCount = DataManager.getShipCountFromForceSize(forceSizes[i]);

            for(int j = 0; j < shipCount; j++)
                spawnShip(Random.Range(minLevel, maxLevel), getRandomShipForRace((DataManager.Race)i), (DataManager.Race)i, instant);
        }

        if (boss)
        {
            DataManager.Race race = game.systems[systemID].planets[planetID].controller;
            spawnShip(9, getBossShipForRace(race), race, instant);
        }
    }

    public int getRandomShipForRace(DataManager.Race race)
    {
        switch (race)
        {
            case DataManager.Race.Asgard:
                return 3;
            case DataManager.Race.Goauld:
                return Random.Range(4, 5);
            case DataManager.Race.Ori:
                return 11;
            case DataManager.Race.Replicator:
                return 10;
            case DataManager.Race.Wraith:
                return Random.Range(7, 8);
        }

        // should NEVER happen
        return -1;
    }

    public int getBossShipForRace(DataManager.Race race)
    {
        switch (race)
        {
            case DataManager.Race.Asgard:
                return 3;
            case DataManager.Race.Goauld:
                return 6;
            case DataManager.Race.Ori:
                return 11;
            case DataManager.Race.Replicator:
                return 10;
            case DataManager.Race.Wraith:
                return 9;
        }

        // should NEVER happen
        return -1;
    }

    public List<GameObject> findShipsWithInRangeOf(Vector3 pos, float range)
    {
        List<GameObject> result = new List<GameObject>();

        for (int i = 0; i < (int)DataManager.Race.Ori; i++)
        {
            foreach (GameObject s in raceShips[i].ships)
            {
                if (Vector3.Distance(transform.position, pos) <= range)
                {
                    result.Add(s);
                }
            }
        }

        return result;
    }

    public List<GameObject> getTargetableObjects()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerLocation = player.transform.position;
        
        List<GameObject> result = new List<GameObject>();
        List<GameObject> enemies = getEnemies(DataManager.Race.Human);
        foreach (GameObject enemy in enemies)
        {
            result = orderedInsertbyLocation(result, enemy, playerLocation);
        }

        foreach (GameObject ship in wreckedships)
        {
            result = orderedInsertbyLocation(result, ship, playerLocation);
        }

        foreach (GameObject ship in raceShips[(int)DataManager.Race.Asgard].ships)
        {
            result = orderedInsertbyLocation(result, ship, playerLocation);
        }

        //result = orderedInsertbyLocation(result, planet, playerLocation);

        return result;
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

    public void addWreck(GameObject wreck)
    {
        wreckedships.Add(wreck);
    }

    public void updateVisionStatus()
    {
        // update the visibility with the ranking of force size
        // and dominant force
        game.updateStatusPlayerVision(systemID, planetID);
    }
}
