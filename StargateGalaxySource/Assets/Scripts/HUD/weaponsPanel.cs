using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***********************************************************           
    * Weapons Panel: Displays the ingame weapons panel     *
    * and available weapons                                *   
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Allow the user to view and controll        *
    * available weapons                                    *   
    *                                                      *   
    * Usage:                                               *   
    *      This script draws the left weapons panel        *
    *      realative to the current screen size, the panel *
    *      scrolls out from the left side of the screen    *
    *      when the user selects the 'W' key and scrolls   *
    *      back and pressed again or when the Top overlay  *
    *      panel is called. All available weapons are      *
    *      displayed in a list with the ability to         *
    *      enable/disable each weapon and allocate how     *
    *      many of each weapon is active.                  *   
    ********************************************************/

public class weaponsPanel : MonoBehaviour
{
//Script assets
    public Texture weaponsBarIMG;
		
	public Texture Asg_Beam;
	public Texture Ori_Beam;
	public Texture Asg_Ener;
	public Texture Goa_Ener;
	public Texture Wra_EneB;
	public Texture Wra_EneS;
	public Texture Hum_pro;
	public Texture Hum_Roc;
	public Texture Asg_Beam2;
	public Texture Ori_Beam2;
	public Texture Asg_Ener2;
	public Texture Goa_Ener2;
	public Texture Wra_EneB2;
	public Texture Wra_EneS2;
	public Texture Hum_pro2;
	public Texture Hum_Roc2;
	public Texture empty;
	private Texture[] skins = new Texture[8];	
		
	public GUIStyle upButton;
	public GUIStyle dwnButton;
	public GUIStyle statText;	
	public GUIStyle lablText;
	
//size and coordinante varables
	private int panel_Width = Screen.height / 4;
	private int panel_Height = (int)(Screen.height/1.7);
	private int panel_X = 0 - (Screen.height/4 - Screen.height/60);
	private int panel_Y = Screen.height / 9;
		
//Other variables
   	private bool isSlide = true;
	private Rect[] weaponsBarGUI;
    private int[,] wepInfo;
		
	private bool[] toggleB = new bool[8];
	private string[] wepName = {"Asgard Beam","Ori Beam","Asgard Energy","Goa'uld Energy","Wraith Heavy Energy","Wraith Light Energy","Projectile","Rocket"};
	private double [,] labLoc = {{6.5,12.5,8.9},{3.8,5.3,4.55},{2.7,3.4,3.1},{2.1,2.45,2.269},{1.715,1.95,1.83},{1.45,1.63,1.55},{1.26,1.39,1.33},{1.115,1.215,1.168}};
	private double[] butLoc = {15,5.7,3.55,2.55,2,1.66,1.41,1.23};
    private int[] elementPos = new int[11];
    public ShipAI shipData;

    void Start(){
        objPosAssign();
        wepInfo = new int[8, 4];
    }

    void Update(){
		statText.fontSize = panel_Width/20;
		lablText.fontSize = panel_Width/21;
		getData();
		buttonSet();
        if (Input.GetKeyDown("w")) slideToggle();

    }

    void OnGUI(){
        slide();

        Rect weaponsBarGUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
       	GUI.BeginGroup(weaponsBarGUI, weaponsBarIMG);
		    weaponsMAIN();
		    weaponsSUB();
		GUI.EndGroup();

    }


	void weaponsMAIN(){
		for(int i=0;i<8;i++){
			if(wepInfo[i,0]!=8){
                if (GUI.Button(new Rect(elementPos[0], (int)(panel_Height / butLoc[i]), elementPos[1], elementPos[2]), skins[i], statText))
                {
					if(toggleB[i]) toggleB[i] = false;
						else toggleB[i] = true;
				}

                if (toggleB[i])
                    lablText.normal.textColor = new Color(0 / 255, 255 / 255, 255 / 255);
                else
                {
                    shipData.disableWeapon((wepInfo[i, 0] > 5) ? wepInfo[i, 0] + 1 : wepInfo[i, 0]);
                    lablText.normal.textColor = new Color(0.1f, 0.6f, 0.4f);
                }

                GUI.Label(new Rect(elementPos[3], (int)(panel_Height / butLoc[i]), elementPos[4], elementPos[5]), wepName[wepInfo[i, 0]], lablText);
			}
		}
	}

