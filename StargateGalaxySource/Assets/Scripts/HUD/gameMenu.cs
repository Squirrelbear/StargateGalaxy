using UnityEngine;
using System.Collections;


/***********************************************************           
    * ingame Menu: Displays the Main menu                  *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Display ingame menu                        *   
    *                                                      *   
    * Usage:                                               *   
    *      This script displays the ingame menu            *
    *      alowing the user to continue or exit game       *
    ********************************************************/
public class gameMenu : MonoBehaviour
{

//Script assests.
	public Texture menu_IMG;
	private Rect[] menu_GUI;
	public GUIStyle Mstyle;
	public GUIStyle bStyle;
	
//Script variables.
	
//size and coordinante varables
	private int panel_Width = Screen.height /3;
	private int panel_Height = Screen.height /3;
	private int panel_X = (Screen.width /2) - ((Screen.width/4)/2);
	private int panel_Y = Screen.height/3;
	
//other variables
	private bool disp = false;

	void Start () {
		bStyle.fontSize = Screen.width/120;
	}
	
	void Update () {
		if(Input.GetKeyDown ("escape"))
			toggle();
	}
    
    void OnGUI(){
		
        Rect menu_GUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
		
		if(disp){
			menu_GUI= GUI.Window(0, menu_GUI, menu_Window, menu_IMG,Mstyle);
		}
    }
	
	void menu_Window(int windowID){

        if (GUI.Button(new Rect((int)(panel_Width / 8), panel_Height / 6, (int)(panel_Width / 1.4), panel_Height / 4), "QUIT GAME", bStyle))
        {
            DestroyObject(GameObject.FindGameObjectWithTag("GameController"));
            Application.LoadLevel("MainMenu");
        }
		if (GUI.Button(new Rect((int)(panel_Width/8), panel_Height/2, (int)(panel_Width/1.4), panel_Height/4), "RETURN TO GAME", bStyle))
        toggle();
	}
	
	void toggle(){
		if(disp) disp = false;
			else disp = true;
	}
}
