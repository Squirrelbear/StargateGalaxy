using UnityEngine;
using System.Collections;


/***********************************************************           
    *Ship Control Panel: Main ingame control panel         *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Allows to user to view and control all ship*
    * functions                                            *   
    *                                                      *   
    * Usage:                                               *   
    *      This script draws the main control panel at the *
    *      bottom of the screen.  The panel is populated   *
    *      with all necisary controlls for the user to     *
    *      command their current ship.  All data is        *
    *      activly imported from the main game controler.  *
    *      This scripts main function is to display shit   *
    *      data from the game controller and send back user*
    *      commands.
    ********************************************************/

public class controlPanel: MonoBehaviour {
	
//Script assests.
public Texture actionBarIMG;
private Rect[] actionBarGUI;
public Texture Power_Bar;
public Texture shield_Bar;
public Texture hull_img;
public GUIStyle text;


public Texture[] recIcon = new Texture[7];
private string[] recName = { "Large Power Core", "Small Power Core", "Ori Weapon Parts", "Wraith Weapon Parts", "Goa'uld Weapon Parts", "Unknown Weapon Parts", "Ship Scraps" };

	
//Script variables
//size and coordinante varables
	private int panel_Width = (int)(Screen.width/1.11);
	private int panel_Height = (int)(Screen.width/5.3);
	private int panel_X = (int)(Screen.width/20);
	private int panel_Y = Screen.height - (int)(Screen.width/6);
	

		
//Other variables
    private float[] enrgyVals = { 0.0F, 0.0F, 0.0F };
    private int[] sliderPosArray = new int[4];
    private int slider_X;
    private int slider_Y;

    private int[] enrgyLablePos = new int[6];
    private int[] enrgyTexturePos = new int[6];
    private int energyTexture_X;
    private int mainEnergyTexture_X;
    private int mainEnergyTexture_Y;

    private int[] targetLablePos = new int[7];

    private int[] resourceIconPos = new int[9];
    private int resourceBox;

	private float totalHull;
	private float totalShield;
	private float shieldL;				//ship shield status
	private float hullL;				//ship shield status
    private float shieldT;
	private int total_power;			//total power available for the ship

            

			
	private	float Mpower_Bits = 0;				//division of power units for the Main power bar
	private float Npower_Bits = 0;				//division of power units for the normal power bars
			
	private float Senergy_Bits = 100;			//division of energy units for the sheild bar
	private float power_usage;					//value of total power used buy the ship (used for the power bar and numeric display)
	private float tmp;							//a temp float varaible
			
	private string targetName;
	private string targetType;
	private string targetFaction;
	private string targetLevel;
	private int	targetDistance;
	private int targetShields;
	private int targetHull;


    void Start()
    {
       objPosAssign();                                         //Assigns all the Relative positions for all panel objects
		gameData();	                                            //collects external data
        text.fontSize = Screen.width/130;                       //sets the font size of the text style
        mainEnergyTexture_Y = (int)(-(panel_Height / 3.9));
        Npower_Bits = (float)(panel_Height / 3) / 100;		        //Finds the division for power units for the normal power bars

	}


	/*Update is called once per frame, used to update dynamic variables*/
	void Update () {
        mainPowerUpdate();
        gameData();	                                                                            //collects and updates external data
	}

    /*Main calling method for all GUI objects*/
    void OnGUI(){

		GUI.depth =2;                                                                   //Sets the depth of the GUI
        Rect actionBarGUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);      //Sets the size and positions of the GUI panel
		
