using UnityEngine;
using System.Collections;

/***********************************************************           
    * Rep ali panel: Displays the repel replicators and    *
    * call ali buttons                                     *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Display repel replicators and call ali     *
    * buttons                                              *   
    *                                                      *   
    * Usage:                                               *   
    *      This script displays a small panel that hides   *
    *      behind the ship control panel untill the user   *
    *      selects 'h' button where it will scroll up      *
    *      into view.  The panel displays the call ali     *
    *      button allowing the user to call for help, and  *
    *      when the ship is invaded by replicators it      *
    *      displays the repel replicators button           *
    ********************************************************/

public class helpPanel : MonoBehaviour {

//Script assets
    public Texture playerIMG;
    public GUIStyle text;

//size and coordinante varables
    private int panel_Width = (int)(Screen.width / 4.55);
    private int panel_Height = (int)(Screen.width / 5.55);
    private int panel_X = (int)(Screen.width / 6.5);
    private int panel_Y = Screen.height - (int)(Screen.width / 9);

//Other variables
    private bool isSlide = true;
    private Rect[] playerinfoGUI;
    private int[] buttonPosArr = new int[5];


    void Start()
    {
        text.fontSize = Screen.width / 100;
        buttonPosArr[0] = (int)(panel_Width / 25);
        buttonPosArr[1] = (int)(panel_Height / 7);
        buttonPosArr[2] = (int)(panel_Width / 2.2);
        buttonPosArr[3] = (int)(panel_Height / 6);
        buttonPosArr[4] = (int)(panel_Width / 2);
    }

    void Update()
    { 
        if (Input.GetKeyDown("h")) slideToggle();
    }


    void OnGUI()
    {
        ShipAI game = (ShipAI)GameObject.FindGameObjectWithTag("Player").GetComponent("ShipAI");
        GameController game2 = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");

        GUI.depth =3;
        slide();

        Rect playerinfoGUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
        GUI.BeginGroup(playerinfoGUI, playerIMG);

        if (GUI.Button(new Rect(buttonPosArr[0], buttonPosArr[1], buttonPosArr[2], buttonPosArr[3]), "Call Allies", text))
                game2.attemptCallAllies();
            if (game.infected)
                {
                    if (GUI.Button(new Rect(buttonPosArr[4], buttonPosArr[1], buttonPosArr[2], buttonPosArr[3]), "Cleanse Ship", text))
                       game.beginPlayerCancelInfection();
                }

        GUI.EndGroup();
	}


    //toggles scroll enabler
    void slideToggle()
    {

        if (isSlide) isSlide = false;
        else isSlide = true;
    }

    //slides the panel in our out depending on the toggle
    void slide()
    {

        if (isSlide & panel_Y < (Screen.height - (int)(Screen.width / 9))) panel_Y = panel_Y + 3;
        if (!isSlide & panel_Y > (Screen.height - (int)(Screen.width / 6.1))) panel_Y = panel_Y - 3;
    }

    public void mapAction()
    {
        isSlide = true;
	}

}