	void weaponsSUB(){

        GUI.Label(new Rect(elementPos[0], elementPos[6], panel_Width, panel_Height), "Energy " + (shipData.weaponPower - shipData.usedWeaponPower), statText);
		for(int i =0; i<8;i++){
			if(wepInfo[i,0]!=8){
                GUI.Label(new Rect(elementPos[7], (int)(panel_Height / labLoc[i, 0]), panel_Width, panel_Height), "E " + wepInfo[i, 1], statText);
                GUI.Label(new Rect(elementPos[8], (int)(panel_Height / labLoc[i, 0]), panel_Width, panel_Height), "A " + wepInfo[i, 2], statText);
                GUI.Label(new Rect(elementPos[9], (int)(panel_Height / labLoc[i, 0]), panel_Width, panel_Height), "F " + wepInfo[i, 3], statText);

                if (GUI.Button(new Rect(elementPos[10], (int)(panel_Height / labLoc[i, 1]), elementPos[0], elementPos[0]), "", upButton))
                    if (toggleB[i]) shipData.enableWeapon((wepInfo[i, 0] > 5) ? wepInfo[i, 0] + 1 : wepInfo[i, 0]);

                if (GUI.Button(new Rect(elementPos[10], (int)(panel_Height / labLoc[i, 2]), elementPos[0], elementPos[0]), "", dwnButton))
                    shipData.disableWeapon((wepInfo[i, 0] > 5) ? wepInfo[i, 0] + 1 : wepInfo[i, 0]);
		
			}
		}
	}
	
	void getData(){

        if (shipData == null) shipData = (ShipAI)GameObject.FindGameObjectWithTag("Player").GetComponent("ShipAI");
        if (shipData == null) return;

        int i = 0;
        foreach (ShipAI.EquippedWeapons e in shipData.equippedWeapons)
        {
            wepInfo[i, 0] = (e.weaponData.weaponID > 6) ? e.weaponData.weaponID - 1 : e.weaponData.weaponID;
            wepInfo[i, 1] = e.weaponData.powerValue;
            wepInfo[i, 2] = e.weaponCount;
            wepInfo[i, 3] = shipData.getNumberOfFiring(e.weaponData.weaponID);
            i++;
        }

        for (; i < 8; i++)
        {
            wepInfo[i, 0] = 8;
        }
	}
	void buttonSet(){
		
		for(int i =0; i<8;i++){
			 switch(wepInfo[i,0]){
				case 0: 
				if(toggleB[i]) skins[i] = Asg_Beam2;
				else skins[i] = Asg_Beam;
					break;
				case 1: 
				if(toggleB[i])skins[i] = Ori_Beam2;
				else skins[i] = Ori_Beam;
					break;
				case 2: 
				if(toggleB[i])skins[i] = Asg_Ener2;
				else skins[i] = Asg_Ener;
					break;
				case 3: 
				if(toggleB[i])skins[i] = Goa_Ener2;
				else skins[i] = Goa_Ener;
					break;
				case 4: 
				if(toggleB[i])skins[i] = Wra_EneB2;
				else skins[i] = Wra_EneB;
					break;
				case 5: 
				if(toggleB[i])skins[i] = Wra_EneS2;
				else skins[i] = Wra_EneS;
					break;
				case 6: 
				if(toggleB[i])skins[i] = Hum_pro2;
				else skins[i] = Hum_pro;
					break;
				case 7: 
				if(toggleB[i])skins[i] = Hum_Roc2;
				else skins[i] = Hum_Roc;
					break;
				case 8: 
				skins[i] = empty;
					break;
				default:
					break;
				}
		}

	}
		
	public void mapAction(){
		isSlide = true;
	}
	
	void slideToggle(){
        if (isSlide) isSlide = false;
        else isSlide = true;
    }

    void slide(){	
        if (isSlide & panel_X > (0 - (Screen.height/4 - Screen.height/60))) panel_X = panel_X - 3;
        if (!isSlide & panel_X < 0) panel_X = panel_X + 3;
    }

    void objPosAssign()
    {
        elementPos[0] = (int)(panel_Width / 12);
        elementPos[1] = (int)(panel_Width/1.8);
        elementPos[2] = (int)(panel_Height/10);
        elementPos[3] = (int)(panel_Width/7);
        elementPos[4] = (int)(panel_Width/3);
        elementPos[5] = (int)(panel_Height/11.2);
        elementPos[6] = (int)(panel_Height/50);
        elementPos[7] = (int)(panel_Width / 3.5);
        elementPos[8] = (int)(panel_Width / 2.2);
        elementPos[9] = (int)(panel_Width/1.8);
        elementPos[10] = (int)(panel_Width / 1.4);

    }
}