        GUI.BeginGroup(actionBarGUI, actionBarIMG);                                     //The panel that holds all the controll objects and draws the panel texture
		    sliderCheck();      //Displays the energy sliders and checks their values
		    lableUpdate();      //Updates all the energy Labels
		    shieldStatus();     //Updates all shield details and status bar
		    hull();             //Updates all Hull details and the status image
		    target();           //Updates all target details
            resource();         //Draws and updates all resource details
		GUI.EndGroup();                                                                 //closes the gui group
    }
	
	
	/*Constantly checks for slider movement and limits the slider controlls to the maximum available power*/
    void sliderCheck()
    {
        ShipAI shipData = (ShipAI)GameObject.FindGameObjectWithTag("Player").GetComponent("ShipAI");
        for (int i = 0; i < 3; i++)
        {

            tmp = enrgyVals[i];																										            //Stores the current value of the slider.
            enrgyVals[i] = GUI.VerticalSlider(new Rect(sliderPosArray[i], sliderPosArray[3], slider_X, slider_Y), enrgyVals[i], 100, 0.0F);     //Draws the slider
            if (i == 1)
            {
                if ((enrgyVals[0] + enrgyVals[1] + enrgyVals[2]) > 100) 																	//check to see if power is available
                    enrgyVals[i] = GUI.VerticalSlider(new Rect(sliderPosArray[i], sliderPosArray[3], slider_X, slider_Y), tmp, 100, 0.0F); 	        //if no power use the previous value
                else
                enrgyVals[i] = GUI.VerticalSlider(new Rect(sliderPosArray[i], sliderPosArray[3], slider_X, slider_Y), shipData.checkWeaponPower(enrgyVals[i]), 100, 0.0F); //if power update to new value
            } else { 
             if ((enrgyVals[0] + enrgyVals[1] + enrgyVals[2]) > 100) 																	//check to see if power is available
                    enrgyVals[i] = GUI.VerticalSlider(new Rect(sliderPosArray[i], sliderPosArray[3], slider_X, slider_Y), tmp, 100, 0.0F); 	        //if no power use the previous value
                else
                    enrgyVals[i] = GUI.VerticalSlider(new Rect(sliderPosArray[i], sliderPosArray[3], slider_X, slider_Y), enrgyVals[i], 100, 0.0F); //if power update to new value
            }
        }
        shipData.updatePowerStatus((int)enrgyVals[0], (int)enrgyVals[1], (int)enrgyVals[2]);
    }
	
	/*Draws and updates all lables and Power bar textures*/
	void lableUpdate(){
        ShipAI shipData = (ShipAI)GameObject.FindGameObjectWithTag("Player").GetComponent("ShipAI");


        GUI.Label(new Rect(enrgyLablePos[4], enrgyLablePos[5], panel_Width, panel_Height), "" + (int)(shipData.maxPower-shipData.usedPower), text);   //Updates the main power Lable
        GUI.DrawTexture(new Rect(enrgyTexturePos[4], enrgyTexturePos[5], mainEnergyTexture_X, mainEnergyTexture_Y), Power_Bar, ScaleMode.ScaleAndCrop, true, 0f);           //Draws the main power bar


            GUI.Label(new Rect(enrgyLablePos[3], enrgyLablePos[0], panel_Width, panel_Height), "" + (int)shipData.shieldPower, text);                                               //Updates all power bar lables
            GUI.DrawTexture(new Rect(enrgyTexturePos[0], enrgyTexturePos[3], energyTexture_X, -Npower_Bits * enrgyVals[0]), Power_Bar, ScaleMode.ScaleAndCrop, true, 0f);   //Draws each power bar    

            GUI.Label(new Rect(enrgyLablePos[3], enrgyLablePos[1], panel_Width, panel_Height), "" + (int)shipData.weaponPower, text);                                               //Updates all power bar lables
            GUI.DrawTexture(new Rect(enrgyTexturePos[1], enrgyTexturePos[3], energyTexture_X, -Npower_Bits * enrgyVals[1]), Power_Bar, ScaleMode.ScaleAndCrop, true, 0f);   //Draws each power bar    

            GUI.Label(new Rect(enrgyLablePos[3], enrgyLablePos[2], panel_Width, panel_Height), "" + (int)shipData.speedPower, text);                                               //Updates all power bar lables
            GUI.DrawTexture(new Rect(enrgyTexturePos[2], enrgyTexturePos[3], energyTexture_X, -Npower_Bits * enrgyVals[2]), Power_Bar, ScaleMode.ScaleAndCrop, true, 0f);   //Draws each power bar    

        
	}
	
	/*Controlls the shield level status bar and text*/
	void shieldStatus(){
        float shieldPercent = 100 / totalShield;
        if ((shieldL * shieldPercent) > 70) GUI.color = Color.green;
        else if ((shieldL * shieldPercent) > 30) GUI.color = Color.yellow;
		else GUI.color = Color.red;
		
		GUI.DrawTexture(new Rect(panel_Width-(int)(panel_Width/3.086), (int)(panel_Height/2.32), Senergy_Bits * shieldL,panel_Width/90),shield_Bar, ScaleMode.ScaleAndCrop, true, 0f);
		
		
		GUI.color = Color.black;
		GUI.Label(new Rect(panel_Width-(int)(panel_Width/3.09), (int)(panel_Height/2.32), panel_Width,panel_Height),Mathf.RoundToInt(shieldL*shieldPercent) +"%",text);
		GUI.color = Color.white;
        GUI.Label(new Rect(panel_Width - (int)(panel_Width / 6.39), (int)(panel_Height / 1.357), panel_Width, panel_Height), Mathf.RoundToInt(shieldL) + "Eu", text);
	}
	
	void hull(){
        float hullPercent = 100 /totalHull;
        if ((hullL * hullPercent) > 70) GUI.color = Color.green;
        else if ((hullL * hullPercent) > 30) GUI.color = Color.yellow;
		else GUI.color = Color.red;

		GUI.DrawTexture(new Rect(panel_Width-(int)(panel_Width/3.1),(int)(panel_Height/1.95),(int)(panel_Width/7),(int)(panel_Height/3.2)), hull_img, ScaleMode.ScaleAndCrop, true, 0f);
	
		GUI.color = Color.black;
		GUI.Label(new Rect((int)(panel_Width-(panel_Width/4)),panel_Height-(int)(panel_Height/2.7),panel_Width,panel_Height),"Hull "+Mathf.RoundToInt( hullL*hullPercent) +"%",text);
		GUI.color = Color.white;
        GUI.Label(new Rect(panel_Width - (int)(panel_Width / 6.39), (int)(panel_Height / 1.66), panel_Width, panel_Height), Mathf.RoundToInt(hullL)+ "Eu", text);
	}
	
	void target(){
		GUI.color = Color.white;
        GUI.Label(new Rect(targetLablePos[0], targetLablePos[3], panel_Width, panel_Height), targetName, text);
        GUI.Label(new Rect(targetLablePos[1], targetLablePos[4], panel_Width, panel_Height), targetType, text);
        GUI.Label(new Rect(targetLablePos[1], targetLablePos[5], panel_Width, panel_Height), targetLevel, text);
        GUI.Label(new Rect(targetLablePos[1], targetLablePos[6], panel_Width, panel_Height), targetFaction, text);
        GUI.Label(new Rect(targetLablePos[2], targetLablePos[4], panel_Width, panel_Height), targetDistance + "km", text);
        GUI.Label(new Rect(targetLablePos[2], targetLablePos[5], panel_Width, panel_Height), targetShields + "", text);
        GUI.Label(new Rect(targetLablePos[2], targetLablePos[6], panel_Width, panel_Height), targetHull + "", text);
	}

    void mainPowerUpdate()
    {
        ShipAI shipData = (ShipAI)GameObject.FindGameObjectWithTag("Player").GetComponent("ShipAI");
        Mpower_Bits = (float)(panel_Height / 3.9) / shipData.maxPower; 	//Finds the division for power units for the main power bar
        power_usage = (float)(Mpower_Bits) * shipData.usedPower;    //Determins the current power usage
        mainEnergyTexture_Y = (int)(-(panel_Height / 3.9) + power_usage);
    }       
    void resource(){
        GameController game = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");


        for (int i = 0; i < 7; i++)
        {
            GUI.Button(new Rect(resourceIconPos[i], resourceIconPos[7], resourceBox, resourceBox), new GUIContent(recIcon[i], recName[i]), text);
            GUI.Label(new Rect(resourceIconPos[i], resourceIconPos[8], resourceBox, resourceBox), game.player.resources[i] + "", text);
        }

        GUI.Label(new Rect((int)(panel_Width / 2.6), (int)(panel_Height / 1.55), panel_Width / 40, panel_Width / 20), GUI.tooltip,text);
    }


	/*Collects external data from external scripts (see peters code)*/
	void gameData(){
        ShipAI shipData = (ShipAI)GameObject.FindGameObjectWithTag("Player").GetComponent("ShipAI");

        shieldT = shipData.maxShield;
        shieldL = shipData.shield;
        hullL = shipData.hull;
        total_power = shipData.maxPower;
        totalHull = shipData.maxHull;
        totalShield = shipData.maxShield;
       
        Senergy_Bits = (float)(panel_Width / 8.4) / shieldT;        //Finds the division for power units for the shield energy bar

        if (shipData == null || shipData.target == null)
        {
            targetName = "No Target";
            targetType = "";
            targetFaction = "";
            targetDistance = 0;
            targetLevel = "";
            targetHull = 0;
            targetShields = 0;
            return;
        }

        if (shipData.target.tag == "Ally" || shipData.target.tag == "Enemy")
        {
            ShipAI targetData = (ShipAI)shipData.target.GetComponent("ShipAI");

            targetName = targetData.shipData.shipName;
            targetType = shipData.target.tag;
            targetFaction = targetData.race.ToString();
            targetDistance = (int)Vector3.Distance(shipData.gameObject.transform.position, targetData.gameObject.transform.position);

            if (targetData.isBoss)
            {
                targetLevel = "BOSS";
            }
            else
            {
                targetLevel = "" + (targetData.shipLevel + 1);
            }
            
            targetHull = targetData.hull;
            targetShields = targetData.shield;
        }

        if (shipData.target.tag == "Planet" || shipData.target.tag == "Supergate" || shipData.target.tag == "Homeworld" || shipData.target.tag == "Tradeworld")
        {
            targetName = shipData.target.tag.ToString();
            targetType = (shipData.target.tag == "Supergate") ? "Supergate" : "Planet";
            targetFaction = shipData.game.systems[shipData.controller.systemID].planets[shipData.controller.planetID].controller.ToString();
            targetDistance = (int)Vector3.Distance(shipData.gameObject.transform.position, shipData.target.transform.position);
            targetLevel = "";
            targetHull = 0;
            targetShields = 0;
            return;
        }

        if (shipData.target.tag == "WreckedShip")
        {
            targetName = "Wrecked Ship";

            LootableShip lootInfo = shipData.target.GetComponent<LootableShip>();
            if (lootInfo.isLooted())
                targetType = "Unlooted Ship";
            else
                targetType = "Looted Ship";
            targetFaction = "";
            targetDistance = (int)Vector3.Distance(shipData.gameObject.transform.position, shipData.target.transform.position);
            targetLevel = "";
            targetHull = 0;
            targetShields = 0;
        }
	}

    void objPosAssign()
    {
        ShipAI shipData = (ShipAI)GameObject.FindGameObjectWithTag("Player").GetComponent("ShipAI");
        
        enrgyVals[0] = (int)(shipData.shieldPower * 100.0 / shipData.maxPower);
        enrgyVals[1] = (int)(shipData.weaponPower * 100.0 / shipData.maxPower);
        enrgyVals[2] = (int)(shipData.speedPower * 100.0 / shipData.maxPower);

        //Slider positions
        slider_X = panel_Width / 70;
        slider_Y = panel_Height / 3;
        sliderPosArray[0] = (int)(panel_Width / 5.7);
        sliderPosArray[1] = (int)(panel_Width / 4.75);
        sliderPosArray[2] = (int)(panel_Width / 4.15);
        sliderPosArray[3] = (int)(panel_Height / 2.05);

        //energy slider label positions
        enrgyLablePos[0] = (int)(panel_Height / 1.97);
        enrgyLablePos[1] = (int)(panel_Height / 1.59);
        enrgyLablePos[2] = (int)(panel_Height / 1.36);
        enrgyLablePos[3] = (int)(panel_Width / 3.3);
        enrgyLablePos[4] = (int)(panel_Width / 9.5);
        enrgyLablePos[5] = (int)(panel_Height / 1.50);
        //energy slider Texture positions
        energyTexture_X = panel_Width / 90;
        mainEnergyTexture_X = panel_Width / 50;
        mainEnergyTexture_Y = -(int)((panel_Height / 3.9) + power_usage);
        enrgyTexturePos[0] = (int)(panel_Width / 5.206);
        enrgyTexturePos[1] = (int)(panel_Width / 4.476);
        enrgyTexturePos[2] = (int)(panel_Width / 3.935);
        enrgyTexturePos[3] = (int)(panel_Height / 1.21);
        enrgyTexturePos[4] = (int)(panel_Width / 7.42);
        enrgyTexturePos[5] = (int)(panel_Height / 1.29);

        //Target label positions
        targetLablePos[0] = (int)(panel_Width / 2.11);
        targetLablePos[1] = (int)(panel_Width / 2.5);
        targetLablePos[2] = (int)(panel_Width / 1.78);
        targetLablePos[3] = (int)(panel_Height / 7.5);
        targetLablePos[4] = (int)(panel_Height / 3.01);
        targetLablePos[5] = (int)(panel_Height / 2.4);
        targetLablePos[6] = (int)(panel_Height / 1.97);

        //Resource icon positions
        resourceBox = panel_Width / 45;
        resourceIconPos[0] = (int)(panel_Width / 2.6);
        resourceIconPos[1] = (int)(panel_Width / 2.42);
        resourceIconPos[2] = (int)(panel_Width / 2.26);
        resourceIconPos[3] = (int)(panel_Width / 2.12);
        resourceIconPos[4] = (int)(panel_Width / 1.99);
        resourceIconPos[5] = (int)(panel_Width / 1.88);
        resourceIconPos[6] = (int)(panel_Width / 1.78);
        resourceIconPos[7] = (int)(panel_Height / 1.43);
        resourceIconPos[8] = (int)(panel_Height / 1.25);
    }

	public float getShields(){
        return enrgyVals[0];	
	}
	
	public float getWeapons(){
        return enrgyVals[1];	
	}
	
	public float getEngines(){
        return enrgyVals[2];	
	}
	
	
}
