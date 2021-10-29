using UnityEngine;
using System.Collections;

public class mapPanel : MonoBehaviour {

//Script assets
    public Texture galaxy;

//panels
    private Rect[] main_view;
    private Rect[] planetGrid;

//styles
    public GUIStyle lStyle;
    public GUIStyle bStyle;
//size and coordinante varables
    private int panel_Width = Screen.width;
    private int panel_Height = (int)(Screen.width / 2);
    private int panel_X = Screen.width / 11;
    private int panel_Y = ((Screen.width / 40) - (Screen.width / 2));


//Other varaibles
    private int[] mapLablePos = new int[15];
    private bool canSlide = true;
    private Vector2[] planets = new Vector2[20];
    private int menuType = 0;
    private int startPos;
    private int[] planetSelection = { 0, 0 };
    private int[] earthButtonPos = new int[9];

    void Start()
    {
        objPosAssign();
        lStyle.fontSize = Screen.width / 80;
        startPos = panel_Y;
        planetMapLoc();
    }


    void Update()
    {
    }

    void OnGUI()
    {
        Rect main_view = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
        Rect planetGrid = new Rect((int)(panel_Width / 3.42), (int)(panel_Height / 22), (int)(panel_Width / 1.887), (int)(panel_Height / 1.135));

        panelSlide();

        GUI.BeginGroup(main_view);

        if (menuType == 0) mapMenu();


        if (menuType == 0) GUI.BeginGroup(planetGrid, galaxy);
        else GUI.BeginGroup(planetGrid);
        if (menuType == 0) mapMain();


        GUI.EndGroup();
        GUI.EndGroup();

    }

    void mapMain()
    {
        GameController game = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");

        for (int i = 0; i < game.systems.Length; i++)
        {

            for (int y = 0; y < game.systems[i].planets.Length; y++)
            {

                switch (game.systems[i].planets[y].status.ToString())
                {
                    case "HomeWorld":
                        GUI.color = Color.blue;
                        break;
                    case "TradeWorld":
                        GUI.color = Color.white;
                        break;
                    case "Unknown":
                        GUI.color = Color.yellow;
                        break;
                    case "Hostile":
                        GUI.color = Color.red;
                        break;
                    case "Friendly":
                        GUI.color = Color.green;
                        break;
                    default:
                        break;
                }
                if (GUI.Button(new Rect(game.systems[i].planets[y].mapLocation.x, game.systems[i].planets[y].mapLocation.y, 10, 10), "Map"))
                {
                    planetSelection[0] = i;
                    planetSelection[1] = y;
                }

            }

        }

    }

