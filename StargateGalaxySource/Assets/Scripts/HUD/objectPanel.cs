using UnityEngine;
using System.Collections;

/***********************************************************           
    *Object Panel: Displays the ingame objects panel       *
    * and all ingame objects with in a specific range      *   
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Allow the user to view specific objects    *   
    *                                                      *   
    * Usage:                                               *   
    *      This script draws the right objects panel       *
    *      realative to the current screen size, the panel *
    *      scrolls out from the right side of the screen   *
    *      when the user selects the 'S' key and scrolls   *
    *      back and pressed again or when the Top overlay  *
    *      panel is called. all available objects are      *
    *      displayed in a list with the ability to target  *
    *      each object in the list                         *  
    ********************************************************/

public class objectPanel : MonoBehaviour
{

    //Script assests.
    public Texture shipsBarIMG;
    private Rect[] shipsBarGUI;
    public Vector2 scrollPos = Vector2.zero;

    //size and coordinante varables
    private int panel_Width = Screen.height / 4;
    private int panel_Height = (int)(Screen.height / 1.7);
    private int panel_X = Screen.width - (Screen.width / 70);
    private int panel_Y = Screen.height / 9;

    //Other varables.
    private bool isSlide = true;
    private int selGridInt = 0;
    private int ActiveGridInt;
    private string[] shipString;
    private string[] shipDistString;
    private int[] targetAssign;
    public GUIStyle list;
    public GUIStyle button;
    public GUIStyle Htext;
    private GameController game;
    private GameObject[] objList;

    public GameObject selectedGameObject;

    private int[,] targetB = { { 0, 1, 2 }, { 1, 2, 6 }, { 1, 2, 3 }, { 1, 2, 6 }, { 1, 2, 5 }, { 1, 2, 5 }, { 1, 2, 4 } };
    private string[] action = { "ATTACK", "MOVE", "ORBIT", "LOOT", "TRADE", "USE" };
    private int targetID;


    void Start()
    {
        list.fontSize = Screen.width / 140;
        Htext.fontSize = Screen.width / 130;
        button.fontSize = Screen.width / 120;
        GameObject obj = GameObject.Find("GameController");
        game = (GameController)obj.GetComponent("GameController");
    }

    void Update()
    {
        objList = game.getTargetableObjects().ToArray();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (Input.GetKeyDown("s")) slideToggle();	//if user has pressed the "S" key toggle isSlide variable.
        
        ActiveGridInt = selGridInt;
        if (ActiveGridInt > (objList.Length - 1))
        {
            ActiveGridInt = objList.Length - 1;
        }

        shipString = new string[objList.Length];
        shipDistString = new string[objList.Length];
        targetAssign = new int[objList.Length];
        
        for (int i = 0; i < objList.Length; i++)
        {
            shipString[i] = (string)objToString(objList[i], i);
            shipDistString[i] = (int)Vector3.Distance(player.transform.position, objList[i].transform.position) + "";
        }

        if (selectedGameObject != objList[ActiveGridInt])
        {
            for (int i = 0; i < objList.Length; i++)
            {
                if (selectedGameObject == objList[i])
                {
                    ActiveGridInt = i;
                    break;
                }
            }

            selectedGameObject = objList[ActiveGridInt];
        }
    }

    void OnGUI()
    {
        GUI.depth = 1;
        slide();									//check each frame for a slide action			

        Rect shipsBarGUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);

        GUI.BeginGroup(shipsBarGUI, shipsBarIMG);
        GUI.Label(new Rect(panel_Width / 6, panel_Height / 15, panel_Width, panel_Height), "Target ", Htext);
        GUI.Label(new Rect((int)(panel_Width / 1.7), panel_Height / 15, panel_Width, panel_Height), " Distance", Htext);
        scrollPos = GUI.BeginScrollView(new Rect(panel_Height / 14, panel_Height / 10, (int)(panel_Width / 1.3), (int)(panel_Height / 3)), scrollPos, new Rect(0, 0, (int)(panel_Width / 1.45), shipString.Length * 20));
        selGridInt = GUI.SelectionGrid(new Rect((int)((panel_Width / 1.4) / 1.6), 0, (int)(panel_Width / 1.4), shipString.Length * 20), ActiveGridInt, shipDistString, 1, list);
        selGridInt = GUI.SelectionGrid(new Rect(0, 0, (int)((panel_Width / 1.4) / 1.6), shipString.Length * 20), ActiveGridInt, shipString, 1, list);
        if (selGridInt != ActiveGridInt) selectedGameObject = objList[selGridInt];
        GUI.EndScrollView();
        buttons();

