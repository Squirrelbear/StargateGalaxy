using UnityEngine;
using System.Collections;

// saving related imports
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Text;

public class DataLoader : MonoBehaviour {

    public enum FileType { ShipSaves=0, PlayerSave = 1, DefaultSave = 2 };

    private string[] _FileNames;
    private string _FileLocation, _FileName;
    private string _data;

	// Use this for initialization
	void Start () {
       /* _FileLocation = Application.dataPath + "\\GameData";
        _FileNames = new string[3];
        _FileNames[(int)FileType.ShipSaves] = "ShipData.xml";
        _FileNames[(int)FileType.PlayerSave] = "PlayerData.xml";
        _FileNames[(int)FileType.DefaultSave] = "PlayerData.xml";

        // handle new game occuring
        GameObject[] newGameFlags = GameObject.FindGameObjectsWithTag("NewGame");

        if (!Directory.Exists(_FileLocation))
            Directory.CreateDirectory(_FileLocation);

        if(!File.Exists(_FileLocation + "\\" + _FileNames[(int)FileType.ShipSaves]))
            generateShipDateBase();

        if (newGameFlags.Length > 0 || !File.Exists(_FileLocation + "\\" + _FileNames[(int)FileType.DefaultSave]))
        {
            
            generateBasicData();
            foreach(GameObject flag in newGameFlags)
                DestroyObject(flag);
        }*/
	}

    // Update is called once per frame
    void Update()
    {
	
	}

    public DataManager.SaveData loadPlayerData(bool defaultSave)
    {
        _FileLocation = Application.dataPath + "\\GameData";
        _FileNames = new string[3];
        _FileNames[(int)FileType.ShipSaves] = "ShipData.xml";
        _FileNames[(int)FileType.PlayerSave] = "PlayerData.xml";
        _FileNames[(int)FileType.DefaultSave] = "PlayerData.xml";

        if (defaultSave)
        {
            _FileName = _FileNames[(int)FileType.DefaultSave];
        }
        else
        {
            _FileName = _FileNames[(int)FileType.PlayerSave];
        }

        if (!Directory.Exists(_FileLocation))
            Directory.CreateDirectory(_FileLocation);

        GameObject[] newGameFlags = GameObject.FindGameObjectsWithTag("NewGame");

        if (newGameFlags.Length > 0 || !File.Exists(_FileLocation + "\\" + _FileNames[(int)FileType.DefaultSave]))
        {

            generateBasicData();
            foreach (GameObject flag in newGameFlags)
                DestroyObject(flag);
        }

        DataManager.SaveData gameData = new DataManager.SaveData();

        // message indicating loading is occuring
      //  GUI.Label(new Rect(10,140,400,40),"Loading from: "+_FileLocation); 
        
        // load the data from file
        LoadXML();

        // deserialize
        gameData = (DataManager.SaveData)DeserializeObject(_data, FileType.PlayerSave); 

        return gameData;
    }

    public DataManager.ShipDatabase loadShipData()
    {
        _FileLocation = Application.dataPath + "\\GameData";
        _FileNames = new string[3];
        _FileNames[(int)FileType.ShipSaves] = "ShipData.xml";
        _FileNames[(int)FileType.PlayerSave] = "PlayerData.xml";
        _FileNames[(int)FileType.PlayerSave] = "PlayerData.xml";

        _FileName = _FileNames[(int)FileType.ShipSaves];

        if (!Directory.Exists(_FileLocation))
            Directory.CreateDirectory(_FileLocation);

        if (!File.Exists(_FileLocation + "\\" + _FileNames[(int)FileType.ShipSaves]))
            generateShipDateBase();

        DataManager.ShipDatabase shipDatabase = new DataManager.ShipDatabase();

        // message indicating loading is occuring
        //GUI.Label(new Rect(10, 140, 400, 40), "Loading from: " + _FileLocation);

        // load the data from file
        LoadXML();

        // deserialize
        shipDatabase = (DataManager.ShipDatabase)DeserializeObject(_data, FileType.ShipSaves);

        return shipDatabase;
    }

    public void saveData(DataManager.SaveData data) 
    {
        _FileName = _FileNames[(int)FileType.PlayerSave];
        _data = SerializeObject(data, FileType.PlayerSave);

        CreateXML();
    }


    public void saveData(DataManager.ShipDatabase data)
    {
        _FileName = _FileNames[(int)FileType.ShipSaves];
        _data = SerializeObject(data, FileType.ShipSaves);

        CreateXML();
        Debug.Log(_data);
    }

    // http://unifycommunity.com/wiki/index.php?title=Save_and_Load_from_XML
     /* The following metods came from the referenced URL */
   string UTF8ByteArrayToString(byte[] characters)
   {     
      UTF8Encoding encoding = new UTF8Encoding();
      string constructedString = encoding.GetString(characters);
      return (constructedString);
   }
   
   byte[] StringToUTF8ByteArray(string pXmlString)
   {
      UTF8Encoding encoding = new UTF8Encoding();
      byte[] byteArray = encoding.GetBytes(pXmlString);
      return byteArray;
   }
   
   // Here we serialize our UserData object of myData
   string SerializeObject(object pObject, FileType type)
   {
      string XmlizedString = null;
      MemoryStream memoryStream = new MemoryStream();
      XmlSerializer xs;
      if (type != FileType.ShipSaves)
      {
          xs = new XmlSerializer(typeof(DataManager.SaveData));
      }
      else
      {
          xs = new XmlSerializer(typeof(DataManager.ShipDatabase));
      }
      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
      xs.Serialize(xmlTextWriter, pObject);
      memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
      XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
      return XmlizedString;
   }
   
   // Here we deserialize it back into its original form
   object DeserializeObject(string pXmlizedString, FileType type)
   {
       XmlSerializer xs;
       if (type != FileType.ShipSaves)
       {
           xs = new XmlSerializer(typeof(DataManager.SaveData));
       }
       else
       {
           xs = new XmlSerializer(typeof(DataManager.ShipDatabase));
       }
      MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
      XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
      return xs.Deserialize(memoryStream);
   }
   
   // Finally our save and load methods for the file itself
   void CreateXML()
   {
      StreamWriter writer;
      FileInfo t = new FileInfo(_FileLocation+"\\"+ _FileName);
      if(!t.Exists)
      {
         writer = t.CreateText();
      }
      else
      {
         t.Delete();
         writer = t.CreateText();
      }
      writer.Write(_data);
      writer.Close();
      Debug.Log("File written.");
   }
   
   void LoadXML()
   {
      StreamReader r = File.OpenText(_FileLocation+"\\"+ _FileName);
      string _info = r.ReadToEnd();
      r.Close();
      _data=_info;
      Debug.Log("File Read");
   }