    void mapMenu()
    {
        GameController game = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");
        topPanel topOverlay = gameObject.GetComponent<topPanel>();

        GUI.Label((new Rect(mapLablePos[0], mapLablePos[2], panel_Width, panel_Height)), "Planet ID:", lStyle);
        GUI.Label((new Rect(mapLablePos[1], mapLablePos[2], panel_Width, panel_Height)), "P" + ((planetSelection[0]*5) +1 + planetSelection[1]), lStyle);

        GUI.Label((new Rect(mapLablePos[0], mapLablePos[3], panel_Width, panel_Height)), "Last Known \nStatus:", lStyle);
        GUI.Label((new Rect(mapLablePos[1], mapLablePos[4], panel_Width, panel_Height)), game.systems[planetSelection[0]].planets[planetSelection[1]].lastStatus.ToString() + "", lStyle);

        if (game.systems[planetSelection[0]].planets[planetSelection[1]].lastStatus == DataManager.PlanetStatus.Unknown || game.systems[planetSelection[0]].planets[planetSelection[1]].lastMaxForceSize == DataManager.ForceSize.None)
        {
            GUI.Label((new Rect(mapLablePos[0], mapLablePos[5], panel_Width, panel_Height)), "Last Known \nFaction:", lStyle);
            GUI.Label((new Rect(mapLablePos[1], mapLablePos[6], panel_Width, panel_Height)), "Unknown", lStyle);
        }
        else
        {
            GUI.Label((new Rect(mapLablePos[0], mapLablePos[5], panel_Width, panel_Height)), "Last Knowen \nFaction:", lStyle);
            GUI.Label((new Rect(mapLablePos[1], mapLablePos[6], panel_Width, panel_Height)), game.systems[planetSelection[0]].planets[planetSelection[1]].lastController.ToString() + "", lStyle);
        }
        

        GUI.Label((new Rect(mapLablePos[0], mapLablePos[7], panel_Width, panel_Height)), "Estimated \nFleet size:", lStyle);
        GUI.Label((new Rect(mapLablePos[1], mapLablePos[8], panel_Width, panel_Height)), game.systems[planetSelection[0]].planets[planetSelection[1]].lastMaxForceSize + "", lStyle);

        if (GUI.Button(new Rect(earthButtonPos[0], earthButtonPos[6], earthButtonPos[2], earthButtonPos[8]), "Jump To Planet", bStyle))
        {
            game.setHyperspaceTarget(planetSelection[0], planetSelection[1]);
            topOverlay.mapMenu();
        }
        if (GUI.Button(new Rect(earthButtonPos[0], earthButtonPos[7], earthButtonPos[2], earthButtonPos[8]), "Exit", bStyle))
            topOverlay.mapMenu();
    }
    //slides the panel in our out depending on the isSlide
    void panelSlide()
    {
        if (canSlide & panel_Y > ((Screen.width / 40) - (Screen.width / 2))) panel_Y = panel_Y - 12;
        if (!canSlide & panel_Y < 0) panel_Y = panel_Y + 12;
    }


    public void panelDisplay()
    {
        if (canSlide) canSlide = false;
        else canSlide = true;
    }


   void planetMapLoc()
    {
        GameController game = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");
        float width = (float)((panel_Width / 1.887) / 6);//127.6276
        float height = (float)((panel_Height / 1.135) / 6);//106.02.06
        float relWidth = (float)(width / 10);//6.077506
        float relHeight = (float)(height / 10);//5.048598

        for (int i = 0; i < game.systems.Length; i++)
        {
            for (int j = 0; j < game.systems[i].planets.Length; j++)
            {
                Vector2 pos = new Vector2();
                pos.x = game.systems[i].mapGridLocation.x * width + game.systems[i].planets[j].location.x * relWidth;
                pos.y = game.systems[i].mapGridLocation.y * height + game.systems[i].planets[j].location.y * relHeight;
                game.systems[i].planets[j].mapLocation = pos;
            }
        }
    }
    void objPosAssign()
    {
        //Map Label positions
        mapLablePos[0] = (int)(panel_Width / 50);
        mapLablePos[1] = (int)(panel_Width / 6.5);
        mapLablePos[2] = (int)(panel_Width / 25);
        mapLablePos[3] = (int)(panel_Width / 16);
        mapLablePos[4] = (int)(panel_Width / 13.7);
        mapLablePos[5] = (int)(panel_Width / 10.5);
        mapLablePos[6] = (int)(panel_Width / 9.3);
        mapLablePos[7] = (int)(panel_Width / 8);
        mapLablePos[8] = (int)(panel_Width / 7.3);

        //Earth menu Button positions
        earthButtonPos[0] = (int)(panel_Width / 25);
        earthButtonPos[1] = (int)(panel_Height / 12);
        earthButtonPos[2] = (int)(panel_Width / 5);
        earthButtonPos[3] = (int)(panel_Height / 7.8);
        earthButtonPos[4] = (int)(panel_Height / 4.2);
        earthButtonPos[5] = (int)(panel_Height / 2.9);
        earthButtonPos[6] = (int)(panel_Height / 1.9);
        earthButtonPos[7] = (int)(panel_Height / 1.4);
        earthButtonPos[8] = (int)(panel_Height / 7);

    }
}