        GUI.EndGroup();
    }

    public string objToString(GameObject obj, int i)
    {
        string tag = obj.tag;

        if (tag == "Enemy" || tag == "Ally")
        {
            if (tag == "Enemy") targetAssign[i] = 0;
            if (tag == "Ally") targetAssign[i] = 1;
            ShipAI shipData = (ShipAI)obj.GetComponent("ShipAI");
            if (shipData.shipData.shipName.Length == 0) print("uh oh");
            return shipData.shipData.shipName;
        }
        else if (tag == "Planet")
        {
            targetAssign[i] = 3;
            return "Planet";
        }
        else if (tag == "WreckedShip")
        {
            targetAssign[i] = 2;
            return "Wrecked Ship";
        }
        else if (tag == "Homeworld")
        {
            targetAssign[i] = 5;
            return "Earth";
        }
        else if (tag == "Tradeworld")
        {
            targetAssign[i] = 6;
            return "Asgard Trade";
        }
        else if (tag == "Supergate")
        {
            targetAssign[i] = 4;
            return "Supergate";
        }
        else
        {
            return "other";
        }
    }


    void buttons()
    {
        topPanel topOverlay = gameObject.GetComponent<topPanel>();
        ShipAI playerData = (ShipAI)GameObject.FindGameObjectWithTag("Player").GetComponent("ShipAI");
        if (targetB[targetAssign[ActiveGridInt], 0] < 2)
        if (GUI.Button(new Rect((int)(panel_Width / 8), (int)(panel_Height / 2), (int)(panel_Width / 1.5), panel_Height / 10), action[targetB[targetAssign[ActiveGridInt], 0]], button))
        {
            if (action[targetB[targetID, 0]] == "ATTACK") playerData.initiateMove(ShipAI.MoveMode.FlyatTarget, objList[ActiveGridInt]);
            if (action[targetB[targetID, 0]] == "MOVE") playerData.initiateMove(ShipAI.MoveMode.FlyatTarget, objList[ActiveGridInt]);

        }
        if (targetB[targetAssign[ActiveGridInt], 1] == 1 || targetB[targetAssign[ActiveGridInt], 1] == 2)
        if (GUI.Button(new Rect((int)(panel_Width / 8), (int)(panel_Height / 1.55), (int)(panel_Width / 1.5), panel_Height / 10), action[targetB[targetAssign[ActiveGridInt], 1]], button))
        {
            if (action[targetB[targetID, 1]] == "MOVE") playerData.initiateMove(ShipAI.MoveMode.FlyatTarget, objList[ActiveGridInt]);
            if (action[targetB[targetID, 1]] == "ORBIT") playerData.initiateMove(ShipAI.MoveMode.Orbit, objList[ActiveGridInt]);

        }
        if (targetB[targetAssign[ActiveGridInt], 2] < 3)
            if (GUI.Button(new Rect((int)(panel_Width / 8), (int)(panel_Height / 1.25), (int)(panel_Width / 1.5), panel_Height / 10), action[targetB[targetAssign[ActiveGridInt], 2]], button))
            {
                if (action[targetB[targetID, 2]] == "ORBIT") playerData.initiateMove(ShipAI.MoveMode.Orbit, objList[ActiveGridInt]);
                {
                    if (targetID == 5) topOverlay.homeMenu();
                    else playerData.initiateMove(ShipAI.MoveMode.FlyatTarget, objList[ActiveGridInt]);
                }

            }
    }






    //toggles scroll enabler
    void slideToggle()
    {

        if (isSlide) isSlide = false;
        else isSlide = true;
    }

    public void mapAction()
    {
        isSlide = true;
    }

    //slides the panel in our out depending on the toggle
    void slide()
    {

        if (isSlide & panel_X < Screen.width - (Screen.width / 70)) panel_X = panel_X + 3;
        if (!isSlide & panel_X > Screen.width - panel_Width) panel_X = panel_X - 3;
    }
}