   private void generateBasicData()
   {
       DataManager.SaveData mydata = new DataManager.SaveData();

       mydata.systems = new DataManager.SystemData[28];


       mydata.systems[0] = new DataManager.SystemData();
       mydata.systems[0].systemID = 0;
       mydata.systems[0].planets = new DataManager.PlanetData[1];

       DataManager.PlanetData P0 = new DataManager.PlanetData(); //Earth
       P0.location = new Vector3(0, 0, 0);
       P0.mapLocation = new Vector2(0, 0);
       P0.textureID = 0;
       P0.status = DataManager.PlanetStatus.Homeworld;
       P0.controller = DataManager.Race.Human;
       P0.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P0.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P0.lastMaxForceSize = DataManager.ForceSize.None;
       P0.lastController = DataManager.Race.Human;
       P0.lastStatus = DataManager.PlanetStatus.Homeworld;
       mydata.systems[0].planets[0] = P0;
       mydata.systems[1] = new DataManager.SystemData();
       mydata.systems[1].systemID = 1;
       mydata.systems[1].isNeutralZone = false;
       mydata.systems[1].hasBoss = false;
       mydata.systems[1].linkedNodes = new int[4];
       mydata.systems[1].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P1 = new DataManager.PlanetData(); //Asgard
       P1.location = new Vector3(4, 2, 20);
       P1.mapLocation = new Vector2(0, 0);
       P1.textureID = 0;
       P1.controller = DataManager.Race.Asgard;
       P1.status = DataManager.PlanetStatus.Friendly;
       P1.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P1.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P1.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P1.lastController = DataManager.Race.Asgard;
       P1.lastStatus = DataManager.PlanetStatus.Friendly;
       P1.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[1].planets[0] = P1;

       DataManager.PlanetData P2 = new DataManager.PlanetData(); //Asgard
       P2.location = new Vector3(16, 1, 19);
       P2.mapLocation = new Vector2(0, 0);
       P2.textureID = 0;
       P2.controller = DataManager.Race.Asgard;
       P2.status = DataManager.PlanetStatus.Friendly;
       P2.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P2.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P2.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P2.lastController = DataManager.Race.Asgard;
       P2.lastStatus = DataManager.PlanetStatus.Friendly;
       P2.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[1].planets[1] = P2;

       DataManager.PlanetData P3 = new DataManager.PlanetData(); //Asgard
       P3.location = new Vector3(18, 2, 0);
       P3.mapLocation = new Vector2(0, 0);
       P3.textureID = 0;
       P3.controller = DataManager.Race.Asgard;
       P3.status = DataManager.PlanetStatus.Friendly;
       P3.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P3.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P3.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P3.lastController = DataManager.Race.Asgard;
       P3.lastStatus = DataManager.PlanetStatus.Friendly;
       P3.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[1].planets[2] = P3;

       DataManager.PlanetData P4 = new DataManager.PlanetData(); //Asgard
       P4.location = new Vector3(11, 1, 15);
       P4.mapLocation = new Vector2(0, 0);
       P4.textureID = 0;
       P4.controller = DataManager.Race.Asgard;
       P4.status = DataManager.PlanetStatus.Friendly;
       P4.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P4.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P4.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P4.lastController = DataManager.Race.Asgard;
       P4.lastStatus = DataManager.PlanetStatus.Friendly;
       P4.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[1].planets[3] = P4;

       DataManager.PlanetData P5 = new DataManager.PlanetData(); //Asgard
       P5.location = new Vector3(12, 1, 12);
       P5.mapLocation = new Vector2(0, 0);
       P5.textureID = 0;
       P5.controller = DataManager.Race.Asgard;
       P5.status = DataManager.PlanetStatus.Friendly;
       P5.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P5.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P5.raceForceSizes[1] = DataManager.ForceSize.Large;
       P5.lastController = DataManager.Race.Asgard;
       P5.lastStatus = DataManager.PlanetStatus.Friendly;
       P5.lastMaxForceSize = DataManager.ForceSize.Large;
       mydata.systems[1].planets[4] = P5;

       mydata.systems[2] = new DataManager.SystemData();
       mydata.systems[2].systemID = 2;
       mydata.systems[2].isNeutralZone = false;
       mydata.systems[2].hasBoss = true;
       mydata.systems[2].linkedNodes = new int[4];
       mydata.systems[2].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P6 = new DataManager.PlanetData(); //Asgard
       P6.location = new Vector3(10, 1, 10);
       P6.mapLocation = new Vector2(0, 0);
       P6.textureID = 0;
       P6.controller = DataManager.Race.Asgard;
       P6.status = DataManager.PlanetStatus.Friendly;
       P6.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P6.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P6.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P6.lastController = DataManager.Race.Asgard;
       P6.lastStatus = DataManager.PlanetStatus.Friendly;
       P6.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[2].planets[0] = P6;

       DataManager.PlanetData P7 = new DataManager.PlanetData(); //Asgard
       P7.location = new Vector3(7, 0, 4);
       P7.mapLocation = new Vector2(0, 0);
       P7.textureID = 0;
       P7.controller = DataManager.Race.Asgard;
       P7.status = DataManager.PlanetStatus.Friendly;
       P7.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P7.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P7.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P7.lastController = DataManager.Race.Asgard;
       P7.lastStatus = DataManager.PlanetStatus.Friendly;
       P7.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[2].planets[1] = P7;

       DataManager.PlanetData P8 = new DataManager.PlanetData(); //Asgard
       P8.location = new Vector3(18, 1, 16);
       P8.mapLocation = new Vector2(0, 0);
       P8.textureID = 0;
       P8.controller = DataManager.Race.Asgard;
       P8.status = DataManager.PlanetStatus.Friendly;
       P8.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P8.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P8.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P8.lastController = DataManager.Race.Asgard;
       P8.lastStatus = DataManager.PlanetStatus.Friendly;
       P8.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[2].planets[2] = P8;

       DataManager.PlanetData P9 = new DataManager.PlanetData(); //Asgard
       P9.location = new Vector3(3, 1, 1);
       P9.mapLocation = new Vector2(0, 0);
       P9.textureID = 0;
       P9.controller = DataManager.Race.Asgard;
       P9.status = DataManager.PlanetStatus.Friendly;
       P9.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P9.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P9.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P9.lastController = DataManager.Race.Asgard;
       P9.lastStatus = DataManager.PlanetStatus.Friendly;
       P9.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[2].planets[3] = P9;

       DataManager.PlanetData P10 = new DataManager.PlanetData(); //Asgard
       P10.location = new Vector3(7, 2, 12);
       P10.mapLocation = new Vector2(0, 0);
       P10.textureID = 0;
       P10.controller = DataManager.Race.Asgard;
       P10.status = DataManager.PlanetStatus.Friendly;
       P10.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P10.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P10.raceForceSizes[1] = DataManager.ForceSize.Large;
       P10.lastController = DataManager.Race.Asgard;
       P10.lastStatus = DataManager.PlanetStatus.Friendly;
       P10.lastMaxForceSize = DataManager.ForceSize.Large;
       mydata.systems[2].planets[4] = P10;

       mydata.systems[3] = new DataManager.SystemData();
       mydata.systems[3].systemID = 3;
       mydata.systems[3].isNeutralZone = false;
       mydata.systems[3].hasBoss = false;
       mydata.systems[3].linkedNodes = new int[4];
       mydata.systems[3].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P11 = new DataManager.PlanetData(); //Asgard
       P11.location = new Vector3(17, 0, 8);
       P11.mapLocation = new Vector2(0, 0);
       P11.textureID = 0;
       P11.controller = DataManager.Race.Asgard;
       P11.status = DataManager.PlanetStatus.Friendly;
       P11.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P11.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P11.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P11.lastController = DataManager.Race.Asgard;
       P11.lastStatus = DataManager.PlanetStatus.Friendly;
       P11.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[3].planets[0] = P11;

       DataManager.PlanetData P12 = new DataManager.PlanetData(); //Asgard
       P12.location = new Vector3(15, 2, 1);
       P12.mapLocation = new Vector2(0, 0);
       P12.textureID = 0;
       P12.controller = DataManager.Race.Asgard;
       P12.status = DataManager.PlanetStatus.Friendly;
       P12.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P12.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P12.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P12.lastController = DataManager.Race.Asgard;
       P12.lastStatus = DataManager.PlanetStatus.Friendly;
       P12.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[3].planets[1] = P12;

       DataManager.PlanetData P13 = new DataManager.PlanetData(); //Asgard
       P13.location = new Vector3(19, 1, 18);
       P13.mapLocation = new Vector2(0, 0);
       P13.textureID = 0;
       P13.controller = DataManager.Race.Asgard;
       P13.status = DataManager.PlanetStatus.Friendly;
       P13.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P13.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P13.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P13.lastController = DataManager.Race.Asgard;
       P13.lastStatus = DataManager.PlanetStatus.Friendly;
       P13.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[3].planets[2] = P13;

       DataManager.PlanetData P14 = new DataManager.PlanetData(); //Asgard
       P14.location = new Vector3(6, 2, 7);
       P14.mapLocation = new Vector2(0, 0);
       P14.textureID = 0;
       P14.controller = DataManager.Race.Asgard;
       P14.status = DataManager.PlanetStatus.Friendly;
       P14.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P14.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P14.raceForceSizes[1] = DataManager.ForceSize.Medium;
       P14.lastController = DataManager.Race.Asgard;
       P14.lastStatus = DataManager.PlanetStatus.Friendly;
       P14.lastMaxForceSize = DataManager.ForceSize.Medium;
       mydata.systems[3].planets[3] = P14;

       DataManager.PlanetData P15 = new DataManager.PlanetData(); //Asgard
       P15.location = new Vector3(18, 0, 0);
       P15.mapLocation = new Vector2(0, 0);
       P15.textureID = 0;
       P15.controller = DataManager.Race.Asgard;
       P15.status = DataManager.PlanetStatus.Tradeworld;
       P15.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P15.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P15.raceForceSizes[1] = DataManager.ForceSize.Large;
       P15.lastController = DataManager.Race.Asgard;
       P15.lastStatus = DataManager.PlanetStatus.Friendly;
       P15.lastMaxForceSize = DataManager.ForceSize.Large;
       mydata.systems[3].planets[4] = P15;

       mydata.systems[4] = new DataManager.SystemData();
       mydata.systems[4].systemID = 4;
       mydata.systems[4].isNeutralZone = false;
       mydata.systems[4].hasBoss = false;
       mydata.systems[4].linkedNodes = new int[4];
       mydata.systems[4].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P16 = new DataManager.PlanetData(); //Goauld
       P16.location = new Vector3(0, 0, 18);
       P16.mapLocation = new Vector2(0, 0);
       P16.textureID = 0;
       P16.controller = DataManager.Race.Goauld;
       P16.status = DataManager.PlanetStatus.Hostile;
       P16.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P16.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P16.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P16.lastController = DataManager.Race.Asgard;
       P16.lastMaxForceSize = DataManager.ForceSize.None;
       P16.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[4].planets[0] = P16;

       DataManager.PlanetData P17 = new DataManager.PlanetData(); //Goauld
       P17.location = new Vector3(9, 2, 19);
       P17.mapLocation = new Vector2(0, 0);
       P17.textureID = 0;
       P17.controller = DataManager.Race.Goauld;
       P17.status = DataManager.PlanetStatus.Hostile;
       P17.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P17.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P17.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P17.lastController = DataManager.Race.Asgard;
       P17.lastMaxForceSize = DataManager.ForceSize.None;
       P17.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[4].planets[1] = P17;

       DataManager.PlanetData P18 = new DataManager.PlanetData(); //Goauld
       P18.location = new Vector3(1, 1, 6);
       P18.mapLocation = new Vector2(0, 0);
       P18.textureID = 0;
       P18.controller = DataManager.Race.Goauld;
       P18.status = DataManager.PlanetStatus.Hostile;
       P18.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P18.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P18.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P18.lastController = DataManager.Race.Asgard;
       P18.lastMaxForceSize = DataManager.ForceSize.None;
       P18.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[4].planets[2] = P18;

       DataManager.PlanetData P19 = new DataManager.PlanetData(); //Goauld
       P19.location = new Vector3(3, 1, 19);
       P19.mapLocation = new Vector2(0, 0);
       P19.textureID = 0;
       P19.controller = DataManager.Race.Goauld;
       P19.status = DataManager.PlanetStatus.Hostile;
       P19.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P19.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P19.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P19.lastController = DataManager.Race.Asgard;
       P19.lastMaxForceSize = DataManager.ForceSize.None;
       P19.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[4].planets[3] = P19;

       DataManager.PlanetData P20 = new DataManager.PlanetData(); //Goauld
       P20.location = new Vector3(18, 1, 6);
       P20.mapLocation = new Vector2(0, 0);
       P20.textureID = 0;
       P20.controller = DataManager.Race.Goauld;
       P20.status = DataManager.PlanetStatus.Hostile;
       P20.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P20.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P20.raceForceSizes[2] = DataManager.ForceSize.Large;
       P20.lastController = DataManager.Race.Asgard;
       P20.lastMaxForceSize = DataManager.ForceSize.None;
       P20.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[4].planets[4] = P20;

       mydata.systems[5] = new DataManager.SystemData();
       mydata.systems[5].systemID = 5;
       mydata.systems[5].isNeutralZone = false;
       mydata.systems[5].hasBoss = false;
       mydata.systems[5].linkedNodes = new int[4];
       mydata.systems[5].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P21 = new DataManager.PlanetData(); //Goauld
       P21.location = new Vector3(9, 2, 8);
       P21.mapLocation = new Vector2(0, 0);
       P21.textureID = 0;
       P21.controller = DataManager.Race.Goauld;
       P21.status = DataManager.PlanetStatus.Hostile;
       P21.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P21.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P21.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P21.lastController = DataManager.Race.Asgard;
       P21.lastMaxForceSize = DataManager.ForceSize.None;
       P21.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[5].planets[0] = P21;

       DataManager.PlanetData P22 = new DataManager.PlanetData(); //Goauld
       P22.location = new Vector3(6, 2, 16);
       P22.mapLocation = new Vector2(0, 0);
       P22.textureID = 0;
       P22.controller = DataManager.Race.Goauld;
       P22.status = DataManager.PlanetStatus.Hostile;
       P22.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P22.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P22.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P22.lastController = DataManager.Race.Asgard;
       P22.lastMaxForceSize = DataManager.ForceSize.None;
       P22.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[5].planets[1] = P22;

       DataManager.PlanetData P23 = new DataManager.PlanetData(); //Goauld
       P23.location = new Vector3(11, 1, 15);
       P23.mapLocation = new Vector2(0, 0);
       P23.textureID = 0;
       P23.controller = DataManager.Race.Goauld;
       P23.status = DataManager.PlanetStatus.Hostile;
       P23.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P23.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P23.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P23.lastController = DataManager.Race.Asgard;
       P23.lastMaxForceSize = DataManager.ForceSize.None;
       P23.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[5].planets[2] = P23;

       DataManager.PlanetData P24 = new DataManager.PlanetData(); //Goauld
       P24.location = new Vector3(12, 1, 5);
       P24.mapLocation = new Vector2(0, 0);
       P24.textureID = 0;
       P24.controller = DataManager.Race.Goauld;
       P24.status = DataManager.PlanetStatus.Hostile;
       P24.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P24.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P24.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P24.lastController = DataManager.Race.Asgard;
       P24.lastMaxForceSize = DataManager.ForceSize.None;
       P24.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[5].planets[3] = P24;

       DataManager.PlanetData P25 = new DataManager.PlanetData(); //Goauld
       P25.location = new Vector3(8, 1, 14);
       P25.mapLocation = new Vector2(0, 0);
       P25.textureID = 0;
       P25.controller = DataManager.Race.Goauld;
       P25.status = DataManager.PlanetStatus.Hostile;
       P25.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P25.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P25.raceForceSizes[2] = DataManager.ForceSize.Large;
       P25.lastController = DataManager.Race.Asgard;
       P25.lastMaxForceSize = DataManager.ForceSize.None;
       P25.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[5].planets[4] = P25;

       mydata.systems[6] = new DataManager.SystemData();
       mydata.systems[6].systemID = 6;
       mydata.systems[6].isNeutralZone = false;
       mydata.systems[6].hasBoss = true;
       mydata.systems[6].linkedNodes = new int[4];
       mydata.systems[6].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P26 = new DataManager.PlanetData(); //Goauld
       P26.location = new Vector3(9, 0, 5);
       P26.mapLocation = new Vector2(0, 0);
       P26.textureID = 0;
       P26.controller = DataManager.Race.Goauld;
       P26.status = DataManager.PlanetStatus.Hostile;
       P26.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P26.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P26.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P26.lastController = DataManager.Race.Asgard;
       P26.lastMaxForceSize = DataManager.ForceSize.None;
       P26.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[6].planets[0] = P26;

       DataManager.PlanetData P27 = new DataManager.PlanetData(); //Goauld
       P27.location = new Vector3(0, 1, 10);
       P27.mapLocation = new Vector2(0, 0);
       P27.textureID = 0;
       P27.controller = DataManager.Race.Goauld;
       P27.status = DataManager.PlanetStatus.Hostile;
       P27.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P27.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P27.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P27.lastController = DataManager.Race.Asgard;
       P27.lastMaxForceSize = DataManager.ForceSize.None;
       P27.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[6].planets[1] = P27;

       DataManager.PlanetData P28 = new DataManager.PlanetData(); //Goauld
       P28.location = new Vector3(14, 1, 5);
       P28.mapLocation = new Vector2(0, 0);
       P28.textureID = 0;
       P28.controller = DataManager.Race.Goauld;
       P28.status = DataManager.PlanetStatus.Hostile;
       P28.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P28.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P28.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P28.lastController = DataManager.Race.Asgard;
       P28.lastMaxForceSize = DataManager.ForceSize.None;
       P28.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[6].planets[2] = P28;

       DataManager.PlanetData P29 = new DataManager.PlanetData(); //Goauld
       P29.location = new Vector3(9, 2, 10);
       P29.mapLocation = new Vector2(0, 0);
       P29.textureID = 0;
       P29.controller = DataManager.Race.Goauld;
       P29.status = DataManager.PlanetStatus.Hostile;
       P29.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P29.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P29.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P29.lastController = DataManager.Race.Asgard;
       P29.lastMaxForceSize = DataManager.ForceSize.None;
       P29.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[6].planets[3] = P29;

       DataManager.PlanetData P30 = new DataManager.PlanetData(); //Goauld
       P30.location = new Vector3(15, 2, 20);
       P30.mapLocation = new Vector2(0, 0);
       P30.textureID = 0;
       P30.controller = DataManager.Race.Goauld;
       P30.status = DataManager.PlanetStatus.Hostile;
       P30.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P30.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P30.raceForceSizes[2] = DataManager.ForceSize.Large;
       P30.lastController = DataManager.Race.Asgard;
       P30.lastMaxForceSize = DataManager.ForceSize.None;
       P30.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[6].planets[4] = P30;

       mydata.systems[7] = new DataManager.SystemData();
       mydata.systems[7].systemID = 7;
       mydata.systems[7].isNeutralZone = false;
       mydata.systems[7].hasBoss = false;
       mydata.systems[7].linkedNodes = new int[4];
       mydata.systems[7].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P31 = new DataManager.PlanetData(); //Goauld
       P31.location = new Vector3(20, 0, 4);
       P31.mapLocation = new Vector2(0, 0);
       P31.textureID = 0;
       P31.controller = DataManager.Race.Goauld;
       P31.status = DataManager.PlanetStatus.Hostile;
       P31.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P31.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P31.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P31.lastController = DataManager.Race.Asgard;
       P31.lastMaxForceSize = DataManager.ForceSize.None;
       P31.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[7].planets[0] = P31;

       DataManager.PlanetData P32 = new DataManager.PlanetData(); //Goauld
       P32.location = new Vector3(10, 2, 2);
       P32.mapLocation = new Vector2(0, 0);
       P32.textureID = 0;
       P32.controller = DataManager.Race.Goauld;
       P32.status = DataManager.PlanetStatus.Hostile;
       P32.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P32.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P32.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P32.lastController = DataManager.Race.Asgard;
       P32.lastMaxForceSize = DataManager.ForceSize.None;
       P32.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[7].planets[1] = P32;

       DataManager.PlanetData P33 = new DataManager.PlanetData(); //Goauld
       P33.location = new Vector3(19, 1, 7);
       P33.mapLocation = new Vector2(0, 0);
       P33.textureID = 0;
       P33.controller = DataManager.Race.Goauld;
       P33.status = DataManager.PlanetStatus.Hostile;
       P33.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P33.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P33.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P33.lastController = DataManager.Race.Asgard;
       P33.lastMaxForceSize = DataManager.ForceSize.None;
       P33.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[7].planets[2] = P33;

       DataManager.PlanetData P34 = new DataManager.PlanetData(); //Goauld
       P34.location = new Vector3(4, 1, 12);
       P34.mapLocation = new Vector2(0, 0);
       P34.textureID = 0;
       P34.controller = DataManager.Race.Goauld;
       P34.status = DataManager.PlanetStatus.Hostile;
       P34.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P34.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P34.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P34.lastController = DataManager.Race.Asgard;
       P34.lastMaxForceSize = DataManager.ForceSize.None;
       P34.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[7].planets[3] = P34;

       DataManager.PlanetData P35 = new DataManager.PlanetData(); //Goauld
       P35.location = new Vector3(2, 2, 6);
       P35.mapLocation = new Vector2(0, 0);
       P35.textureID = 0;
       P35.controller = DataManager.Race.Goauld;
       P35.status = DataManager.PlanetStatus.Hostile;
       P35.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P35.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P35.raceForceSizes[2] = DataManager.ForceSize.Large;
       P35.lastController = DataManager.Race.Asgard;
       P35.lastMaxForceSize = DataManager.ForceSize.None;
       P35.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[7].planets[4] = P35;

       mydata.systems[8] = new DataManager.SystemData();
       mydata.systems[8].systemID = 8;
       mydata.systems[8].isNeutralZone = false;
       mydata.systems[8].hasBoss = false;
       mydata.systems[8].linkedNodes = new int[4];
       mydata.systems[8].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P36 = new DataManager.PlanetData(); //Goauld
       P36.location = new Vector3(14, 1, 16);
       P36.mapLocation = new Vector2(0, 0);
       P36.textureID = 0;
       P36.controller = DataManager.Race.Goauld;
       P36.status = DataManager.PlanetStatus.Hostile;
       P36.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P36.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P36.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P36.lastController = DataManager.Race.Asgard;
       P36.lastMaxForceSize = DataManager.ForceSize.None;
       P36.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[8].planets[0] = P36;

       DataManager.PlanetData P37 = new DataManager.PlanetData(); //Goauld
       P37.location = new Vector3(12, 2, 7);
       P37.mapLocation = new Vector2(0, 0);
       P37.textureID = 0;
       P37.controller = DataManager.Race.Goauld;
       P37.status = DataManager.PlanetStatus.Hostile;
       P37.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P37.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P37.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P37.lastController = DataManager.Race.Asgard;
       P37.lastMaxForceSize = DataManager.ForceSize.None;
       P37.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[8].planets[1] = P37;

       DataManager.PlanetData P38 = new DataManager.PlanetData(); //Goauld
       P38.location = new Vector3(14, 2, 4);
       P38.mapLocation = new Vector2(0, 0);
       P38.textureID = 0;
       P38.controller = DataManager.Race.Goauld;
       P38.status = DataManager.PlanetStatus.Hostile;
       P38.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P38.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P38.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P38.lastController = DataManager.Race.Asgard;
       P38.lastMaxForceSize = DataManager.ForceSize.None;
       P38.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[8].planets[2] = P38;

       DataManager.PlanetData P39 = new DataManager.PlanetData(); //Goauld
       P39.location = new Vector3(17, 2, 7);
       P39.mapLocation = new Vector2(0, 0);
       P39.textureID = 0;
       P39.controller = DataManager.Race.Goauld;
       P39.status = DataManager.PlanetStatus.Hostile;
       P39.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P39.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P39.raceForceSizes[2] = DataManager.ForceSize.Medium;
       P39.lastController = DataManager.Race.Asgard;
       P39.lastMaxForceSize = DataManager.ForceSize.None;
       P39.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[8].planets[3] = P39;

       DataManager.PlanetData P40 = new DataManager.PlanetData(); //Goauld
       P40.location = new Vector3(2, 2, 4);
       P40.mapLocation = new Vector2(0, 0);
       P40.textureID = 0;
       P40.controller = DataManager.Race.Goauld;
       P40.status = DataManager.PlanetStatus.Hostile;
       P40.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P40.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P40.raceForceSizes[2] = DataManager.ForceSize.Large;
       P40.lastController = DataManager.Race.Asgard;
       P40.lastMaxForceSize = DataManager.ForceSize.None;
       P40.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[8].planets[4] = P40;

       mydata.systems[9] = new DataManager.SystemData();
       mydata.systems[9].systemID = 9;
       mydata.systems[9].isNeutralZone = false;
       mydata.systems[9].hasBoss = false;
       mydata.systems[9].linkedNodes = new int[4];
       mydata.systems[9].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P41 = new DataManager.PlanetData(); //Wraith
       P41.location = new Vector3(6, 2, 17);
       P41.mapLocation = new Vector2(0, 0);
       P41.textureID = 0;
       P41.controller = DataManager.Race.Wraith;
       P41.status = DataManager.PlanetStatus.Hostile;
       P41.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P41.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P41.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P41.lastController = DataManager.Race.Asgard;
       P41.lastMaxForceSize = DataManager.ForceSize.None;
       P41.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[9].planets[0] = P41;

       DataManager.PlanetData P42 = new DataManager.PlanetData(); //Wraith
       P42.location = new Vector3(15, 1, 10);
       P42.mapLocation = new Vector2(0, 0);
       P42.textureID = 0;
       P42.controller = DataManager.Race.Wraith;
       P42.status = DataManager.PlanetStatus.Hostile;
       P42.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P42.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P42.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P42.lastController = DataManager.Race.Asgard;
       P42.lastMaxForceSize = DataManager.ForceSize.None;
       P42.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[9].planets[1] = P42;

       DataManager.PlanetData P43 = new DataManager.PlanetData(); //Wraith
       P43.location = new Vector3(0, 2, 14);
       P43.mapLocation = new Vector2(0, 0);
       P43.textureID = 0;
       P43.controller = DataManager.Race.Wraith;
       P43.status = DataManager.PlanetStatus.Hostile;
       P43.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P43.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P43.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P43.lastController = DataManager.Race.Asgard;
       P43.lastMaxForceSize = DataManager.ForceSize.None;
       P43.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[9].planets[2] = P43;

       DataManager.PlanetData P44 = new DataManager.PlanetData(); //Wraith
       P44.location = new Vector3(12, 1, 15);
       P44.mapLocation = new Vector2(0, 0);
       P44.textureID = 0;
       P44.controller = DataManager.Race.Wraith;
       P44.status = DataManager.PlanetStatus.Hostile;
       P44.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P44.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P44.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P44.lastController = DataManager.Race.Asgard;
       P44.lastMaxForceSize = DataManager.ForceSize.None;
       P44.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[9].planets[3] = P44;

       DataManager.PlanetData P45 = new DataManager.PlanetData(); //Wraith
       P45.location = new Vector3(19, 2, 18);
       P45.mapLocation = new Vector2(0, 0);
       P45.textureID = 0;
       P45.controller = DataManager.Race.Wraith;
       P45.status = DataManager.PlanetStatus.Hostile;
       P45.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P45.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P45.raceForceSizes[3] = DataManager.ForceSize.Large;
       P45.lastController = DataManager.Race.Asgard;
       P45.lastMaxForceSize = DataManager.ForceSize.None;
       P45.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[9].planets[4] = P45;

       mydata.systems[10] = new DataManager.SystemData();
       mydata.systems[10].systemID = 10;
       mydata.systems[10].isNeutralZone = false;
       mydata.systems[10].hasBoss = false;
       mydata.systems[10].linkedNodes = new int[4];
       mydata.systems[10].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P46 = new DataManager.PlanetData(); //Wraith
       P46.location = new Vector3(18, 1, 10);
       P46.mapLocation = new Vector2(0, 0);
       P46.textureID = 0;
       P46.controller = DataManager.Race.Wraith;
       P46.status = DataManager.PlanetStatus.Hostile;
       P46.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P46.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P46.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P46.lastController = DataManager.Race.Asgard;
       P46.lastMaxForceSize = DataManager.ForceSize.None;
       P46.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[10].planets[0] = P46;

       DataManager.PlanetData P47 = new DataManager.PlanetData(); //Wraith
       P47.location = new Vector3(20, 2, 0);
       P47.mapLocation = new Vector2(0, 0);
       P47.textureID = 0;
       P47.controller = DataManager.Race.Wraith;
       P47.status = DataManager.PlanetStatus.Hostile;
       P47.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P47.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P47.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P47.lastController = DataManager.Race.Asgard;
       P47.lastMaxForceSize = DataManager.ForceSize.None;
       P47.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[10].planets[1] = P47;

       DataManager.PlanetData P48 = new DataManager.PlanetData(); //Wraith
       P48.location = new Vector3(14, 0, 20);
       P48.mapLocation = new Vector2(0, 0);
       P48.textureID = 0;
       P48.controller = DataManager.Race.Wraith;
       P48.status = DataManager.PlanetStatus.Hostile;
       P48.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P48.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P48.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P48.lastController = DataManager.Race.Asgard;
       P48.lastMaxForceSize = DataManager.ForceSize.None;
       P48.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[10].planets[2] = P48;

       DataManager.PlanetData P49 = new DataManager.PlanetData(); //Wraith
       P49.location = new Vector3(13, 0, 3);
       P49.mapLocation = new Vector2(0, 0);
       P49.textureID = 0;
       P49.controller = DataManager.Race.Wraith;
       P49.status = DataManager.PlanetStatus.Hostile;
       P49.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P49.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P49.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P49.lastController = DataManager.Race.Asgard;
       P49.lastMaxForceSize = DataManager.ForceSize.None;
       P49.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[10].planets[3] = P49;

       DataManager.PlanetData P50 = new DataManager.PlanetData(); //Wraith
       P50.location = new Vector3(0, 2, 2);
       P50.mapLocation = new Vector2(0, 0);
       P50.textureID = 0;
       P50.controller = DataManager.Race.Wraith;
       P50.status = DataManager.PlanetStatus.Hostile;
       P50.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P50.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P50.raceForceSizes[3] = DataManager.ForceSize.Large;
       P50.lastController = DataManager.Race.Asgard;
       P50.lastMaxForceSize = DataManager.ForceSize.None;
       P50.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[10].planets[4] = P50;

       mydata.systems[11] = new DataManager.SystemData();
       mydata.systems[11].systemID = 11;
       mydata.systems[11].isNeutralZone = false;
       mydata.systems[11].hasBoss = false;
       mydata.systems[11].linkedNodes = new int[4];
       mydata.systems[11].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P51 = new DataManager.PlanetData(); //Wraith
       P51.location = new Vector3(15, 2, 13);
       P51.mapLocation = new Vector2(0, 0);
       P51.textureID = 0;
       P51.controller = DataManager.Race.Wraith;
       P51.status = DataManager.PlanetStatus.Hostile;
       P51.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P51.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P51.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P51.lastController = DataManager.Race.Asgard;
       P51.lastMaxForceSize = DataManager.ForceSize.None;
       P51.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[11].planets[0] = P51;

       DataManager.PlanetData P52 = new DataManager.PlanetData(); //Wraith
       P52.location = new Vector3(3, 2, 13);
       P52.mapLocation = new Vector2(0, 0);
       P52.textureID = 0;
       P52.controller = DataManager.Race.Wraith;
       P52.status = DataManager.PlanetStatus.Hostile;
       P52.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P52.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P52.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P52.lastController = DataManager.Race.Asgard;
       P52.lastMaxForceSize = DataManager.ForceSize.None;
       P52.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[11].planets[1] = P52;

       DataManager.PlanetData P53 = new DataManager.PlanetData(); //Wraith
       P53.location = new Vector3(16, 1, 4);
       P53.mapLocation = new Vector2(0, 0);
       P53.textureID = 0;
       P53.controller = DataManager.Race.Wraith;
       P53.status = DataManager.PlanetStatus.Hostile;
       P53.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P53.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P53.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P53.lastController = DataManager.Race.Asgard;
       P53.lastMaxForceSize = DataManager.ForceSize.None;
       P53.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[11].planets[2] = P53;

       DataManager.PlanetData P54 = new DataManager.PlanetData(); //Wraith
       P54.location = new Vector3(17, 1, 15);
       P54.mapLocation = new Vector2(0, 0);
       P54.textureID = 0;
       P54.controller = DataManager.Race.Wraith;
       P54.status = DataManager.PlanetStatus.Hostile;
       P54.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P54.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P54.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P54.lastController = DataManager.Race.Asgard;
       P54.lastMaxForceSize = DataManager.ForceSize.None;
       P54.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[11].planets[3] = P54;

       DataManager.PlanetData P55 = new DataManager.PlanetData(); //Wraith
       P55.location = new Vector3(1, 2, 4);
       P55.mapLocation = new Vector2(0, 0);
       P55.textureID = 0;
       P55.controller = DataManager.Race.Wraith;
       P55.status = DataManager.PlanetStatus.Hostile;
       P55.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P55.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P55.raceForceSizes[3] = DataManager.ForceSize.Large;
       P55.lastController = DataManager.Race.Asgard;
       P55.lastMaxForceSize = DataManager.ForceSize.None;
       P55.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[11].planets[4] = P55;

       mydata.systems[12] = new DataManager.SystemData();
       mydata.systems[12].systemID = 12;
       mydata.systems[12].isNeutralZone = false;
       mydata.systems[12].hasBoss = true;
       mydata.systems[12].linkedNodes = new int[4];
       mydata.systems[12].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P56 = new DataManager.PlanetData(); //Wraith
       P56.location = new Vector3(13, 2, 0);
       P56.mapLocation = new Vector2(0, 0);
       P56.textureID = 0;
       P56.controller = DataManager.Race.Wraith;
       P56.status = DataManager.PlanetStatus.Hostile;
       P56.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P56.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P56.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P56.lastController = DataManager.Race.Asgard;
       P56.lastMaxForceSize = DataManager.ForceSize.None;
       P56.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[12].planets[0] = P56;

       DataManager.PlanetData P57 = new DataManager.PlanetData(); //Wraith
       P57.location = new Vector3(6, 2, 4);
       P57.mapLocation = new Vector2(0, 0);
       P57.textureID = 0;
       P57.controller = DataManager.Race.Wraith;
       P57.status = DataManager.PlanetStatus.Hostile;
       P57.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P57.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P57.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P57.lastController = DataManager.Race.Asgard;
       P57.lastMaxForceSize = DataManager.ForceSize.None;
       P57.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[12].planets[1] = P57;

       DataManager.PlanetData P58 = new DataManager.PlanetData(); //Wraith
       P58.location = new Vector3(18, 0, 2);
       P58.mapLocation = new Vector2(0, 0);
       P58.textureID = 0;
       P58.controller = DataManager.Race.Wraith;
       P58.status = DataManager.PlanetStatus.Hostile;
       P58.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P58.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P58.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P58.lastController = DataManager.Race.Asgard;
       P58.lastMaxForceSize = DataManager.ForceSize.None;
       P58.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[12].planets[2] = P58;

       DataManager.PlanetData P59 = new DataManager.PlanetData(); //Wraith
       P59.location = new Vector3(3, 1, 13);
       P59.mapLocation = new Vector2(0, 0);
       P59.textureID = 0;
       P59.controller = DataManager.Race.Wraith;
       P59.status = DataManager.PlanetStatus.Hostile;
       P59.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P59.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P59.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P59.lastController = DataManager.Race.Asgard;
       P59.lastMaxForceSize = DataManager.ForceSize.None;
       P59.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[12].planets[3] = P59;

       DataManager.PlanetData P60 = new DataManager.PlanetData(); //Wraith
       P60.location = new Vector3(17, 0, 7);
       P60.mapLocation = new Vector2(0, 0);
       P60.textureID = 0;
       P60.controller = DataManager.Race.Wraith;
       P60.status = DataManager.PlanetStatus.Hostile;
       P60.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P60.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P60.raceForceSizes[3] = DataManager.ForceSize.Large;
       P60.lastController = DataManager.Race.Asgard;
       P60.lastMaxForceSize = DataManager.ForceSize.None;
       P60.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[12].planets[4] = P60;

       mydata.systems[13] = new DataManager.SystemData();
       mydata.systems[13].systemID = 13;
       mydata.systems[13].isNeutralZone = false;
       mydata.systems[13].hasBoss = false;
       mydata.systems[13].linkedNodes = new int[4];
       mydata.systems[13].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P61 = new DataManager.PlanetData(); //Wraith
       P61.location = new Vector3(7, 0, 4);
       P61.mapLocation = new Vector2(0, 0);
       P61.textureID = 0;
       P61.controller = DataManager.Race.Wraith;
       P61.status = DataManager.PlanetStatus.Hostile;
       P61.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P61.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P61.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P61.lastController = DataManager.Race.Asgard;
       P61.lastMaxForceSize = DataManager.ForceSize.None;
       P61.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[13].planets[0] = P61;

       DataManager.PlanetData P62 = new DataManager.PlanetData(); //Wraith
       P62.location = new Vector3(12, 2, 14);
       P62.mapLocation = new Vector2(0, 0);
       P62.textureID = 0;
       P62.controller = DataManager.Race.Wraith;
       P62.status = DataManager.PlanetStatus.Hostile;
       P62.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P62.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P62.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P62.lastController = DataManager.Race.Asgard;
       P62.lastMaxForceSize = DataManager.ForceSize.None;
       P62.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[13].planets[1] = P62;

       DataManager.PlanetData P63 = new DataManager.PlanetData(); //Wraith
       P63.location = new Vector3(2, 2, 18);
       P63.mapLocation = new Vector2(0, 0);
       P63.textureID = 0;
       P63.controller = DataManager.Race.Wraith;
       P63.status = DataManager.PlanetStatus.Hostile;
       P63.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P63.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P63.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P63.lastController = DataManager.Race.Asgard;
       P63.lastMaxForceSize = DataManager.ForceSize.None;
       P63.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[13].planets[2] = P63;

       DataManager.PlanetData P64 = new DataManager.PlanetData(); //Wraith
       P64.location = new Vector3(4, 0, 3);
       P64.mapLocation = new Vector2(0, 0);
       P64.textureID = 0;
       P64.controller = DataManager.Race.Wraith;
       P64.status = DataManager.PlanetStatus.Hostile;
       P64.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P64.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P64.raceForceSizes[3] = DataManager.ForceSize.Medium;
       P64.lastController = DataManager.Race.Asgard;
       P64.lastMaxForceSize = DataManager.ForceSize.None;
       P64.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[13].planets[3] = P64;

       DataManager.PlanetData P65 = new DataManager.PlanetData(); //Wraith
       P65.location = new Vector3(15, 0, 11);
       P65.mapLocation = new Vector2(0, 0);
       P65.textureID = 0;
       P65.controller = DataManager.Race.Wraith;
       P65.status = DataManager.PlanetStatus.Hostile;
       P65.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P65.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P65.raceForceSizes[3] = DataManager.ForceSize.Large;
       P65.lastController = DataManager.Race.Asgard;
       P65.lastMaxForceSize = DataManager.ForceSize.None;
       P65.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[13].planets[4] = P65;

       mydata.systems[14] = new DataManager.SystemData();
       mydata.systems[14].systemID = 14;
       mydata.systems[14].isNeutralZone = false;
       mydata.systems[14].hasBoss = true;
       mydata.systems[14].linkedNodes = new int[4];
       mydata.systems[14].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P66 = new DataManager.PlanetData(); //Replicator
       P66.location = new Vector3(2, 2, 15);
       P66.mapLocation = new Vector2(0, 0);
       P66.textureID = 0;
       P66.controller = DataManager.Race.Replicator;
       P66.status = DataManager.PlanetStatus.Hostile;
       P66.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P66.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P66.raceForceSizes[4] = DataManager.ForceSize.Medium;
       P66.lastController = DataManager.Race.Asgard;
       P66.lastMaxForceSize = DataManager.ForceSize.None;
       P66.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[14].planets[0] = P66;

       DataManager.PlanetData P67 = new DataManager.PlanetData(); //Replicator
       P67.location = new Vector3(19, 2, 13);
       P67.mapLocation = new Vector2(0, 0);
       P67.textureID = 0;
       P67.controller = DataManager.Race.Replicator;
       P67.status = DataManager.PlanetStatus.Hostile;
       P67.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P67.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P67.raceForceSizes[4] = DataManager.ForceSize.Medium;
       P67.lastController = DataManager.Race.Asgard;
       P67.lastMaxForceSize = DataManager.ForceSize.None;
       P67.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[14].planets[1] = P67;

       DataManager.PlanetData P68 = new DataManager.PlanetData(); //Replicator
       P68.location = new Vector3(10, 2, 7);
       P68.mapLocation = new Vector2(0, 0);
       P68.textureID = 0;
       P68.controller = DataManager.Race.Replicator;
       P68.status = DataManager.PlanetStatus.Hostile;
       P68.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P68.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P68.raceForceSizes[4] = DataManager.ForceSize.Medium;
       P68.lastController = DataManager.Race.Asgard;
       P68.lastMaxForceSize = DataManager.ForceSize.None;
       P68.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[14].planets[2] = P68;

       DataManager.PlanetData P69 = new DataManager.PlanetData(); //Replicator
       P69.location = new Vector3(0, 2, 5);
       P69.mapLocation = new Vector2(0, 0);
       P69.textureID = 0;
       P69.controller = DataManager.Race.Replicator;
       P69.status = DataManager.PlanetStatus.Hostile;
       P69.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P69.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P69.raceForceSizes[4] = DataManager.ForceSize.Medium;
       P69.lastController = DataManager.Race.Asgard;
       P69.lastMaxForceSize = DataManager.ForceSize.None;
       P69.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[14].planets[3] = P69;

       DataManager.PlanetData P70 = new DataManager.PlanetData(); //Replicator
       P70.location = new Vector3(11, 1, 2);
       P70.mapLocation = new Vector2(0, 0);
       P70.textureID = 0;
       P70.controller = DataManager.Race.Replicator;
       P70.status = DataManager.PlanetStatus.Hostile;
       P70.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P70.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P70.raceForceSizes[4] = DataManager.ForceSize.Large;
       P70.lastController = DataManager.Race.Asgard;
       P70.lastMaxForceSize = DataManager.ForceSize.None;
       P70.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[14].planets[4] = P70;

       mydata.systems[15] = new DataManager.SystemData();
       mydata.systems[15].systemID = 15;
       mydata.systems[15].isNeutralZone = false;
       mydata.systems[15].hasBoss = false;
       mydata.systems[15].linkedNodes = new int[4];
       mydata.systems[15].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P71 = new DataManager.PlanetData(); //Ori
       P71.location = new Vector3(11, 1, 20);
       P71.mapLocation = new Vector2(0, 0);
       P71.textureID = 0;
       P71.controller = DataManager.Race.Ori;
       P71.status = DataManager.PlanetStatus.Hostile;
       P71.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P71.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P71.raceForceSizes[5] = DataManager.ForceSize.Medium;
       P71.lastController = DataManager.Race.Asgard;
       P71.lastMaxForceSize = DataManager.ForceSize.None;
       P71.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[15].planets[0] = P71;

       DataManager.PlanetData P72 = new DataManager.PlanetData(); //Ori
       P72.location = new Vector3(3, 0, 17);
       P72.mapLocation = new Vector2(0, 0);
       P72.textureID = 0;
       P72.controller = DataManager.Race.Ori;
       P72.status = DataManager.PlanetStatus.Hostile;
       P72.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P72.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P72.raceForceSizes[5] = DataManager.ForceSize.Medium;
       P72.lastController = DataManager.Race.Asgard;
       P72.lastMaxForceSize = DataManager.ForceSize.None;
       P72.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[15].planets[1] = P72;

       DataManager.PlanetData P73 = new DataManager.PlanetData(); //Ori
       P73.location = new Vector3(1, 1, 0);
       P73.mapLocation = new Vector2(0, 0);
       P73.textureID = 0;
       P73.controller = DataManager.Race.Ori;
       P73.status = DataManager.PlanetStatus.Hostile;
       P73.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P73.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P73.raceForceSizes[5] = DataManager.ForceSize.Medium;
       P73.lastController = DataManager.Race.Asgard;
       P73.lastMaxForceSize = DataManager.ForceSize.None;
       P73.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[15].planets[2] = P73;

       DataManager.PlanetData P74 = new DataManager.PlanetData(); //Ori
       P74.location = new Vector3(0, 2, 20);
       P74.mapLocation = new Vector2(0, 0);
       P74.textureID = 0;
       P74.controller = DataManager.Race.Ori;
       P74.status = DataManager.PlanetStatus.Hostile;
       P74.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P74.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P74.raceForceSizes[5] = DataManager.ForceSize.Medium;
       P74.lastController = DataManager.Race.Asgard;
       P74.lastMaxForceSize = DataManager.ForceSize.None;
       P74.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[15].planets[3] = P74;

       DataManager.PlanetData P75 = new DataManager.PlanetData(); //Ori
       P75.location = new Vector3(10, 0, 2);
       P75.mapLocation = new Vector2(0, 0);
       P75.textureID = 2;
       P75.controller = DataManager.Race.Ori;
       P75.status = DataManager.PlanetStatus.Hostile;
       P75.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P75.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P75.raceForceSizes[5] = DataManager.ForceSize.Large;
       P75.lastController = DataManager.Race.Asgard;
       P75.lastMaxForceSize = DataManager.ForceSize.None;
       P75.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[15].planets[4] = P75;

       mydata.systems[16] = new DataManager.SystemData();
       mydata.systems[16].systemID = 16;
       mydata.systems[16].isNeutralZone = false;
       mydata.systems[16].hasBoss = true;
       mydata.systems[16].linkedNodes = new int[4];
       mydata.systems[16].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P76 = new DataManager.PlanetData(); //Ori
       P76.location = new Vector3(17, 0, 6);
       P76.mapLocation = new Vector2(0, 0);
       P76.textureID = 0;
       P76.controller = DataManager.Race.Ori;
       P76.status = DataManager.PlanetStatus.Hostile;
       P76.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P76.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P76.raceForceSizes[5] = DataManager.ForceSize.Medium;
       P76.lastController = DataManager.Race.Asgard;
       P76.lastMaxForceSize = DataManager.ForceSize.None;
       P76.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[16].planets[0] = P76;

       DataManager.PlanetData P77 = new DataManager.PlanetData(); //Ori
       P77.location = new Vector3(9, 2, 20);
       P77.mapLocation = new Vector2(0, 0);
       P77.textureID = 0;
       P77.controller = DataManager.Race.Ori;
       P77.status = DataManager.PlanetStatus.Hostile;
       P77.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P77.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P77.raceForceSizes[5] = DataManager.ForceSize.Medium;
       P77.lastController = DataManager.Race.Asgard;
       P77.lastMaxForceSize = DataManager.ForceSize.None;
       P77.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[16].planets[1] = P77;

       DataManager.PlanetData P78 = new DataManager.PlanetData(); //Ori
       P78.location = new Vector3(1, 1, 16);
       P78.mapLocation = new Vector2(0, 0);
       P78.textureID = 0;
       P78.controller = DataManager.Race.Ori;
       P78.status = DataManager.PlanetStatus.Hostile;
       P78.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P78.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P78.raceForceSizes[5] = DataManager.ForceSize.Medium;
       P78.lastController = DataManager.Race.Asgard;
       P78.lastMaxForceSize = DataManager.ForceSize.None;
       P78.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[16].planets[2] = P78;

       DataManager.PlanetData P79 = new DataManager.PlanetData(); //Ori
       P79.location = new Vector3(11, 0, 2);
       P79.mapLocation = new Vector2(0, 0);
       P79.textureID = 0;
       P79.controller = DataManager.Race.Ori;
       P79.status = DataManager.PlanetStatus.Hostile;
       P79.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P79.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P79.raceForceSizes[5] = DataManager.ForceSize.Medium;
       P79.lastController = DataManager.Race.Asgard;
       P79.lastMaxForceSize = DataManager.ForceSize.None;
       P79.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[16].planets[3] = P79;

       DataManager.PlanetData P80 = new DataManager.PlanetData(); //Ori
       P80.location = new Vector3(18, 1, 13);
       P80.mapLocation = new Vector2(0, 0);
       P80.textureID = 0;
       P80.controller = DataManager.Race.Ori;
       P80.status = DataManager.PlanetStatus.Hostile;
       P80.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P80.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P80.raceForceSizes[5] = DataManager.ForceSize.Large;
       P80.lastController = DataManager.Race.Asgard;
       P80.lastMaxForceSize = DataManager.ForceSize.None;
       P80.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[16].planets[4] = P80;

       mydata.systems[17] = new DataManager.SystemData();
       mydata.systems[17].systemID = 17;
       mydata.systems[17].isNeutralZone = true;
       mydata.systems[17].hasBoss = false;
       mydata.systems[17].linkedNodes = new int[4];
       mydata.systems[17].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P81 = new DataManager.PlanetData(); //Neutral
       P81.location = new Vector3(9, 2, 2);
       P81.mapLocation = new Vector2(0, 0);
       P81.textureID = 0;
       P81.controller = DataManager.Race.Human;
       P81.status = DataManager.PlanetStatus.Unknown;
       P81.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P81.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P81.lastController = DataManager.Race.Asgard;
       P81.lastMaxForceSize = DataManager.ForceSize.None;
       P81.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[17].planets[0] = P81;

       DataManager.PlanetData P82 = new DataManager.PlanetData(); //Neutral
       P82.location = new Vector3(0, 0, 10);
       P82.mapLocation = new Vector2(0, 0);
       P82.textureID = 0;
       P82.controller = DataManager.Race.Human;
       P82.status = DataManager.PlanetStatus.Unknown;
       P82.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P82.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P82.lastController = DataManager.Race.Asgard;
       P82.lastMaxForceSize = DataManager.ForceSize.None;
       P82.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[17].planets[1] = P82;

       DataManager.PlanetData P83 = new DataManager.PlanetData(); //Neutral
       P83.location = new Vector3(12, 2, 13);
       P83.mapLocation = new Vector2(0, 0);
       P83.textureID = 0;
       P83.controller = DataManager.Race.Human;
       P83.status = DataManager.PlanetStatus.Unknown;
       P83.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P83.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P83.lastController = DataManager.Race.Asgard;
       P83.lastMaxForceSize = DataManager.ForceSize.None;
       P83.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[17].planets[2] = P83;

       DataManager.PlanetData P84 = new DataManager.PlanetData(); //Neutral
       P84.location = new Vector3(18, 2, 5);
       P84.mapLocation = new Vector2(0, 0);
       P84.textureID = 0;
       P84.controller = DataManager.Race.Human;
       P84.status = DataManager.PlanetStatus.Unknown;
       P84.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P84.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P84.lastController = DataManager.Race.Asgard;
       P84.lastMaxForceSize = DataManager.ForceSize.None;
       P84.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[17].planets[3] = P84;

       DataManager.PlanetData P85 = new DataManager.PlanetData(); //Neutral
       P85.location = new Vector3(12, 1, 1);
       P85.mapLocation = new Vector2(0, 0);
       P85.textureID = 0;
       P85.controller = DataManager.Race.Human;
       P85.status = DataManager.PlanetStatus.Unknown;
       P85.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P85.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P85.lastController = DataManager.Race.Asgard;
       P85.lastMaxForceSize = DataManager.ForceSize.None;
       P85.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[17].planets[4] = P85;

       mydata.systems[18] = new DataManager.SystemData();
       mydata.systems[18].systemID = 18;
       mydata.systems[18].isNeutralZone = true;
       mydata.systems[18].hasBoss = false;
       mydata.systems[18].linkedNodes = new int[4];
       mydata.systems[18].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P86 = new DataManager.PlanetData(); //Neutral
       P86.location = new Vector3(7, 1, 18);
       P86.mapLocation = new Vector2(0, 0);
       P86.textureID = 0;
       P86.controller = DataManager.Race.Human;
       P86.status = DataManager.PlanetStatus.Unknown;
       P86.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P86.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P86.lastController = DataManager.Race.Asgard;
       P86.lastMaxForceSize = DataManager.ForceSize.None;
       P86.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[18].planets[0] = P86;

       DataManager.PlanetData P87 = new DataManager.PlanetData(); //Neutral
       P87.location = new Vector3(3, 0, 5);
       P87.mapLocation = new Vector2(0, 0);
       P87.textureID = 0;
       P87.controller = DataManager.Race.Human;
       P87.status = DataManager.PlanetStatus.Unknown;
       P87.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P87.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P87.lastController = DataManager.Race.Asgard;
       P87.lastMaxForceSize = DataManager.ForceSize.None;
       P87.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[18].planets[1] = P87;

       DataManager.PlanetData P88 = new DataManager.PlanetData(); //Neutral
       P88.location = new Vector3(3, 1, 19);
       P88.mapLocation = new Vector2(0, 0);
       P88.textureID = 0;
       P88.controller = DataManager.Race.Human;
       P88.status = DataManager.PlanetStatus.Unknown;
       P88.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P88.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P88.lastController = DataManager.Race.Asgard;
       P88.lastMaxForceSize = DataManager.ForceSize.None;
       P88.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[18].planets[2] = P88;

       DataManager.PlanetData P89 = new DataManager.PlanetData(); //Neutral
       P89.location = new Vector3(14, 0, 3);
       P89.mapLocation = new Vector2(0, 0);
       P89.textureID = 0;
       P89.controller = DataManager.Race.Human;
       P89.status = DataManager.PlanetStatus.Unknown;
       P89.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P89.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P89.lastController = DataManager.Race.Asgard;
       P89.lastMaxForceSize = DataManager.ForceSize.None;
       P89.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[18].planets[3] = P89;

       DataManager.PlanetData P90 = new DataManager.PlanetData(); //Neutral
       P90.location = new Vector3(7, 1, 3);
       P90.mapLocation = new Vector2(0, 0);
       P90.textureID = 0;
       P90.controller = DataManager.Race.Human;
       P90.status = DataManager.PlanetStatus.Unknown;
       P90.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P90.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P90.lastController = DataManager.Race.Asgard;
       P90.lastMaxForceSize = DataManager.ForceSize.None;
       P90.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[18].planets[4] = P90;

       mydata.systems[19] = new DataManager.SystemData();
       mydata.systems[19].systemID = 19;
       mydata.systems[19].isNeutralZone = true;
       mydata.systems[19].hasBoss = false;
       mydata.systems[19].linkedNodes = new int[4];
       mydata.systems[19].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P91 = new DataManager.PlanetData(); //Neutral
       P91.location = new Vector3(17, 1, 18);
       P91.mapLocation = new Vector2(0, 0);
       P91.textureID = 0;
       P91.controller = DataManager.Race.Human;
       P91.status = DataManager.PlanetStatus.Unknown;
       P91.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P91.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P91.lastController = DataManager.Race.Asgard;
       P91.lastMaxForceSize = DataManager.ForceSize.None;
       P91.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[19].planets[0] = P91;

       DataManager.PlanetData P92 = new DataManager.PlanetData(); //Neutral
       P92.location = new Vector3(13, 0, 4);
       P92.mapLocation = new Vector2(0, 0);
       P92.textureID = 0;
       P92.controller = DataManager.Race.Human;
       P92.status = DataManager.PlanetStatus.Unknown;
       P92.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P92.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P92.lastController = DataManager.Race.Asgard;
       P92.lastMaxForceSize = DataManager.ForceSize.None;
       P92.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[19].planets[1] = P92;

       DataManager.PlanetData P93 = new DataManager.PlanetData(); //Neutral
       P93.location = new Vector3(14, 1, 17);
       P93.mapLocation = new Vector2(0, 0);
       P93.textureID = 0;
       P93.controller = DataManager.Race.Human;
       P93.status = DataManager.PlanetStatus.Unknown;
       P93.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P93.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P93.lastController = DataManager.Race.Asgard;
       P93.lastMaxForceSize = DataManager.ForceSize.None;
       P93.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[19].planets[2] = P93;

       DataManager.PlanetData P94 = new DataManager.PlanetData(); //Neutral
       P94.location = new Vector3(15, 0, 14);
       P94.mapLocation = new Vector2(0, 0);
       P94.textureID = 0;
       P94.controller = DataManager.Race.Human;
       P94.status = DataManager.PlanetStatus.Unknown;
       P94.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P94.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P94.lastController = DataManager.Race.Asgard;
       P94.lastMaxForceSize = DataManager.ForceSize.None;
       P94.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[19].planets[3] = P94;

       DataManager.PlanetData P95 = new DataManager.PlanetData(); //Neutral
       P95.location = new Vector3(5, 2, 8);
       P95.mapLocation = new Vector2(0, 0);
       P95.textureID = 0;
       P95.controller = DataManager.Race.Human;
       P95.status = DataManager.PlanetStatus.Unknown;
       P95.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P95.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P95.lastController = DataManager.Race.Asgard;
       P95.lastMaxForceSize = DataManager.ForceSize.None;
       P95.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[19].planets[4] = P95;

       mydata.systems[20] = new DataManager.SystemData();
       mydata.systems[20].systemID = 20;
       mydata.systems[20].isNeutralZone = true;
       mydata.systems[20].hasBoss = false;
       mydata.systems[20].linkedNodes = new int[4];
       mydata.systems[20].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P96 = new DataManager.PlanetData(); //Neutral
       P96.location = new Vector3(7, 0, 14);
       P96.mapLocation = new Vector2(0, 0);
       P96.textureID = 0;
       P96.controller = DataManager.Race.Human;
       P96.status = DataManager.PlanetStatus.Unknown;
       P96.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P96.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P96.lastController = DataManager.Race.Asgard;
       P96.lastMaxForceSize = DataManager.ForceSize.None;
       P96.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[20].planets[0] = P96;

       DataManager.PlanetData P97 = new DataManager.PlanetData(); //Neutral
       P97.location = new Vector3(6, 2, 16);
       P97.mapLocation = new Vector2(0, 0);
       P97.textureID = 0;
       P97.controller = DataManager.Race.Human;
       P97.status = DataManager.PlanetStatus.Unknown;
       P97.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P97.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P97.lastController = DataManager.Race.Asgard;
       P97.lastMaxForceSize = DataManager.ForceSize.None;
       P97.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[20].planets[1] = P97;

       DataManager.PlanetData P98 = new DataManager.PlanetData(); //Neutral
       P98.location = new Vector3(11, 1, 7);
       P98.mapLocation = new Vector2(0, 0);
       P98.textureID = 0;
       P98.controller = DataManager.Race.Human;
       P98.status = DataManager.PlanetStatus.Unknown;
       P98.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P98.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P98.lastController = DataManager.Race.Asgard;
       P98.lastMaxForceSize = DataManager.ForceSize.None;
       P98.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[20].planets[2] = P98;

       DataManager.PlanetData P99 = new DataManager.PlanetData(); //Neutral
       P99.location = new Vector3(16, 0, 14);
       P99.mapLocation = new Vector2(0, 0);
       P99.textureID = 0;
       P99.controller = DataManager.Race.Human;
       P99.status = DataManager.PlanetStatus.Unknown;
       P99.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P99.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P99.lastController = DataManager.Race.Asgard;
       P99.lastMaxForceSize = DataManager.ForceSize.None;
       P99.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[20].planets[3] = P99;

       DataManager.PlanetData P100 = new DataManager.PlanetData(); //Neutral
       P100.location = new Vector3(2, 0, 19);
       P100.mapLocation = new Vector2(0, 0);
       P100.textureID = 0;
       P100.controller = DataManager.Race.Human;
       P100.status = DataManager.PlanetStatus.Unknown;
       P100.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P100.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P100.lastController = DataManager.Race.Asgard;
       P100.lastMaxForceSize = DataManager.ForceSize.None;
       P100.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[20].planets[4] = P100;

       mydata.systems[21] = new DataManager.SystemData();
       mydata.systems[21].systemID = 21;
       mydata.systems[21].isNeutralZone = true;
       mydata.systems[21].hasBoss = false;
       mydata.systems[21].linkedNodes = new int[4];
       mydata.systems[21].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P101 = new DataManager.PlanetData(); //Neutral
       P101.location = new Vector3(19, 1, 0);
       P101.mapLocation = new Vector2(0, 0);
       P101.textureID = 0;
       P101.controller = DataManager.Race.Human;
       P101.status = DataManager.PlanetStatus.Unknown;
       P101.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P101.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P101.lastController = DataManager.Race.Asgard;
       P101.lastMaxForceSize = DataManager.ForceSize.None;
       P101.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[21].planets[0] = P101;

       DataManager.PlanetData P102 = new DataManager.PlanetData(); //Neutral
       P102.location = new Vector3(0, 1, 16);
       P102.mapLocation = new Vector2(0, 0);
       P102.textureID = 0;
       P102.controller = DataManager.Race.Human;
       P102.status = DataManager.PlanetStatus.Unknown;
       P102.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P102.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P102.lastController = DataManager.Race.Asgard;
       P102.lastMaxForceSize = DataManager.ForceSize.None;
       P102.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[21].planets[1] = P102;

       DataManager.PlanetData P103 = new DataManager.PlanetData(); //Neutral
       P103.location = new Vector3(1, 1, 10);
       P103.mapLocation = new Vector2(0, 0);
       P103.textureID = 0;
       P103.controller = DataManager.Race.Human;
       P103.status = DataManager.PlanetStatus.Unknown;
       P103.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P103.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P103.lastController = DataManager.Race.Asgard;
       P103.lastMaxForceSize = DataManager.ForceSize.None;
       P103.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[21].planets[2] = P103;

       DataManager.PlanetData P104 = new DataManager.PlanetData(); //Neutral
       P104.location = new Vector3(0, 1, 0);
       P104.mapLocation = new Vector2(0, 0);
       P104.textureID = 0;
       P104.controller = DataManager.Race.Human;
       P104.status = DataManager.PlanetStatus.Unknown;
       P104.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P104.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P104.lastController = DataManager.Race.Asgard;
       P104.lastMaxForceSize = DataManager.ForceSize.None;
       P104.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[21].planets[3] = P104;

       DataManager.PlanetData P105 = new DataManager.PlanetData(); //Neutral
       P105.location = new Vector3(17, 1, 9);
       P105.mapLocation = new Vector2(0, 0);
       P105.textureID = 0;
       P105.controller = DataManager.Race.Human;
       P105.status = DataManager.PlanetStatus.Unknown;
       P105.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P105.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P105.lastController = DataManager.Race.Asgard;
       P105.lastMaxForceSize = DataManager.ForceSize.None;
       P105.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[21].planets[4] = P105;

       mydata.systems[22] = new DataManager.SystemData();
       mydata.systems[22].systemID = 22;
       mydata.systems[22].isNeutralZone = true;
       mydata.systems[22].hasBoss = false;
       mydata.systems[22].linkedNodes = new int[4];
       mydata.systems[22].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P106 = new DataManager.PlanetData(); //Neutral
       P106.location = new Vector3(3, 1, 9);
       P106.mapLocation = new Vector2(0, 0);
       P106.textureID = 0;
       P106.controller = DataManager.Race.Human;
       P106.status = DataManager.PlanetStatus.Unknown;
       P106.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P106.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P106.lastController = DataManager.Race.Asgard;
       P106.lastMaxForceSize = DataManager.ForceSize.None;
       P106.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[22].planets[0] = P106;

       DataManager.PlanetData P107 = new DataManager.PlanetData(); //Neutral
       P107.location = new Vector3(15, 2, 8);
       P107.mapLocation = new Vector2(0, 0);
       P107.textureID = 0;
       P107.controller = DataManager.Race.Human;
       P107.status = DataManager.PlanetStatus.Unknown;
       P107.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P107.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P107.lastController = DataManager.Race.Asgard;
       P107.lastMaxForceSize = DataManager.ForceSize.None;
       P107.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[22].planets[1] = P107;

       DataManager.PlanetData P108 = new DataManager.PlanetData(); //Neutral
       P108.location = new Vector3(9, 1, 13);
       P108.mapLocation = new Vector2(0, 0);
       P108.textureID = 0;
       P108.controller = DataManager.Race.Human;
       P108.status = DataManager.PlanetStatus.Unknown;
       P108.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P108.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P108.lastController = DataManager.Race.Asgard;
       P108.lastMaxForceSize = DataManager.ForceSize.None;
       P108.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[22].planets[2] = P108;

       DataManager.PlanetData P109 = new DataManager.PlanetData(); //Neutral
       P109.location = new Vector3(2, 0, 16);
       P109.mapLocation = new Vector2(0, 0);
       P109.textureID = 0;
       P109.controller = DataManager.Race.Human;
       P109.status = DataManager.PlanetStatus.Unknown;
       P109.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P109.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P109.lastController = DataManager.Race.Asgard;
       P109.lastMaxForceSize = DataManager.ForceSize.None;
       P109.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[22].planets[3] = P109;

       DataManager.PlanetData P110 = new DataManager.PlanetData(); //Neutral
       P110.location = new Vector3(18, 2, 7);
       P110.mapLocation = new Vector2(0, 0);
       P110.textureID = 0;
       P110.controller = DataManager.Race.Human;
       P110.status = DataManager.PlanetStatus.Unknown;
       P110.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P110.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P110.lastController = DataManager.Race.Asgard;
       P110.lastMaxForceSize = DataManager.ForceSize.None;
       P110.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[22].planets[4] = P110;

       mydata.systems[23] = new DataManager.SystemData();
       mydata.systems[23].systemID = 23;
       mydata.systems[23].isNeutralZone = true;
       mydata.systems[23].hasBoss = false;
       mydata.systems[23].linkedNodes = new int[4];
       mydata.systems[23].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P111 = new DataManager.PlanetData(); //Neutral
       P111.location = new Vector3(2, 1, 4);
       P111.mapLocation = new Vector2(0, 0);
       P111.textureID = 0;
       P111.controller = DataManager.Race.Human;
       P111.status = DataManager.PlanetStatus.Unknown;
       P111.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P111.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P111.lastController = DataManager.Race.Asgard;
       P111.lastMaxForceSize = DataManager.ForceSize.None;
       P111.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[23].planets[0] = P111;

       DataManager.PlanetData P112 = new DataManager.PlanetData(); //Neutral
       P112.location = new Vector3(4, 2, 13);
       P112.mapLocation = new Vector2(0, 0);
       P112.textureID = 0;
       P112.controller = DataManager.Race.Human;
       P112.status = DataManager.PlanetStatus.Unknown;
       P112.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P112.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P112.lastController = DataManager.Race.Asgard;
       P112.lastMaxForceSize = DataManager.ForceSize.None;
       P112.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[23].planets[1] = P112;

       DataManager.PlanetData P113 = new DataManager.PlanetData(); //Neutral
       P113.location = new Vector3(13, 0, 3);
       P113.mapLocation = new Vector2(0, 0);
       P113.textureID = 0;
       P113.controller = DataManager.Race.Human;
       P113.status = DataManager.PlanetStatus.Unknown;
       P113.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P113.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P113.lastController = DataManager.Race.Asgard;
       P113.lastMaxForceSize = DataManager.ForceSize.None;
       P113.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[23].planets[2] = P113;

       DataManager.PlanetData P114 = new DataManager.PlanetData(); //Neutral
       P114.location = new Vector3(12, 0, 10);
       P114.mapLocation = new Vector2(0, 0);
       P114.textureID = 0;
       P114.controller = DataManager.Race.Human;
       P114.status = DataManager.PlanetStatus.Unknown;
       P114.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P114.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P114.lastController = DataManager.Race.Asgard;
       P114.lastMaxForceSize = DataManager.ForceSize.None;
       P114.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[23].planets[3] = P114;

       DataManager.PlanetData P115 = new DataManager.PlanetData(); //Neutral
       P115.location = new Vector3(3, 0, 8);
       P115.mapLocation = new Vector2(0, 0);
       P115.textureID = 0;
       P115.controller = DataManager.Race.Human;
       P115.status = DataManager.PlanetStatus.Unknown;
       P115.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P115.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P115.lastController = DataManager.Race.Asgard;
       P115.lastMaxForceSize = DataManager.ForceSize.None;
       P115.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[23].planets[4] = P115;

       mydata.systems[24] = new DataManager.SystemData();
       mydata.systems[24].systemID = 24;
       mydata.systems[24].isNeutralZone = true;
       mydata.systems[24].hasBoss = false;
       mydata.systems[24].linkedNodes = new int[4];
       mydata.systems[24].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P116 = new DataManager.PlanetData(); //Neutral
       P116.location = new Vector3(15, 2, 0);
       P116.mapLocation = new Vector2(0, 0);
       P116.textureID = 0;
       P116.controller = DataManager.Race.Human;
       P116.status = DataManager.PlanetStatus.Unknown;
       P116.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P116.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P116.lastController = DataManager.Race.Asgard;
       P116.lastMaxForceSize = DataManager.ForceSize.None;
       P116.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[24].planets[0] = P116;

       DataManager.PlanetData P117 = new DataManager.PlanetData(); //Neutral
       P117.location = new Vector3(19, 1, 16);
       P117.mapLocation = new Vector2(0, 0);
       P117.textureID = 0;
       P117.controller = DataManager.Race.Human;
       P117.status = DataManager.PlanetStatus.Unknown;
       P117.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P117.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P117.lastController = DataManager.Race.Asgard;
       P117.lastMaxForceSize = DataManager.ForceSize.None;
       P117.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[24].planets[1] = P117;

       DataManager.PlanetData P118 = new DataManager.PlanetData(); //Neutral
       P118.location = new Vector3(4, 1, 5);
       P118.mapLocation = new Vector2(0, 0);
       P118.textureID = 0;
       P118.controller = DataManager.Race.Human;
       P118.status = DataManager.PlanetStatus.Unknown;
       P118.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P118.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P118.lastController = DataManager.Race.Asgard;
       P118.lastMaxForceSize = DataManager.ForceSize.None;
       P118.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[24].planets[2] = P118;

       DataManager.PlanetData P119 = new DataManager.PlanetData(); //Neutral
       P119.location = new Vector3(10, 1, 0);
       P119.mapLocation = new Vector2(0, 0);
       P119.textureID = 0;
       P119.controller = DataManager.Race.Human;
       P119.status = DataManager.PlanetStatus.Unknown;
       P119.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P119.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P119.lastController = DataManager.Race.Asgard;
       P119.lastMaxForceSize = DataManager.ForceSize.None;
       P119.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[24].planets[3] = P119;

       DataManager.PlanetData P120 = new DataManager.PlanetData(); //Neutral
       P120.location = new Vector3(8, 2, 10);
       P120.mapLocation = new Vector2(0, 0);
       P120.textureID = 0;
       P120.controller = DataManager.Race.Human;
       P120.status = DataManager.PlanetStatus.Unknown;
       P120.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P120.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P120.lastController = DataManager.Race.Asgard;
       P120.lastMaxForceSize = DataManager.ForceSize.None;
       P120.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[24].planets[4] = P120;

       mydata.systems[25] = new DataManager.SystemData();
       mydata.systems[25].systemID = 25;
       mydata.systems[25].isNeutralZone = true;
       mydata.systems[25].hasBoss = false;
       mydata.systems[25].linkedNodes = new int[4];
       mydata.systems[25].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P121 = new DataManager.PlanetData(); //Neutral
       P121.location = new Vector3(20, 0, 15);
       P121.mapLocation = new Vector2(0, 0);
       P121.textureID = 0;
       P121.controller = DataManager.Race.Human;
       P121.status = DataManager.PlanetStatus.Unknown;
       P121.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P121.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P121.lastController = DataManager.Race.Asgard;
       P121.lastMaxForceSize = DataManager.ForceSize.None;
       P121.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[25].planets[0] = P121;

       DataManager.PlanetData P122 = new DataManager.PlanetData(); //Neutral
       P122.location = new Vector3(2, 2, 18);
       P122.mapLocation = new Vector2(0, 0);
       P122.textureID = 0;
       P122.controller = DataManager.Race.Human;
       P122.status = DataManager.PlanetStatus.Unknown;
       P122.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P122.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P122.lastController = DataManager.Race.Asgard;
       P122.lastMaxForceSize = DataManager.ForceSize.None;
       P122.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[25].planets[1] = P122;

       DataManager.PlanetData P123 = new DataManager.PlanetData(); //Neutral
       P123.location = new Vector3(16, 1, 13);
       P123.mapLocation = new Vector2(0, 0);
       P123.textureID = 0;
       P123.controller = DataManager.Race.Human;
       P123.status = DataManager.PlanetStatus.Unknown;
       P123.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P123.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P123.lastController = DataManager.Race.Asgard;
       P123.lastMaxForceSize = DataManager.ForceSize.None;
       P123.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[25].planets[2] = P123;

       DataManager.PlanetData P124 = new DataManager.PlanetData(); //Neutral
       P124.location = new Vector3(17, 1, 3);
       P124.mapLocation = new Vector2(0, 0);
       P124.textureID = 0;
       P124.controller = DataManager.Race.Human;
       P124.status = DataManager.PlanetStatus.Unknown;
       P124.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P124.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P124.lastController = DataManager.Race.Asgard;
       P124.lastMaxForceSize = DataManager.ForceSize.None;
       P124.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[25].planets[3] = P124;

       DataManager.PlanetData P125 = new DataManager.PlanetData(); //Neutral
       P125.location = new Vector3(6, 0, 14);
       P125.mapLocation = new Vector2(0, 0);
       P125.textureID = 0;
       P125.controller = DataManager.Race.Human;
       P125.status = DataManager.PlanetStatus.Unknown;
       P125.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P125.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P125.lastController = DataManager.Race.Asgard;
       P125.lastMaxForceSize = DataManager.ForceSize.None;
       P125.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[25].planets[4] = P125;

       mydata.systems[26] = new DataManager.SystemData();
       mydata.systems[26].systemID = 26;
       mydata.systems[26].isNeutralZone = true;
       mydata.systems[26].hasBoss = false;
       mydata.systems[26].linkedNodes = new int[4];
       mydata.systems[26].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P126 = new DataManager.PlanetData(); //Neutral
       P126.location = new Vector3(18, 0, 4);
       P126.mapLocation = new Vector2(0, 0);
       P126.textureID = 0;
       P126.controller = DataManager.Race.Human;
       P126.status = DataManager.PlanetStatus.Unknown;
       P126.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P126.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P126.lastController = DataManager.Race.Asgard;
       P126.lastMaxForceSize = DataManager.ForceSize.None;
       P126.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[26].planets[0] = P126;

       DataManager.PlanetData P127 = new DataManager.PlanetData(); //Neutral
       P127.location = new Vector3(5, 1, 6);
       P127.mapLocation = new Vector2(0, 0);
       P127.textureID = 0;
       P127.controller = DataManager.Race.Human;
       P127.status = DataManager.PlanetStatus.Unknown;
       P127.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P127.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P127.lastController = DataManager.Race.Asgard;
       P127.lastMaxForceSize = DataManager.ForceSize.None;
       P127.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[26].planets[1] = P127;

       DataManager.PlanetData P128 = new DataManager.PlanetData(); //Neutral
       P128.location = new Vector3(16, 2, 5);
       P128.mapLocation = new Vector2(0, 0);
       P128.textureID = 0;
       P128.controller = DataManager.Race.Human;
       P128.status = DataManager.PlanetStatus.Unknown;
       P128.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P128.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P128.lastController = DataManager.Race.Asgard;
       P128.lastMaxForceSize = DataManager.ForceSize.None;
       P128.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[26].planets[2] = P128;

       DataManager.PlanetData P129 = new DataManager.PlanetData(); //Neutral
       P129.location = new Vector3(12, 2, 4);
       P129.mapLocation = new Vector2(0, 0);
       P129.textureID = 0;
       P129.controller = DataManager.Race.Human;
       P129.status = DataManager.PlanetStatus.Unknown;
       P129.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P129.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P129.lastController = DataManager.Race.Asgard;
       P129.lastMaxForceSize = DataManager.ForceSize.None;
       P129.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[26].planets[3] = P129;

       DataManager.PlanetData P130 = new DataManager.PlanetData(); //Neutral
       P130.location = new Vector3(19, 2, 6);
       P130.mapLocation = new Vector2(0, 0);
       P130.textureID = 0;
       P130.controller = DataManager.Race.Human;
       P130.status = DataManager.PlanetStatus.Unknown;
       P130.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P130.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P130.lastController = DataManager.Race.Asgard;
       P130.lastMaxForceSize = DataManager.ForceSize.None;
       P130.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[26].planets[4] = P130;

       mydata.systems[27] = new DataManager.SystemData();
       mydata.systems[27].systemID = 27;
       mydata.systems[27].isNeutralZone = true;
       mydata.systems[27].hasBoss = false;
       mydata.systems[27].linkedNodes = new int[4];
       mydata.systems[27].planets = new DataManager.PlanetData[5];

       DataManager.PlanetData P131 = new DataManager.PlanetData(); //Neutral
       P131.location = new Vector3(12, 0, 7);
       P131.mapLocation = new Vector2(0, 0);
       P131.textureID = 0;
       P131.controller = DataManager.Race.Human;
       P131.status = DataManager.PlanetStatus.Unknown;
       P131.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P131.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P131.lastController = DataManager.Race.Asgard;
       P131.lastMaxForceSize = DataManager.ForceSize.None;
       P131.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[27].planets[0] = P131;

       DataManager.PlanetData P132 = new DataManager.PlanetData(); //Neutral
       P132.location = new Vector3(14, 1, 2);
       P132.mapLocation = new Vector2(0, 0);
       P132.textureID = 0;
       P132.controller = DataManager.Race.Human;
       P132.status = DataManager.PlanetStatus.Unknown;
       P132.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P132.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P132.lastController = DataManager.Race.Asgard;
       P132.lastMaxForceSize = DataManager.ForceSize.None;
       P132.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[27].planets[1] = P132;

       DataManager.PlanetData P133 = new DataManager.PlanetData(); //Neutral
       P133.location = new Vector3(16, 0, 8);
       P133.mapLocation = new Vector2(0, 0);
       P133.textureID = 0;
       P133.controller = DataManager.Race.Human;
       P133.status = DataManager.PlanetStatus.Unknown;
       P133.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P133.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P133.lastController = DataManager.Race.Asgard;
       P133.lastMaxForceSize = DataManager.ForceSize.None;
       P133.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[27].planets[2] = P133;

       DataManager.PlanetData P134 = new DataManager.PlanetData(); //Neutral
       P134.location = new Vector3(10, 2, 2);
       P134.mapLocation = new Vector2(0, 0);
       P134.textureID = 0;
       P134.controller = DataManager.Race.Human;
       P134.status = DataManager.PlanetStatus.Unknown;
       P134.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P134.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P134.lastController = DataManager.Race.Asgard;
       P134.lastMaxForceSize = DataManager.ForceSize.None;
       P134.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[27].planets[3] = P134;

       DataManager.PlanetData P135 = new DataManager.PlanetData(); //Neutral
       P135.location = new Vector3(7, 0, 2);
       P135.mapLocation = new Vector2(0, 0);
       P135.textureID = 0;
       P135.controller = DataManager.Race.Human;
       P135.status = DataManager.PlanetStatus.Unknown;
       P135.raceForceSizes = new DataManager.ForceSize[6];
       for (int i = 0; i < 6; i++)
       {
           P135.raceForceSizes[i] = DataManager.ForceSize.None;
       }
       P135.lastController = DataManager.Race.Asgard;
       P135.lastMaxForceSize = DataManager.ForceSize.None;
       P135.lastStatus = DataManager.PlanetStatus.Unknown;
       mydata.systems[27].planets[4] = P135;

       mydata.systems[0].mapGridLocation = new Vector2(2, 2); // Human
       mydata.systems[1].mapGridLocation = new Vector2(1, 0); // Asgard
       mydata.systems[2].mapGridLocation = new Vector2(2, 0); // Asgard
       mydata.systems[3].mapGridLocation = new Vector2(2, 1); // Asgard
       mydata.systems[4].mapGridLocation = new Vector2(0, 5); // Goauld
       mydata.systems[5].mapGridLocation = new Vector2(1, 4); // Goauld
       mydata.systems[6].mapGridLocation = new Vector2(1, 5); // Goauld
       mydata.systems[7].mapGridLocation = new Vector2(2, 5); // Goauld
       mydata.systems[8].mapGridLocation = new Vector2(3, 5); // Goauld
       mydata.systems[9].mapGridLocation = new Vector2(4, 2); // Wraith
       mydata.systems[10].mapGridLocation = new Vector2(4, 3); // Wraith
       mydata.systems[11].mapGridLocation = new Vector2(5, 1); // Wraith
       mydata.systems[12].mapGridLocation = new Vector2(5, 2); // Wraith
       mydata.systems[13].mapGridLocation = new Vector2(5, 3); // Wraith
       mydata.systems[14].mapGridLocation = new Vector2(0, 2); // Replicator
       mydata.systems[15].mapGridLocation = new Vector2(5, 5); // Ori - this is the one on map
       mydata.systems[16].mapGridLocation = new Vector2(-1, -1); // Ori - this is the separate galaxy
       mydata.systems[17].mapGridLocation = new Vector2(1, 1); // Neutral
       mydata.systems[18].mapGridLocation = new Vector2(1, 2); // Neutral
       mydata.systems[19].mapGridLocation = new Vector2(1, 3); // Neutral
       mydata.systems[20].mapGridLocation = new Vector2(2, 3); // Neutral
       mydata.systems[21].mapGridLocation = new Vector2(2, 4); // Neutral
       mydata.systems[22].mapGridLocation = new Vector2(3, 1); // Neutral
       mydata.systems[23].mapGridLocation = new Vector2(3, 2); // Neutral
       mydata.systems[24].mapGridLocation = new Vector2(3, 3); // Neutral
       mydata.systems[25].mapGridLocation = new Vector2(3, 4); // Neutral
       mydata.systems[26].mapGridLocation = new Vector2(4, 1); // Neutral
       mydata.systems[27].mapGridLocation = new Vector2(4, 4); // Neutral

       P0.textureID = 0; // System: 0 race: Human
       P1.textureID = 33; // System: 1 race: Asgard
       P2.textureID = 18; // System: 1 race: Asgard
       P3.textureID = 18; // System: 1 race: Asgard
       P4.textureID = 18; // System: 1 race: Asgard
       P5.textureID = 8; // System: 1 race: Asgard
       P6.textureID = 3; // System: 2 race: Asgard
       P7.textureID = 22; // System: 2 race: Asgard
       P8.textureID = 25; // System: 2 race: Asgard
       P9.textureID = 24; // System: 2 race: Asgard
       P10.textureID = 1; // System: 2 race: Asgard TRADE WORLD
       P11.textureID = 17; // System: 3 race: Asgard
       P12.textureID = 16; // System: 3 race: Asgard
       P13.textureID = 19; // System: 3 race: Asgard
       P14.textureID = 7; // System: 3 race: Asgard
       P15.textureID = 11; // System: 3 race: Asgard
       P16.textureID = 8; // System: 4 race: Goauld
       P17.textureID = 3; // System: 4 race: Goauld
       P18.textureID = 19; // System: 4 race: Goauld
       P19.textureID = 23; // System: 4 race: Goauld
       P20.textureID = 19; // System: 4 race: Goauld
       P21.textureID = 20; // System: 5 race: Goauld
       P22.textureID = 31; // System: 5 race: Goauld
       P23.textureID = 23; // System: 5 race: Goauld
       P24.textureID = 24; // System: 5 race: Goauld
       P25.textureID = 4; // System: 5 race: Goauld
       P26.textureID = 3; // System: 6 race: Goauld
       P27.textureID = 24; // System: 6 race: Goauld
       P28.textureID = 16; // System: 6 race: Goauld
       P29.textureID = 33; // System: 6 race: Goauld
       P30.textureID = 11; // System: 6 race: Goauld
       P31.textureID = 22; // System: 7 race: Goauld
       P32.textureID = 3; // System: 7 race: Goauld
       P33.textureID = 11; // System: 7 race: Goauld
       P34.textureID = 5; // System: 7 race: Goauld
       P35.textureID = 22; // System: 7 race: Goauld
       P36.textureID = 32; // System: 8 race: Goauld
       P37.textureID = 19; // System: 8 race: Goauld
       P38.textureID = 9; // System: 8 race: Goauld
       P39.textureID = 23; // System: 8 race: Goauld
       P40.textureID = 27; // System: 8 race: Goauld
       P41.textureID = 24; // System: 9 race: Wraith
       P42.textureID = 13; // System: 9 race: Wraith
       P43.textureID = 7; // System: 9 race: Wraith
       P44.textureID = 4; // System: 9 race: Wraith
       P45.textureID = 7; // System: 9 race: Wraith
       P46.textureID = 4; // System: 10 race: Wraith
       P47.textureID = 27; // System: 10 race: Wraith
       P48.textureID = 4; // System: 10 race: Wraith
       P49.textureID = 33; // System: 10 race: Wraith
       P50.textureID = 30; // System: 10 race: Wraith
       P51.textureID = 28; // System: 11 race: Wraith
       P52.textureID = 8; // System: 11 race: Wraith
       P53.textureID = 19; // System: 11 race: Wraith
       P54.textureID = 7; // System: 11 race: Wraith
       P55.textureID = 15; // System: 11 race: Wraith
       P56.textureID = 4; // System: 12 race: Wraith
       P57.textureID = 28; // System: 12 race: Wraith
       P58.textureID = 28; // System: 12 race: Wraith
       P59.textureID = 16; // System: 12 race: Wraith
       P60.textureID = 7; // System: 12 race: Wraith
       P61.textureID = 13; // System: 13 race: Wraith
       P62.textureID = 9; // System: 13 race: Wraith
       P63.textureID = 25; // System: 13 race: Wraith
       P64.textureID = 16; // System: 13 race: Wraith
       P65.textureID = 31; // System: 13 race: Wraith
       P66.textureID = 16; // System: 14 race: Replicator
       P67.textureID = 7; // System: 14 race: Replicator
       P68.textureID = 16; // System: 14 race: Replicator
       P69.textureID = 18; // System: 14 race: Replicator
       P70.textureID = 12; // System: 14 race: Replicator
       P71.textureID = 23; // System: 15 race: Ori
       P72.textureID = 27; // System: 15 race: Ori
       P73.textureID = 29; // System: 15 race: Ori
       P74.textureID = 30; // System: 15 race: Ori
       P75.textureID = 2; // System: 15 race: Ori SUPERGATE
       P76.textureID = 2; // System: 16 race: Ori SUPERGATE
       P77.textureID = 27; // System: 16 race: Ori
       P78.textureID = 22; // System: 16 race: Ori
       P79.textureID = 19; // System: 16 race: Ori
       P80.textureID = 3; // System: 16 race: Ori
       P81.textureID = 9; // System: 17 race: Neutral
       P82.textureID = 22; // System: 17 race: Neutral
       P83.textureID = 17; // System: 17 race: Neutral
       P84.textureID = 5; // System: 17 race: Neutral
       P85.textureID = 16; // System: 17 race: Neutral
       P86.textureID = 17; // System: 18 race: Neutral
       P87.textureID = 18; // System: 18 race: Neutral
       P88.textureID = 6; // System: 18 race: Neutral
       P89.textureID = 24; // System: 18 race: Neutral
       P90.textureID = 20; // System: 18 race: Neutral
       P91.textureID = 6; // System: 19 race: Neutral
       P92.textureID = 25; // System: 19 race: Neutral
       P93.textureID = 26; // System: 19 race: Neutral
       P94.textureID = 18; // System: 19 race: Neutral
       P95.textureID = 16; // System: 19 race: Neutral
       P96.textureID = 18; // System: 20 race: Neutral
       P97.textureID = 21; // System: 20 race: Neutral
       P98.textureID = 24; // System: 20 race: Neutral
       P99.textureID = 7; // System: 20 race: Neutral
       P100.textureID = 24; // System: 20 race: Neutral
       P101.textureID = 30; // System: 21 race: Neutral
       P102.textureID = 13; // System: 21 race: Neutral
       P103.textureID = 20; // System: 21 race: Neutral
       P104.textureID = 23; // System: 21 race: Neutral
       P105.textureID = 10; // System: 21 race: Neutral
       P106.textureID = 3; // System: 22 race: Neutral
       P107.textureID = 12; // System: 22 race: Neutral
       P108.textureID = 22; // System: 22 race: Neutral
       P109.textureID = 12; // System: 22 race: Neutral
       P110.textureID = 7; // System: 22 race: Neutral
       P111.textureID = 11; // System: 23 race: Neutral
       P112.textureID = 9; // System: 23 race: Neutral
       P113.textureID = 11; // System: 23 race: Neutral
       P114.textureID = 16; // System: 23 race: Neutral
       P115.textureID = 32; // System: 23 race: Neutral
       P116.textureID = 25; // System: 24 race: Neutral
       P117.textureID = 25; // System: 24 race: Neutral
       P118.textureID = 22; // System: 24 race: Neutral
       P119.textureID = 3; // System: 24 race: Neutral
       P120.textureID = 17; // System: 24 race: Neutral
       P121.textureID = 32; // System: 25 race: Neutral
       P122.textureID = 3; // System: 25 race: Neutral
       P123.textureID = 26; // System: 25 race: Neutral
       P124.textureID = 32; // System: 25 race: Neutral
       P125.textureID = 9; // System: 25 race: Neutral
       P126.textureID = 32; // System: 26 race: Neutral
       P127.textureID = 3; // System: 26 race: Neutral
       P128.textureID = 13; // System: 26 race: Neutral
       P129.textureID = 17; // System: 26 race: Neutral
       P130.textureID = 13; // System: 26 race: Neutral
       P131.textureID = 22; // System: 27 race: Neutral
       P132.textureID = 5; // System: 27 race: Neutral
       P133.textureID = 17; // System: 27 race: Neutral
       P134.textureID = 28; // System: 27 race: Neutral
       P135.textureID = 7; // System: 27 race: Neutral

       mydata.systems[0].planets[0] = P0;
       mydata.systems[1].planets[0] = P1;
       mydata.systems[1].planets[1] = P2;
       mydata.systems[1].planets[2] = P3;
       mydata.systems[1].planets[3] = P4;
       mydata.systems[1].planets[4] = P5;
       mydata.systems[2].planets[0] = P6;
       mydata.systems[2].planets[1] = P7;
       mydata.systems[2].planets[2] = P8;
       mydata.systems[2].planets[3] = P9;
       mydata.systems[2].planets[4] = P10;
       mydata.systems[3].planets[0] = P11;
       mydata.systems[3].planets[1] = P12;
       mydata.systems[3].planets[2] = P13;
       mydata.systems[3].planets[3] = P14;
       mydata.systems[3].planets[4] = P15;
       mydata.systems[4].planets[0] = P16;
       mydata.systems[4].planets[1] = P17;
       mydata.systems[4].planets[2] = P18;
       mydata.systems[4].planets[3] = P19;
       mydata.systems[4].planets[4] = P20;
       mydata.systems[5].planets[0] = P21;
       mydata.systems[5].planets[1] = P22;
       mydata.systems[5].planets[2] = P23;
       mydata.systems[5].planets[3] = P24;
       mydata.systems[5].planets[4] = P25;
       mydata.systems[6].planets[0] = P26;
       mydata.systems[6].planets[1] = P27;
       mydata.systems[6].planets[2] = P28;
       mydata.systems[6].planets[3] = P29;
       mydata.systems[6].planets[4] = P30;
       mydata.systems[7].planets[0] = P31;
       mydata.systems[7].planets[1] = P32;
       mydata.systems[7].planets[2] = P33;
       mydata.systems[7].planets[3] = P34;
       mydata.systems[7].planets[4] = P35;
       mydata.systems[8].planets[0] = P36;
       mydata.systems[8].planets[1] = P37;
       mydata.systems[8].planets[2] = P38;
       mydata.systems[8].planets[3] = P39;
       mydata.systems[8].planets[4] = P40;
       mydata.systems[9].planets[0] = P41;
       mydata.systems[9].planets[1] = P42;
       mydata.systems[9].planets[2] = P43;
       mydata.systems[9].planets[3] = P44;
       mydata.systems[9].planets[4] = P45;
       mydata.systems[10].planets[0] = P46;
       mydata.systems[10].planets[1] = P47;
       mydata.systems[10].planets[2] = P48;
       mydata.systems[10].planets[3] = P49;
       mydata.systems[10].planets[4] = P50;
       mydata.systems[11].planets[0] = P51;
       mydata.systems[11].planets[1] = P52;
       mydata.systems[11].planets[2] = P53;
       mydata.systems[11].planets[3] = P54;
       mydata.systems[11].planets[4] = P55;
       mydata.systems[12].planets[0] = P56;
       mydata.systems[12].planets[1] = P57;
       mydata.systems[12].planets[2] = P58;
       mydata.systems[12].planets[3] = P59;
       mydata.systems[12].planets[4] = P60;
       mydata.systems[13].planets[0] = P61;
       mydata.systems[13].planets[1] = P62;
       mydata.systems[13].planets[2] = P63;
       mydata.systems[13].planets[3] = P64;
       mydata.systems[13].planets[4] = P65;
       mydata.systems[14].planets[0] = P66;
       mydata.systems[14].planets[1] = P67;
       mydata.systems[14].planets[2] = P68;
       mydata.systems[14].planets[3] = P69;
       mydata.systems[14].planets[4] = P70;
       mydata.systems[15].planets[0] = P71;
       mydata.systems[15].planets[1] = P72;
       mydata.systems[15].planets[2] = P73;
       mydata.systems[15].planets[3] = P74;
       mydata.systems[15].planets[4] = P75;
       mydata.systems[16].planets[0] = P76;
       mydata.systems[16].planets[1] = P77;
       mydata.systems[16].planets[2] = P78;
       mydata.systems[16].planets[3] = P79;
       mydata.systems[16].planets[4] = P80;
       mydata.systems[17].planets[0] = P81;
       mydata.systems[17].planets[1] = P82;
       mydata.systems[17].planets[2] = P83;
       mydata.systems[17].planets[3] = P84;
       mydata.systems[17].planets[4] = P85;
       mydata.systems[18].planets[0] = P86;
       mydata.systems[18].planets[1] = P87;
       mydata.systems[18].planets[2] = P88;
       mydata.systems[18].planets[3] = P89;
       mydata.systems[18].planets[4] = P90;
       mydata.systems[19].planets[0] = P91;
       mydata.systems[19].planets[1] = P92;
       mydata.systems[19].planets[2] = P93;
       mydata.systems[19].planets[3] = P94;
       mydata.systems[19].planets[4] = P95;
       mydata.systems[20].planets[0] = P96;
       mydata.systems[20].planets[1] = P97;
       mydata.systems[20].planets[2] = P98;
       mydata.systems[20].planets[3] = P99;
       mydata.systems[20].planets[4] = P100;
       mydata.systems[21].planets[0] = P101;
       mydata.systems[21].planets[1] = P102;
       mydata.systems[21].planets[2] = P103;
       mydata.systems[21].planets[3] = P104;
       mydata.systems[21].planets[4] = P105;
       mydata.systems[22].planets[0] = P106;
       mydata.systems[22].planets[1] = P107;
       mydata.systems[22].planets[2] = P108;
       mydata.systems[22].planets[3] = P109;
       mydata.systems[22].planets[4] = P110;
       mydata.systems[23].planets[0] = P111;
       mydata.systems[23].planets[1] = P112;
       mydata.systems[23].planets[2] = P113;
       mydata.systems[23].planets[3] = P114;
       mydata.systems[23].planets[4] = P115;
       mydata.systems[24].planets[0] = P116;
       mydata.systems[24].planets[1] = P117;
       mydata.systems[24].planets[2] = P118;
       mydata.systems[24].planets[3] = P119;
       mydata.systems[24].planets[4] = P120;
       mydata.systems[25].planets[0] = P121;
       mydata.systems[25].planets[1] = P122;
       mydata.systems[25].planets[2] = P123;
       mydata.systems[25].planets[3] = P124;
       mydata.systems[25].planets[4] = P125;
       mydata.systems[26].planets[0] = P126;
       mydata.systems[26].planets[1] = P127;
       mydata.systems[26].planets[2] = P128;
       mydata.systems[26].planets[3] = P129;
       mydata.systems[26].planets[4] = P130;
       mydata.systems[27].planets[0] = P131;
       mydata.systems[27].planets[1] = P132;
       mydata.systems[27].planets[2] = P133;
       mydata.systems[27].planets[3] = P134;
       mydata.systems[27].planets[4] = P135;

       mydata.systems[0].owner = DataManager.Race.Human;
       mydata.systems[0].skyboxID = 0;
       mydata.systems[1].owner = DataManager.Race.Asgard; // Asgard
       mydata.systems[1].skyboxID = 4; // Asgard
       mydata.systems[2].owner = DataManager.Race.Asgard; // Asgard
       mydata.systems[2].skyboxID = 4; // Asgard
       mydata.systems[3].owner = DataManager.Race.Asgard; // Asgard
       mydata.systems[3].skyboxID = 4; // Asgard
       mydata.systems[4].owner = DataManager.Race.Goauld; // Goauld
       mydata.systems[4].skyboxID = 3; // Goauld
       mydata.systems[5].owner = DataManager.Race.Goauld; // Goauld
       mydata.systems[5].skyboxID = 3; // Goauld
       mydata.systems[6].owner = DataManager.Race.Goauld; // Goauld
       mydata.systems[6].skyboxID = 3; // Goauld
       mydata.systems[7].owner = DataManager.Race.Goauld; // Goauld
       mydata.systems[7].skyboxID = 3; // Goauld
       mydata.systems[8].owner = DataManager.Race.Goauld; // Goauld
       mydata.systems[8].skyboxID = 3; // Goauld
       mydata.systems[9].owner = DataManager.Race.Wraith; // Wraith
       mydata.systems[9].skyboxID = 5; // Wraith
       mydata.systems[10].owner = DataManager.Race.Wraith; // Wraith
       mydata.systems[10].skyboxID = 5; // Wraith
       mydata.systems[11].owner = DataManager.Race.Wraith; // Wraith
       mydata.systems[11].skyboxID = 5; // Wraith
       mydata.systems[12].owner = DataManager.Race.Wraith; // Wraith
       mydata.systems[12].skyboxID = 5; // Wraith
       mydata.systems[13].owner = DataManager.Race.Wraith; // Wraith
       mydata.systems[13].skyboxID = 5; // Wraith
       mydata.systems[14].owner = DataManager.Race.Replicator; // Replicator
       mydata.systems[14].skyboxID = 0; // Replicator
       mydata.systems[15].owner = DataManager.Race.Ori; // Ori
       mydata.systems[15].skyboxID = 1; // Ori
       mydata.systems[16].owner = DataManager.Race.Ori; // Ori
       mydata.systems[16].skyboxID = 1; // Ori
       mydata.systems[17].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[17].skyboxID = 2; // Neutral
       mydata.systems[18].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[18].skyboxID = 0; // Neutral
       mydata.systems[19].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[19].skyboxID = 0; // Neutral
       mydata.systems[20].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[20].skyboxID = 0; // Neutral
       mydata.systems[21].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[21].skyboxID = 2; // Neutral
       mydata.systems[22].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[22].skyboxID = 0; // Neutral
       mydata.systems[23].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[23].skyboxID = 0; // Neutral
       mydata.systems[24].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[24].skyboxID = 0; // Neutral
       mydata.systems[25].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[25].skyboxID = 2; // Neutral
       mydata.systems[26].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[26].skyboxID = 0; // Neutral
       mydata.systems[27].owner = DataManager.Race.Asgard; // Neutral
       mydata.systems[27].skyboxID = 0; // Neutral

       // Earth has no linked systems
       mydata.systems[0].linkedNodes = new int[0];

       Vector2[] offsets = new Vector2[] { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1) };
       for (int i = 1; i < mydata.systems.Length; i++)
       {
           if (i == 16 || i == 17) continue;

           // find all the adjacent cells
           int[] temp = new int[4];
           int total = 0;
           for (int j = 0; j < 4; j++)
           {
               temp[j] = locatePairID(mydata.systems, mydata.systems[i].mapGridLocation + offsets[j]);
               if (temp[j] != -1) total++;
           }

           // insert the non-negative values
           mydata.systems[i].linkedNodes = new int[total];
           for (int j = 0; j < 4; j++)
           {
               if(temp[j] == -1) continue;
               total--;
               mydata.systems[i].linkedNodes[total] = temp[j];
           }
       }

