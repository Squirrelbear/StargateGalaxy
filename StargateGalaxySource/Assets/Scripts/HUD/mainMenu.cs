using UnityEngine;
using System.Collections;



/***********************************************************           
    * Main Menu: Displays the Main menu                    *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Display main menu                          *   
    *                                                      *   
    * Usage:                                               *   
    *      This script displays the main game menu         *
    *      alowing the user to start a new game, continue  *
    *      current game view credits or exit the program   *
    ********************************************************/

public class mainMenu : MonoBehaviour
{

//Script assests.
	public Texture menu_IMG;
	private Rect[] menu_GUI;
	public GUIStyle Mstyle;
    public GUIStyle lStyle;	
	public GUIStyle button;
    public GUIStyle cStyle;
    public GameObject newGameFlag;
    public TextAsset cText;
//size and coordinante varables
	private int panel_Width = (int)(Screen.height/2.5);
	private int panel_Height = (int)(Screen.height /2);
	private int panel_X = (Screen.width /2) - (int)((Screen.width/3.5)/2);
	private int panel_Y = Screen.height/4;
	
		//other variables
    private bool creditsView = false;    

	void Start () {
        lStyle.fontSize = Screen.width / 20;
        cStyle.fontSize = Screen.height / 80;
	}
	
	void Update () {
	}
    
    void OnGUI(){
        Rect menu_GUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
        if (creditsView)
        {
            GUI.Label(new Rect(0,0, Screen.width, Screen.height), cText.ToString(), cStyle);
            if (GUI.Button(new Rect((int)(Screen.width / 1.4), (int)(Screen.height / 1.2), (int)(panel_Width / 1.5), panel_Height / 6), "Menu", button))
                creditsView = false;
        }
        else
        {
            menu_GUI = GUI.Window(0, menu_GUI, menu_Window, menu_IMG, Mstyle);
            GUI.Label((new Rect(Screen.width / 3, (int)(Screen.height / 1.3), Screen.width / 3, Screen.height / 6)), "logo text", lStyle);

        }
    }
	
	void menu_Window(int windowID){

        if (GUI.Button(new Rect((int)(panel_Width / 8), (int)(panel_Height / 9), (int)(panel_Width / 1.5), panel_Height / 6), "NEW GAME", button))
        {
            Instantiate(newGameFlag);
            Application.LoadLevel("LoadingScreen");
        }
		if (GUI.Button(new Rect((int)(panel_Width/8), (int)(panel_Height/3.4), (int)(panel_Width/1.5), panel_Height/6), "CONTINUE GAME", button))
                Application.LoadLevel("LoadingScreen");
        if (GUI.Button(new Rect((int)(panel_Width / 8), (int)(panel_Height / 2.1), (int)(panel_Width / 1.5), panel_Height / 6), "CREDITS", button))
            creditsView = true;
		if (GUI.Button(new Rect((int)(panel_Width/8), (int)(panel_Height/1.5), (int)(panel_Width/1.5), panel_Height/6), "QUIT GAME", button))
            	Application.Quit();
	}
	
}
