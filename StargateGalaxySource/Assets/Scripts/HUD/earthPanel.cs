using UnityEngine;
using System.Collections;
/***********************************************************           
    * earthPanel: Displays the top panel witch displays    *
    *  Earth Menu                                          *   
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  A Large panel that is capable of displaying*
    *                                                      *   
    * Usage:                                               *   
    *      This script provides a large drop down panel    *
    *      realative to the current screen size,           *
    *      that when called can display the earth menu,    *
    ********************************************************/

public class earthPanel : MonoBehaviour {
//Script assets
    public Vector2 scrollPos = Vector2.zero;
    public Texture[] shipIMGS = new Texture[3];

//panels
    private Rect[] mainView;
    private Rect[] planetGrid;

//styles
    public GUIStyle bStyle;
    public GUIStyle lStyle;
    public GUIStyle lStyle2;


//size and coordinante varables
    private int panelWidth = Screen.width;
    private int panelHeight = (int)(Screen.width / 2);
    private int panelX = Screen.width / 11;
    private int panelY = ((Screen.width / 40) - (Screen.width / 2));

//Other varaibles
    private int[] earthLabelPos = new int[14];
    private int[] earthButtonPos = new int[13];
    private bool canSlide = true;
    private string[] shipName = { "Promethius", "Daedalus", "Asgard Mothership" };
    private int pSelection;
    private int menuType = 0;
    private int startPos;
    private int shipSelect;


    void Start()
    {
        objPosAssign();

        bStyle.fontSize = Screen.width / 100;
        lStyle.fontSize = Screen.width / 80;
        lStyle2.fontSize = Screen.width / 80;
        startPos = panelY ;
    }


    void Update()
    {
        if (Input.GetKeyDown("n")) panelDisplay();
    }

    void OnGUI()
    {
        Rect mainView = new Rect(panelX, panelY , panelWidth, panelHeight);
        Rect planetGrid = new Rect((int)(panelWidth / 3.42), (int)(panelHeight / 22), (int)(panelWidth / 1.887), (int)(panelHeight / 1.135));

        panelSlide();

        GUI.BeginGroup(mainView);
            earthMenu();
            GUI.BeginGroup(planetGrid);      
                earthMain();
            GUI.EndGroup();
        GUI.EndGroup();

    }

