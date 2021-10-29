using UnityEngine;
using System.Collections;

public class DataManager : MonoBehaviour
{

    public enum Race { Human = 0, Asgard = 1, Goauld = 2, Wraith = 3, Replicator = 4, Ori = 5 };
    public enum PlanetStatus { Homeworld, Tradeworld, Unknown, Hostile, Friendly };
    public enum ForceSize { None, Tiny, Small, Medium, Large, Extreme };
    public enum DamageType { Projectile, Energy, Replicator };
    public enum WeaponEffectType { Beam, Bolt, Projectile, Rocket, Replicator };

    public struct PlayerData
    {
        public PlayerShips[] ships;
        public int currentShip;

        public int[] resources;
    }

    public struct PlayerShips
    {
        public int[] shipWeapons;

        public int shipLevel;
        public int shipExp;
        public int shipID;
        public bool owned;

        // saved configuration
        public int shieldPower;
        public int weaponPower;
        public int speedPower;
        public int[] enabledWeapons;
    }

    public struct SaveData
    {
        public SystemData[] systems;
        public PlayerData player;
    }

    public struct SystemData
    {
        public int systemID;
        public PlanetData[] planets;

        public Vector2 mapGridLocation;
        public int[] linkedNodes;
        public bool isNeutralZone;
        public bool hasBoss;

        public int skyboxID;
        public Race owner;
    }

    public struct PlanetData
    {
        public Vector3 location;
        public Vector2 mapLocation;
        public int textureID;

        // actual
        public Race controller;
        public PlanetStatus status;
        public ForceSize[] raceForceSizes;

        // last seen
        public ForceSize lastMaxForceSize;
        public Race lastController;
        public PlanetStatus lastStatus;
    }

    public struct WeaponData
    {
        public string weaponName;
        public int weaponID;
        public int prefabID;
        public int[] resources;

        public float fireRate;
        public int powerValue;
        public int damageValue;
        public DamageType damageType;
        public WeaponEffectType effectType;
    }

    public struct ShipData
    {
        public string shipName;
        public int shipID;
        public int prefabID;
        public Race shipRace;
        public bool isBoss;

        public float turnSpeed;

        public int[] resources;
        public int resourceMultiplier;

        public ShipStats[] AIStats;
    }

    public struct ShipStats
    {
        public int hull;
        public int shield;
        public int[] maxWeapons;
        public int power;
        public float speed;
    }

    public struct ShipDatabase
    {
        public ShipData[] ships;
        public WeaponData[] weapons;
    }

    public static bool isAlly(Race r1, Race r2)
    {
        if (r1 == r2) return true;

        if (r1 != Race.Human && r1 != Race.Asgard) return false;
        if (r2 != Race.Human && r2 != Race.Asgard) return false;

        return true;
    }

    public static ForceSize getForceSizeFromCount(int count)
    {
        if (count == 0)
            return ForceSize.None;
        else if (count <= 2)
            return ForceSize.Tiny;
        else if (count <= 4)
            return ForceSize.Small;
        else if (count <= 7)
            return ForceSize.Medium;
        else if (count <= 12)
            return ForceSize.Large;
        else
            return ForceSize.Extreme;
    }

    public static int getShipCountFromForceSize(ForceSize forceSize)
    {
        switch (forceSize)
        {
            case ForceSize.None:
                return 0;
            case ForceSize.Tiny:
                return Random.Range(1, 2);
            case ForceSize.Small:
                return Random.Range(3, 4);
            case ForceSize.Medium:
                return Random.Range(5, 7);
            case ForceSize.Large:
                return Random.Range(8, 12);
            case ForceSize.Extreme:
                return Random.Range(12, 15);
        }
        return 0;
    }

    public struct Battle
    {
        public Race race;
        public int systemID;
        public int planetID;
        public ForceSize forceSize;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