        // END SYSTEMS

       // BEGIN PLAYER DATA

       mydata.player = new DataManager.PlayerData();

       mydata.player.ships = new DataManager.PlayerShips[3];

       // 0 -Prometheus
       mydata.player.ships[0].shipID = 0;

       // 1 - Deadalus
       mydata.player.ships[1].shipID = 1;

       // 2 - Asgard Mothership
       mydata.player.ships[2].shipID = 2;

       // configure all the default values to 0s
       for (int i = 0; i < 3; i++)
       {
           mydata.player.ships[i].shipWeapons = new int[10];
           mydata.player.ships[i].enabledWeapons = new int[10];

           for (int j = 0; j < 10; j++)
           {
               mydata.player.ships[i].shipWeapons[j] = 0;
               mydata.player.ships[i].enabledWeapons[j] = 0;
           }

           mydata.player.ships[i].shipLevel = 0;
           mydata.player.ships[i].shipExp = 0;
           mydata.player.ships[i].shieldPower = 0;
           mydata.player.ships[i].weaponPower = 0;
           mydata.player.ships[i].speedPower = 0;
           mydata.player.ships[i].owned = false;
       }
       mydata.player.currentShip = -1;
       mydata.player.ships[0].owned = false;

       mydata.player.resources = new int[8];
       for (int i = 0; i < 8; i++)
       {
           mydata.player.resources[i] = 0;
       }

