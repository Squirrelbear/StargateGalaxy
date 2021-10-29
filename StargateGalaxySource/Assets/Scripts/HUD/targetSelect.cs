using UnityEngine;
using System.Collections;



/***********************************************************           
    * Target Select: A small box that display actions the  *
    * user can perform on selected target.                 *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  To allow the user to interact with a       *
    * selected object.                                     *
    *                                                      *   
    * Usage:                                               *   
    *      This script displays a small box containing     *
    *      a set of comads which are determined by the     *
    *      selected target.                                *
    ********************************************************/

public class targetSelect : MonoBehaviour {

//Script assests.
	public Texture menu_IMG;
	private Rect[] menu_GUI;
	public GUIStyle text;
	
	
//size and coordinante varables
	private int panel_Width = (int)(Screen.height /6);
	private int panel_Height = (int)(Screen.height /5);
	private int panel_X = 0;
    private int panel_Y = 0;
	
//other variables
	private bool disp = false;
	private int[,] targetB = {{0,1,2},{1,2,6},{1,2,3},{1,2,6},{1,2,5},{1,2,5},{1,2,4}};
	private string[] action = {"ATTACK","MOVE","ORBIT","LOOT","TRADE","USE"};
	private int targetID;

	void Start () {
		text.fontSize = Screen.width/120;
	}
	
	void Update () {
	}
    
    void OnGUI(){
       

		GUI.depth =1;
        Rect menu_GUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
		
		if(disp){
			GUI.BeginGroup(menu_GUI,menu_IMG);
				buttons();
			GUI.EndGroup();
		}
    }
	
	void buttons(){
     topPanel topOverlay = gameObject.GetComponent<topPanel>();
     MovementControl shipControl = (MovementControl)GameObject.FindGameObjectWithTag("Player").GetComponent("MovementControl");

        if (GUI.Button(new Rect((int)(panel_Width / 8), panel_Height / 11, (int)(panel_Width / 1.5), panel_Height / 4), action[targetB[targetID, 0]], text))
        {
            if (action[targetB[targetID, 0]] == "ATTACK") shipControl.selectedAttack();
            if (action[targetB[targetID, 0]] == "MOVE") shipControl.selectedMove();
            toggle();
        }
        if (GUI.Button(new Rect((int)(panel_Width / 8), (int)(panel_Height / 2.55), (int)(panel_Width / 1.5), panel_Height / 4), action[targetB[targetID, 1]], text))
        {
           if (action[targetB[targetID, 1]] == "MOVE") shipControl.selectedMove();
           if (action[targetB[targetID, 1]] == "ORBIT") shipControl.selectedOrbit();
            toggle();
        }
		if(targetB[targetID,2]!=6)
            if (GUI.Button(new Rect((int)(panel_Width / 8), (int)(panel_Height / 1.45), (int)(panel_Width / 1.5), panel_Height / 4), action[targetB[targetID, 2]], text))
            {
                if (action[targetB[targetID, 2]] == "ORBIT") shipControl.selectedOrbit();
                if (action[targetB[targetID, 2]] == "LOOT") shipControl.selectedLoot();
                if (action[targetB[targetID, 2]] == "TRADE") topOverlay.trade();
                if (action[targetB[targetID, 2]] == "USE")
                {
                    if (targetID == 5) topOverlay.homeMenu();
                    else shipControl.selectedUse();
                }
                toggle();
            }
	}
	

    public void callGUI(int tID, float x, float y)
    {
        panel_X = (int)x;
        panel_Y = Screen.height - (int)y;
        targetID = tID;
        disp = true;
    }
	
	void toggle(){
		if(disp) disp = false;
			else disp = true;
	}

    public void clearMenu(){
        disp = false;
    }
}