    void earthMain()
    {
        GameController game = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");
        topPanel topOverlay = gameObject.GetComponent<topPanel>();

        GUI.DrawTexture(new Rect(0, 0, earthLabelPos[12], earthLabelPos[13]), shipIMGS[pSelection], ScaleMode.StretchToFill, true, 10.0f);

        GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[2], panelWidth, earthLabelPos[7])), "Ship Name:", lStyle);
        GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[2], panelWidth, earthLabelPos[7])), game.shipDatabase.ships[shipSelect].shipName, lStyle2);

        GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[3], panelWidth, earthLabelPos[7])), "Ship Level:", lStyle);
        GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[3], panelWidth, earthLabelPos[7])), "" + game.player.ships[pSelection].shipLevel, lStyle2);

        GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[4], panelWidth, earthLabelPos[7])), "Ship Experience:", lStyle);
        GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[4], panelWidth, earthLabelPos[7])), "" + game.player.ships[pSelection].shipExp, lStyle2);

        GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[5], panelWidth, earthLabelPos[7])), "Ship Power level:", lStyle);
        GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[5], panelWidth, earthLabelPos[7])), "" + game.shipDatabase.ships[pSelection].AIStats[pSelection].power, lStyle2);

        GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[6], panelWidth, earthLabelPos[7])), "Hull strength:", lStyle);
        GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[6], panelWidth, earthLabelPos[7])), "" + game.shipDatabase.ships[pSelection].AIStats[pSelection].hull, lStyle2);

        if (GUI.Button(new Rect(earthLabelPos[8], earthLabelPos[9], earthLabelPos[10], earthLabelPos[11]), "Launch", bStyle))
        {
            game.setCurrentShip(pSelection);
            topOverlay.homeMenu();
        }
    }

    void earthMenu()
    {

        GUI.Label(new Rect(earthButtonPos[0], earthButtonPos[1], panelWidth, panelHeight), "Select Ship", lStyle);
        GameController game = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");
        topPanel topOverlay = gameObject.GetComponent<topPanel>();

        if (game.player.ships[0].owned)
            if (GUI.Button(new Rect(earthButtonPos[0], earthButtonPos[3], earthButtonPos[2], earthButtonPos[1]), game.shipDatabase.ships[0].shipName, bStyle))pSelection = 0;
                
        if (game.player.ships[1].owned)
            if (GUI.Button(new Rect(earthButtonPos[0], earthButtonPos[4], earthButtonPos[2], earthButtonPos[1]), game.shipDatabase.ships[1].shipName, bStyle))pSelection = 1;
                
        if (game.player.ships[2].owned)
            if (GUI.Button(new Rect(earthButtonPos[0], earthButtonPos[5], earthButtonPos[2], earthButtonPos[1]), game.shipDatabase.ships[2].shipName, bStyle))pSelection = 2;

        //GAME OPTIONS
        if (GUI.Button(new Rect(earthButtonPos[9], earthButtonPos[10], earthButtonPos[11], earthButtonPos[12]), "Exit Earth", bStyle))
            topOverlay.homeMenu();

        if (GUI.Button(new Rect(earthButtonPos[0], earthButtonPos[6], earthButtonPos[2], earthButtonPos[8]), "Load Game", bStyle))
            game.loadGame();

        if (GUI.Button(new Rect(earthButtonPos[0], earthButtonPos[7], earthButtonPos[2], earthButtonPos[8]), "Save Game", bStyle))
            game.saveGame();
            
    }

 
    void panelSlide()
    {
        if (canSlide & panelY > ((Screen.width / 40) - (Screen.width / 2))) panelY = panelY -12;
        if (!canSlide & panelY < 0) panelY = panelY +12;
    }


   public void panelDisplay()
    {
        if (canSlide) canSlide = false;
        else canSlide = true;
    }

    void objPosAssign()
    {
        //Earth menu lable positions
        earthLabelPos[0] = (int)(panelWidth / 25);
        earthLabelPos[1] = (int)(panelWidth / 4);
        earthLabelPos[2] = (int)(panelHeight / 1.4);
        earthLabelPos[3] = (int)(panelHeight / 1.35);
        earthLabelPos[4] = (int)(panelHeight / 1.3);
        earthLabelPos[5] = (int)(panelHeight / 1.25);
        earthLabelPos[6] = (int)(panelHeight / 1.2);
        earthLabelPos[7] = (int)(panelHeight / 7);
        earthLabelPos[8] = (int)(panelWidth / 6);
        earthLabelPos[9] = (int)(panelHeight / 1.6);
        earthLabelPos[10] = (int)(panelWidth / 5);
        earthLabelPos[11] = (int)(panelHeight / 12);
        earthLabelPos[12] = (int)(panelWidth / 1.887);
        earthLabelPos[13] = (int)(panelHeight / 1.6);

        //Earth menu Button positions
        earthButtonPos[0] = (int)(panelWidth / 25);
        earthButtonPos[1] = (int)(panelHeight / 12);
        earthButtonPos[2] = (int)(panelWidth / 5);
        earthButtonPos[3] = (int)(panelHeight / 7.8);
        earthButtonPos[4] = (int)(panelHeight / 4.2);
        earthButtonPos[5] = (int)(panelHeight / 2.9);
        earthButtonPos[6] = (int)(panelHeight / 1.9);
        earthButtonPos[7] = (int)(panelHeight / 1.4);
        earthButtonPos[8] = (int)(panelHeight / 7);
        earthButtonPos[9] = (int)(panelWidth / 15);
        earthButtonPos[11] = (int)(panelWidth / 7);
        earthButtonPos[10] = (int)(panelHeight / 2.9);
        earthButtonPos[12] = (int)(panelHeight / 12);

    }

}
