using UnityEngine;
using System.Collections;



/***********************************************************           
    * player info: Displays the players ship name, level   *
    * and current XP                                       *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Displays the players ship name, level      *
    * and current XP                                       *
    *                                                      *   
    * Usage:                                               *   
    *      This script displays a small panel that hides   *
    *      behind the ship control panel untill the user   *
    *      selects 'i' button where it will scroll up      *
    *      into view.  The panel displays the players      *
    *      ship name, level and current XP                 *
    ********************************************************/

public class playerInfoPanel : MonoBehaviour {

//Script assets
    public Texture playerIMG;
	public Texture xp_barIMG;
	public GUIStyle text;

//size and coordinante varables
	private int panel_Width = (int)(Screen.width/4.55);
	private int panel_Height = (int)(Screen.width/5.55);
	private int panel_X = (int)(Screen.width/1.589);
	private int panel_Y = Screen.height - (int)(Screen.width/9);
		
//Other variables
   	private bool isSlide = true;
	private Rect[] playerinfoGUI;
	private float exp;
	private int totalXp;
	private int level;
	private string ship;
	private float xp_bits;
    private GameController game;

    void Start(){
		gameData();
		xp_bits = ((float)(panel_Width/1.04)/totalXp);
		text.fontSize = Screen.width/100;
    }


    void Update(){
		gameData();
        if (Input.GetKeyDown("i")) slideToggle();

    }


    void OnGUI(){
		GUI.depth =3;
        slide();

        Rect playerinfoGUI = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
       	GUI.BeginGroup(playerinfoGUI, playerIMG);
			xp_bar();
			shipDetails();
		GUI.EndGroup();

    }


    //toggles scroll enabler
    void slideToggle(){
		
        if (isSlide) isSlide = false;
        else isSlide = true;
    }

    //slides the panel in our out depending on the toggle
    void slide(){
		
        if (isSlide & panel_Y < (Screen.height - (int)(Screen.width/9))) panel_Y = panel_Y + 3;
        if (!isSlide & panel_Y > (Screen.height - (int)(Screen.width/6.1))) panel_Y = panel_Y - 3;
    }
	
	public void mapAction(){
		isSlide = true;
	}
	void shipDetails(){
		GUI.Label(new Rect(panel_Width-(int)(panel_Width/1.24), (int)(panel_Height/15), panel_Width,panel_Height),ship,text);
		GUI.Label(new Rect(panel_Width-(int)(panel_Width/1.24), (int)(panel_Height/6.8), panel_Width,panel_Height),""+(level+1),text);
	
	}
	void xp_bar(){
		
		GUI.DrawTexture(new Rect(panel_Width-(int)(panel_Width/1.016), (int)(panel_Height/3.247),(xp_bits*exp),panel_Width/70),xp_barIMG, ScaleMode.ScaleAndCrop, true, 0f);
		GUI.Label(new Rect(panel_Width-(int)(panel_Width/1.09), (int)(panel_Height/4.4), panel_Width,panel_Height),exp+" / "+ totalXp,text);	
	}
	
	void gameData(){

        //methods getData = gameObject.GetComponent<methods>();	
        if (game == null) game = (GameController)GameObject.Find("GameController").GetComponent("GameController");
        totalXp = (int)(100 * Mathf.Exp(game.player.ships[game.player.currentShip].shipLevel)); //getData.totalXp();
        exp = game.player.ships[game.player.currentShip].shipExp; // getData.getXP();
        level = game.player.ships[game.player.currentShip].shipLevel; // getData.getLevel();
        ship = game.shipDatabase.ships[game.player.ships[game.player.currentShip].shipID].shipName;//getData.shipName();
	}
	
}
