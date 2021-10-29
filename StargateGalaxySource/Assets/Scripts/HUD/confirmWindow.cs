using UnityEngine;
using System.Collections;


/***********************************************************           
    * Yes No window: A conformation popup box.             *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Displays a yes/no conformation box         *   
    *                                                      *   
    * Usage:                                               *   
    *      This script displays a simple window with a     *
    *      message and a yes or no option, used for        *
    *      conformation of ingame selections.              *
    ********************************************************/

public class confirmWindow : MonoBehaviour {

//Script assests.
	public Texture menu_IMG;
	private Rect[] menu_GUI;
	public GUIStyle Mstyle;
	public GUIStyle bStyle;


	
//size and coordinante varables
	private int panel_Width = Screen.height /3;
	private int panel_Height = Screen.height /3;
	private int panel_X = (Screen.width /2) - ((Screen.width/4)/2);
	private int panel_Y = Screen.height/3;
	
	public GUIStyle yesB;
	public GUIStyle noB;
	
//other variables
	private bool disp = false;
	private bool yes_no;
    private string message ="msg here";


	void Start () {
		Mstyle.fontSize = Screen.width/100;
	}
	
	void Update () {
		if(Input.GetKeyDown ("y"))
			toggle();
	}
    
    void OnGUI(){
		
        Rect menu_GUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
		
		if(disp){
			menu_GUI= GUI.Window(0, menu_GUI, menu_Window, menu_IMG,Mstyle);
		}
    }
	
    /*This script is not currently usable, when working it will be a confirmation box displaying a messaged needing a
     * "yes" or "no" responce*/

	void menu_Window(int windowID){
		

        
		GUI.Label(new Rect((int)(panel_Width/10), panel_Height/10, (int)(panel_Width/1.2), panel_Height/5), message,Mstyle);             
		
		if (GUI.Button(new Rect((int)(panel_Width/8), panel_Height/3, (int)(panel_Width/1.4), panel_Height/5), "Yes", bStyle))              
            toggle();
		if (GUI.Button(new Rect((int)(panel_Width/8), (int)(panel_Height/1.8), (int)(panel_Width/1.4), panel_Height/5), "No", bStyle))
            toggle();
        
	}
	
	public bool Option(){
		return yes_no;
	}
	
	
	//void dispMsg(string msg,int btn){
//	}
	
	void toggle(){
		if(disp) disp = false;
			else disp = true;
	}

    /*Script that must be called by the script needing a conformation
     * from the user*/
    void sendMsg(string msg)
    {
       msg = message;
    }
}