       saveData(mydata);
   }

   private int locatePairID(DataManager.SystemData[] systems, Vector2 searchPos)
   {
       for (int i = 1; i < systems.Length; i++)
       {
           if (systems[i].mapGridLocation.Equals(searchPos)) return i;
       }

       return -1;
   }

   public void generateShipDateBase()
    {
       DataManager.ShipDatabase database = new DataManager.ShipDatabase();
       database.ships = new DataManager.ShipData[12];

       /****************
        *  HUMAN SHIPS
        *****************/

       // Prometheus
       database.ships[0].isBoss = false;
       database.ships[0].prefabID = 0;
       database.ships[0].resourceMultiplier = 0;
       database.ships[0].resources = new int[] { 0, 0, 0, 0, 0, 0, 0 }; 
       database.ships[0].shipID = 0;
       database.ships[0].shipName = "Prometheus";
       database.ships[0].shipRace = DataManager.Race.Human;
       database.ships[0].turnSpeed = 0.8f;
       database.ships[0].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[0].AIStats[i].hull = 10000 + 2000 * i;
           database.ships[0].AIStats[i].power = 10000 + 3000 * i;
           database.ships[0].AIStats[i].shield = 5000 + 2000 * i;
           database.ships[0].AIStats[i].speed = 10;

           database.ships[0].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[0].AIStats[i].maxWeapons[j] = 0;
           }
           
           // Asgard Beam Weapon
           if (i > 3)
           {
               if (i > 8) database.ships[0].AIStats[i].maxWeapons[0] = 3;
               else if (i > 6) database.ships[0].AIStats[i].maxWeapons[0] = 2;
               else database.ships[0].AIStats[i].maxWeapons[0] = 1;
           }

           // Ori Beam Weapon
           if (i > 5)
           {
               if (i == 9) database.ships[0].AIStats[i].maxWeapons[1] = 2;
               else database.ships[0].AIStats[i].maxWeapons[1] = 1;
           }
           
           // Asgard Energy Weapon
           database.ships[0].AIStats[i].maxWeapons[2] = i;

           // Goa'uld Energy Weapon
           if (i > 1)
           {
               if (i > 7) database.ships[0].AIStats[i].maxWeapons[3] = 15;
               else if (i > 5) database.ships[0].AIStats[i].maxWeapons[3] = 13;
               else if (i > 3) database.ships[0].AIStats[i].maxWeapons[3] = 10;
               else database.ships[0].AIStats[i].maxWeapons[3] = 5;
           }

           // Wraith Energy Weapon
           if (i > 1)
           {
               if (i > 7) database.ships[0].AIStats[i].maxWeapons[4] = 4;
               else if (i > 5) database.ships[0].AIStats[i].maxWeapons[4] = 3;
               else if (i > 3) database.ships[0].AIStats[i].maxWeapons[4] = 2;
               else database.ships[0].AIStats[i].maxWeapons[4] = 1;
           }

           // Wraith Large Energy Weapon
           if (i > 3)
           {
               if (i > 7) database.ships[0].AIStats[i].maxWeapons[5] = 3;
               else if (i > 5) database.ships[0].AIStats[i].maxWeapons[5] = 2;
               else database.ships[0].AIStats[i].maxWeapons[5] = 1;
           }

           // Human projectile
           if (i > 7) database.ships[0].AIStats[i].maxWeapons[7] = 9;
           else if (i > 5) database.ships[0].AIStats[i].maxWeapons[7] = 8;
           else if (i > 3) database.ships[0].AIStats[i].maxWeapons[7] = 6;
           else if (i > 1) database.ships[0].AIStats[i].maxWeapons[7] = 4;
           else database.ships[0].AIStats[i].maxWeapons[7] = 2;

           // Human rocket
           if (i > 8) database.ships[0].AIStats[i].maxWeapons[8] = 5;
           else if (i > 5) database.ships[0].AIStats[i].maxWeapons[8] = 4;
           else if (i > 3) database.ships[0].AIStats[i].maxWeapons[8] = 3;
           else if (i > 1) database.ships[0].AIStats[i].maxWeapons[8] = 2;
           else database.ships[0].AIStats[i].maxWeapons[8] = 1;
       }

       // Deadalus
       database.ships[1].isBoss = false;
       database.ships[1].prefabID = 1; 
       database.ships[1].resourceMultiplier = 0;
       database.ships[1].resources = new int[] { 1500, 3000, 0, 0, 0, 300, 7000 };
       database.ships[1].shipID = 1;
       database.ships[1].shipName = "Daedalus";
       database.ships[1].shipRace = DataManager.Race.Human;
       database.ships[1].turnSpeed = 1.0f; 
       database.ships[1].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[1].AIStats[i].hull = 15000 + 2700 * i;
           database.ships[1].AIStats[i].power = 1000 + 2000 * i;
           database.ships[1].AIStats[i].shield = 20000 + 3000 * i;
           database.ships[1].AIStats[i].speed = 10;

           database.ships[1].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[1].AIStats[i].maxWeapons[j] = 0;
           }

           // Asgard Beam Weapon
           if (i > 0)
           {
               if (i > 8) database.ships[1].AIStats[i].maxWeapons[0] = 4;
               else if (i > 6) database.ships[1].AIStats[i].maxWeapons[0] = 3;
               else if (i > 4) database.ships[1].AIStats[i].maxWeapons[0] = 2;
               else database.ships[1].AIStats[i].maxWeapons[0] = 1;
           }

           // Ori Beam Weapon
           if (i > 5)
           {
               if (i == 9) database.ships[1].AIStats[i].maxWeapons[1] = 2;
               else database.ships[1].AIStats[i].maxWeapons[1] = 1;
           }

           // Asgard Energy Weapon
           database.ships[1].AIStats[i].maxWeapons[2] = i;

           // Goa'uld Energy Weapon
           if (i > 1)
           {
               if (i > 7) database.ships[1].AIStats[i].maxWeapons[3] = 15;
               else if (i > 5) database.ships[1].AIStats[i].maxWeapons[3] = 13;
               else if (i > 3) database.ships[1].AIStats[i].maxWeapons[3] = 10;
               else database.ships[1].AIStats[i].maxWeapons[3] = 5;
           }

           // Wraith Energy Weapon
           if (i > 1)
           {
               if (i > 7) database.ships[1].AIStats[i].maxWeapons[4] = 4;
               else if (i > 5) database.ships[1].AIStats[i].maxWeapons[4] = 3;
               else if (i > 3) database.ships[1].AIStats[i].maxWeapons[4] = 2;
               else database.ships[1].AIStats[i].maxWeapons[4] = 1;
           }

           // Wraith Large Energy Weapon
           if (i > 3)
           {
               if (i > 7) database.ships[1].AIStats[i].maxWeapons[5] = 3;
               else if (i > 5) database.ships[1].AIStats[i].maxWeapons[5] = 2;
               else database.ships[1].AIStats[i].maxWeapons[5] = 1;
           }

           // Human projectile
           if (i > 7) database.ships[1].AIStats[i].maxWeapons[7] = 10;
           else if (i > 5) database.ships[1].AIStats[i].maxWeapons[7] = 9;
           else if (i > 3) database.ships[1].AIStats[i].maxWeapons[7] = 7;
           else if (i > 1) database.ships[1].AIStats[i].maxWeapons[7] = 5;
           else database.ships[1].AIStats[i].maxWeapons[7] = 3;

           // Human rocket
           if (i > 7) database.ships[1].AIStats[i].maxWeapons[8] = 5;
           else if (i > 5) database.ships[1].AIStats[i].maxWeapons[8] = 4;
           else if (i > 3) database.ships[1].AIStats[i].maxWeapons[8] = 3;
           else if (i > 1) database.ships[1].AIStats[i].maxWeapons[8] = 2;
           else database.ships[1].AIStats[i].maxWeapons[8] = 1;
       }


       /****************
        *  ASGARD SHIPS
        *****************/

       // Asgard Mothership (player version)
       database.ships[2].isBoss = false;
       database.ships[2].prefabID = 2;
       database.ships[2].resourceMultiplier = 0;
       database.ships[2].resources = new int[] { 6000, 25000, 0, 0, 0, 1000, 20000 };
       database.ships[2].shipID = 2;
       database.ships[2].shipName = "Asgard Mothership";
       database.ships[2].shipRace = DataManager.Race.Human;
       database.ships[2].turnSpeed = 0.8f; 
       database.ships[2].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[2].AIStats[i].hull = 10000 + 1000 * i;
           database.ships[2].AIStats[i].power = 1500 + 1500 * i;
           database.ships[2].AIStats[i].shield = 30000 + 2000 * i;
           database.ships[2].AIStats[i].speed = 10;
        
           database.ships[2].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[2].AIStats[i].maxWeapons[j] = 0;
           }

           // Asgard Beam Weapon
            if (i > 7) database.ships[2].AIStats[i].maxWeapons[0] = 5;
            else if (i > 5) database.ships[2].AIStats[i].maxWeapons[0] = 4;
            else if (i > 3) database.ships[2].AIStats[i].maxWeapons[0] = 3;
            else if (i > 1) database.ships[2].AIStats[i].maxWeapons[0] = 2;
            else database.ships[2].AIStats[i].maxWeapons[0] = 1;

           // Ori Beam Weapon
           if (i > 5)
           {
               if (i == 9) database.ships[2].AIStats[i].maxWeapons[1] = 2;
               else database.ships[2].AIStats[i].maxWeapons[1] = 1;
           }

           // Asgard Energy Weapon
           database.ships[2].AIStats[i].maxWeapons[2] = i+3;

           // Goa'uld Energy Weapon
           if (i > 1)
           {
               if (i > 7) database.ships[2].AIStats[i].maxWeapons[3] = 15;
               else if (i > 5) database.ships[2].AIStats[i].maxWeapons[3] = 13;
               else if (i > 3) database.ships[2].AIStats[i].maxWeapons[3] = 10;
               else database.ships[2].AIStats[i].maxWeapons[3] = 5;
           }

           // Wraith Energy Weapon
           if (i > 1)
           {
               if (i > 7) database.ships[2].AIStats[i].maxWeapons[4] = 4;
               else if (i > 5) database.ships[2].AIStats[i].maxWeapons[4] = 3;
               else if (i > 3) database.ships[2].AIStats[i].maxWeapons[4] = 2;
               else database.ships[2].AIStats[i].maxWeapons[4] = 1;
           }

           // Wraith Large Energy Weapon
           if (i > 3)
           {
               if (i > 7) database.ships[2].AIStats[i].maxWeapons[5] = 3;
               else if (i > 5) database.ships[2].AIStats[i].maxWeapons[5] = 2;
               else database.ships[2].AIStats[i].maxWeapons[5] = 1;
           }

           // Human projectile
           if (i > 7) database.ships[2].AIStats[i].maxWeapons[7] = 5;
           else if (i > 5) database.ships[2].AIStats[i].maxWeapons[7] = 3;
           else if (i > 3) database.ships[2].AIStats[i].maxWeapons[7] = 2;
           else if (i > 1) database.ships[2].AIStats[i].maxWeapons[7] = 1;
           else database.ships[2].AIStats[i].maxWeapons[7] = 0;

           // Human rocket
           if (i > 7) database.ships[2].AIStats[i].maxWeapons[8] = 4;
           else if (i > 5) database.ships[2].AIStats[i].maxWeapons[8] = 3;
           else if (i > 3) database.ships[2].AIStats[i].maxWeapons[8] = 2;
           else if (i > 1) database.ships[2].AIStats[i].maxWeapons[8] = 1;
           else database.ships[2].AIStats[i].maxWeapons[8] = 0;
       }


       // Asgard Mothership
       database.ships[3].isBoss = false;
       database.ships[3].prefabID = 3; 
       database.ships[3].resourceMultiplier = 1;
       database.ships[3].resources = new int[] { 1, 3, 0, 0, 0, 1, 10 };
       database.ships[3].shipID = 3;
       database.ships[3].shipName = "Asgard Mothership";
       database.ships[3].shipRace = DataManager.Race.Asgard;
       database.ships[3].turnSpeed = 0.8f;
       database.ships[3].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[3].AIStats[i].hull = 15000 + 3000 * i;
           database.ships[3].AIStats[i].power = 100000;
           database.ships[3].AIStats[i].shield = 20000 + 1500 * i;
           database.ships[3].AIStats[i].speed = 10;

           database.ships[3].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[3].AIStats[i].maxWeapons[j] = 0;
           }

           // Asgard Beam Weapon
           if (i > 7) database.ships[3].AIStats[i].maxWeapons[0] = 5;
           else if (i > 5) database.ships[3].AIStats[i].maxWeapons[0] = 4;
           else if (i > 3) database.ships[3].AIStats[i].maxWeapons[0] = 3;
           else if (i > 1) database.ships[3].AIStats[i].maxWeapons[0] = 2;
           else database.ships[3].AIStats[i].maxWeapons[0] = 1;

           // Asgard Energy Weapon
           database.ships[3].AIStats[i].maxWeapons[2] = i + 3;
       }

       /****************
       *  GOA'ULD SHIPS
       *****************/

       // Goa'uld Ha'tak
       database.ships[4].isBoss = false;
       database.ships[4].prefabID = 4;
       database.ships[4].resourceMultiplier = 2;
       database.ships[4].resources = new int[] { 1, 2, 0, 0, 1, 1, 15 };
       database.ships[4].shipID = 4;
       database.ships[4].shipName = "Goa\'uld Ha\'tak";
       database.ships[4].shipRace = DataManager.Race.Goauld;
       database.ships[4].turnSpeed = 0.6f;
       database.ships[4].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[4].AIStats[i].hull = 15000 + 3000 * i;
           database.ships[4].AIStats[i].power = 100000;
           database.ships[4].AIStats[i].shield = 30000 + 1500 * i;
           database.ships[4].AIStats[i].speed = 10;

           database.ships[4].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[4].AIStats[i].maxWeapons[j] = 0;
           }

           database.ships[4].AIStats[i].maxWeapons[3] = (int)Mathf.Pow(1.3f, i) + 2;
       }

       // Goa'uld Advanced Ha'tak
       database.ships[5].isBoss = false; ;
       database.ships[5].prefabID = 5; 
       database.ships[5].resourceMultiplier = 3;
       database.ships[5].resources = new int[] { 1, 3, 0, 0, 2, 1, 20 }; 
       database.ships[5].shipID = 5;
       database.ships[5].shipName = "Goa\'uld Advanced Ha\'tak";
       database.ships[5].shipRace = DataManager.Race.Goauld;
       database.ships[5].turnSpeed = 0.7f;
       database.ships[5].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[5].AIStats[i].hull = 30000+i*1500;
           database.ships[5].AIStats[i].power = 100000;
           database.ships[5].AIStats[i].shield = 30000 + 1500 * i;
           database.ships[5].AIStats[i].speed = 10;

           database.ships[5].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[5].AIStats[i].maxWeapons[j] = 0;
           }

           database.ships[5].AIStats[i].maxWeapons[3] = (int)Mathf.Pow(1.4f, i)+3;
       }

       // Goa'uld Anubus's Flagship (boss)
       database.ships[6].isBoss = true;
       database.ships[6].prefabID = 6; 
       database.ships[6].resourceMultiplier = 5;
       database.ships[6].resources = new int[] { 3, 5, 0, 0, 4, 1, 40 }; 
       database.ships[6].shipID = 6;
       database.ships[6].shipName = "Goa\'uld Anubus\'s Flagship";
       database.ships[6].shipRace = DataManager.Race.Goauld;
       database.ships[6].turnSpeed = 0.5f;
       database.ships[6].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[6].AIStats[i].hull = 100000;
           database.ships[6].AIStats[i].power = 100000;
           database.ships[6].AIStats[i].shield = 100000;
           database.ships[6].AIStats[i].speed = 20;

           database.ships[6].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[6].AIStats[i].maxWeapons[j] = 0;
           }

           database.ships[6].AIStats[i].maxWeapons[3] = 30;
       }

       /****************
       *  WRAITH SHIPS
       *****************/

       // Wraith Hive Ship
       database.ships[7].isBoss = false;
       database.ships[7].prefabID = 7;
       database.ships[7].resourceMultiplier = 3;
       database.ships[7].resources = new int[] { 3, 4, 0, 2, 0, 1, 25 }; 
       database.ships[7].shipID = 7;
       database.ships[7].shipName = "Wraith Hive Ship";
       database.ships[7].shipRace = DataManager.Race.Wraith;
       database.ships[7].turnSpeed = 0.6f;
       database.ships[7].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[7].AIStats[i].hull = 40000 + 4000 * i;
           database.ships[7].AIStats[i].power = 100000;
           database.ships[7].AIStats[i].shield = 0;
           database.ships[7].AIStats[i].speed = 30;

           database.ships[7].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[7].AIStats[i].maxWeapons[j] = 0;
           }

           database.ships[7].AIStats[i].maxWeapons[5] = (i + 1) / 2 + 1;
       }

       // Wraith Cruiser
       database.ships[8].isBoss = false;
       database.ships[8].prefabID = 8; 
       database.ships[8].resourceMultiplier = 2;
       database.ships[8].resources = new int[] { 1, 4, 0, 1, 0, 1, 15 };
       database.ships[8].shipID = 8;
       database.ships[8].shipName = "Wraith Cruiser";
       database.ships[8].shipRace = DataManager.Race.Wraith;
       database.ships[8].turnSpeed = 0.7f; 
       database.ships[8].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[8].AIStats[i].hull = 30000 + 3000 * i;
           database.ships[8].AIStats[i].power = 100000;
           database.ships[8].AIStats[i].shield = 0;
           database.ships[8].AIStats[i].speed = 20;

           database.ships[8].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[8].AIStats[i].maxWeapons[j] = 0;
           }

           database.ships[8].AIStats[i].maxWeapons[4] = (i+1)/2 + 1;
       }

       // Wraith Super Hive Ship
       database.ships[9].isBoss = true;
       database.ships[9].prefabID = 9;
       database.ships[9].resourceMultiplier = 5;
       database.ships[9].resources = new int[] { 3, 5, 0, 4, 0, 1, 40 }; 
       database.ships[9].shipID = 9;
       database.ships[9].shipName = "Wraith Super Hive Ship";
       database.ships[9].shipRace = DataManager.Race.Wraith;
       database.ships[9].turnSpeed = 0.5f;
       database.ships[9].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[9].AIStats[i].hull = 100000;
           database.ships[9].AIStats[i].power = 100000; 
           database.ships[9].AIStats[i].shield = 0;
           database.ships[9].AIStats[i].speed = 35;

           database.ships[9].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[9].AIStats[i].maxWeapons[j] = 0;
           }
           
           database.ships[9].AIStats[i].maxWeapons[5] = 8;
       }


       /****************
       * REPLICATOR SHIPS
       *****************/

       // Replicator Ship
       database.ships[10].isBoss = false;
       database.ships[10].prefabID = 10;
       database.ships[10].resourceMultiplier = 2;
       database.ships[10].resources = new int[] { 1, 3, 0, 0, 0, 1, 20 };
       database.ships[10].shipID = 10;
       database.ships[10].shipName = "Replicator Ship";
       database.ships[10].shipRace = DataManager.Race.Replicator;
       database.ships[10].turnSpeed = 0.8f;
       database.ships[10].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[10].AIStats[i].hull = 20000 + 2000 * i;
           database.ships[10].AIStats[i].power = 100000;
           database.ships[10].AIStats[i].shield = 15000 + 1000 * i;
           database.ships[10].AIStats[i].speed = 10;

           database.ships[10].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[10].AIStats[i].maxWeapons[j] = 0;
           }
           
           if (i < 3) database.ships[10].AIStats[i].maxWeapons[6] = 1;
           else if (i < 5) database.ships[10].AIStats[i].maxWeapons[6] = 2;
           else if (i < 7) database.ships[10].AIStats[i].maxWeapons[6] = 3;
           else database.ships[10].AIStats[i].maxWeapons[6] = 4;

           database.ships[10].AIStats[i].maxWeapons[9] = 1;
       }

       /****************
       *  ORI SHIPS
       *****************/

       // Ori Mothership
       database.ships[11].isBoss = false;
       database.ships[11].prefabID = 11;
       database.ships[11].resourceMultiplier = 4;
       database.ships[11].resources = new int[] { 3, 5, 1, 0, 0, 1, 20 }; 
       database.ships[11].shipID = 11;
       database.ships[11].shipName = "Ori Mothership";
       database.ships[11].shipRace = DataManager.Race.Ori;
       database.ships[11].turnSpeed = 0.6f;
       database.ships[11].AIStats = new DataManager.ShipStats[10];

       for (int i = 0; i < 10; i++)
       {
           database.ships[11].AIStats[i].hull = 40000 + 2000 * i;
           database.ships[11].AIStats[i].power = 100000;
           database.ships[11].AIStats[i].shield = 20000 + 2000 * i;
           database.ships[11].AIStats[i].speed = 10;

           
           database.ships[11].AIStats[i].maxWeapons = new int[10];
           for (int j = 0; j < 10; j++)
           {
               database.ships[11].AIStats[i].maxWeapons[j] = 0;
           }
           database.ships[11].AIStats[i].maxWeapons[1] = (i < 5) ? 1 : 2;
       }

       database.weapons = new DataManager.WeaponData[10];

       // Asgard Beam Weapon
       database.weapons[0].damageType = DataManager.DamageType.Energy;
       database.weapons[0].effectType = DataManager.WeaponEffectType.Beam;
       database.weapons[0].damageValue = 90;
       database.weapons[0].fireRate = 0.7f;
       database.weapons[0].powerValue = 1000;
       database.weapons[0].prefabID = 0;
       database.weapons[0].resources = new int[] { 350, 50, 0, 0, 0, 100, 1000 };  
       database.weapons[0].weaponID = 0;
       database.weapons[0].weaponName = "Asgard Beam Weapon";

       // Ori Beam Weapon
       database.weapons[1].damageType = DataManager.DamageType.Energy;
       database.weapons[1].effectType = DataManager.WeaponEffectType.Beam;
       database.weapons[1].damageValue = 2100; 
       database.weapons[1].fireRate = 10;
       database.weapons[1].powerValue = 3000; 
       database.weapons[1].prefabID = 1;
       database.weapons[1].resources = new int[] { 5000, 200, 400, 0, 0, 250, 10000 };
       database.weapons[1].weaponID = 1;
       database.weapons[1].weaponName = "Ori Beam Weapon";

       // Asgard Energy Weapon
       database.weapons[2].damageType = DataManager.DamageType.Energy;
       database.weapons[2].effectType = DataManager.WeaponEffectType.Bolt;
       database.weapons[2].damageValue = 55;
       database.weapons[2].fireRate = 1;
       database.weapons[2].powerValue = 500;
       database.weapons[2].prefabID = 2;
       database.weapons[2].resources = new int[] { 100, 150, 0, 0, 0, 100, 200 }; 
       database.weapons[2].weaponID = 2;
       database.weapons[2].weaponName = "Asgard Energy Weapon";

       // Goa'uld Energy Weapon
       database.weapons[3].damageType = DataManager.DamageType.Energy;
       database.weapons[3].effectType = DataManager.WeaponEffectType.Bolt;
       database.weapons[3].damageValue = 30;
       database.weapons[3].fireRate = 1;
       database.weapons[3].powerValue = 250; 
       database.weapons[3].prefabID = 3;
       database.weapons[3].resources = new int[] { 50, 300, 0, 0, 300, 50, 300 }; 
       database.weapons[3].weaponID = 3;
       database.weapons[3].weaponName = "Goa\'uld Energy Weapon";

       // Wraith Energy Weapon
       database.weapons[4].damageType = DataManager.DamageType.Energy;
       database.weapons[4].effectType = DataManager.WeaponEffectType.Bolt;
       database.weapons[4].damageValue = 80;
       database.weapons[4].fireRate = 1;
       database.weapons[4].powerValue = 600; 
       database.weapons[4].prefabID = 4;
       database.weapons[4].resources = new int[] { 100, 250, 0, 300, 0, 100, 350 }; 
       database.weapons[4].weaponID = 4;
       database.weapons[4].weaponName = "Wraith Energy Weapon";

       // Wraith Large Energy Weapon
       database.weapons[5].damageType = DataManager.DamageType.Energy;
       database.weapons[5].effectType = DataManager.WeaponEffectType.Bolt;
       database.weapons[5].damageValue = 280; 
       database.weapons[5].fireRate = 2; 
       database.weapons[5].powerValue = 1200;
       database.weapons[5].prefabID = 5;
       database.weapons[5].resources = new int[] { 800, 100, 0, 800, 0, 200, 500 }; 
       database.weapons[5].weaponID = 5;
       database.weapons[5].weaponName = "Wraith Large Energy Weapon";

       // Replicator Energy Weapon
       database.weapons[6].damageType = DataManager.DamageType.Energy;
       database.weapons[6].effectType = DataManager.WeaponEffectType.Bolt;
       database.weapons[6].damageValue = 100; 
       database.weapons[6].fireRate = 1; 
       database.weapons[6].powerValue = 0;
       database.weapons[6].prefabID = 6;
       database.weapons[6].resources = new int[] { 0, 0, 0, 0, 0, 0, 0 };
       database.weapons[6].weaponID = 6;
       database.weapons[6].weaponName = "Replicator Energy Weapon";

       // Human Projectile
       database.weapons[7].damageType = DataManager.DamageType.Projectile;
       database.weapons[7].effectType = DataManager.WeaponEffectType.Projectile;
       database.weapons[7].damageValue = 8; 
       database.weapons[7].fireRate = 0.1f;
       database.weapons[7].powerValue = 100;
       database.weapons[7].prefabID = 7;
       database.weapons[7].resources = new int[] { 10, 250, 0, 0, 0, 50, 100 }; 
       database.weapons[7].weaponID = 7;
       database.weapons[7].weaponName = "Human Projectile";

       // Human Rocket
       database.weapons[8].damageType = DataManager.DamageType.Projectile;
       database.weapons[8].effectType = DataManager.WeaponEffectType.Rocket;
       database.weapons[8].damageValue = 150; 
       database.weapons[8].fireRate = 8; 
       database.weapons[8].powerValue = 150;
       database.weapons[8].prefabID = 8;
       database.weapons[8].resources = new int[] { 300, 150, 0, 0, 0, 100, 150 };
       database.weapons[8].weaponID = 8;
       database.weapons[8].weaponName = "Human Rocket";

       // Replicator Infector
       database.weapons[9].damageType = DataManager.DamageType.Replicator;
       database.weapons[9].effectType = DataManager.WeaponEffectType.Replicator;
       database.weapons[9].damageValue = 0; 
       database.weapons[9].fireRate = 0; 
       database.weapons[9].powerValue = 0;
       database.weapons[9].prefabID = 9;
       database.weapons[9].resources = new int[] { 0, 0, 0, 0, 0, 0, 0 };
       database.weapons[9].weaponID = 9;
       database.weapons[9].weaponName = "Replicator Infector";

       // store all the data
       saveData(database);
   }
} 
