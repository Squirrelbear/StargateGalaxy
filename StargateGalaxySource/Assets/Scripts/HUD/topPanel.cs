using UnityEngine;
using System.Collections;


/***********************************************************           
    * Top Overlay: Displays the top panel witch displays   *
    * The ingame map, Trade menu, and Earth Menu           *   
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  A Large panel that is capable of displaying*
    * The trade menu, map menu and earth menu as needed    *
    *                                                      *   
    * Usage:                                               *   
    *      This script provides a large drop down panel    *
    *      realative to the current screen size,           *
    *      that when called can display the earth menu,    *
    *      trade window or Map menu.  It can only display  *
    *      one menu at a time, the earth menu is only      *
    *      accessable when the user is at earth, trade     *
    *      when the user is at the trade planet but map can*
    *      be called at any time by pressing the 'M' key   *
    ********************************************************/

public class topPanel: MonoBehaviour {

//Script assets
    public Texture mapGUIimg;


    //panels
    
	private Rect[] main_view;
    private Rect[] planetGrid;
		
//size and coordinante varables
	private int panel_Width = Screen.width;
	private int panel_Height = (int)(Screen.width/2);
	private int panel_X = Screen.width/11;
	private int panel_Y = ((Screen.width / 40) - (Screen.width / 2));
    private bool canSlide = true;

	void Start () {
	}
	
    void Update(){
        mapPanel mapDisp = gameObject.GetComponent<mapPanel>();
        if (Input.GetKeyDown("m"))
        {
            panelDisplay();
            mapDisp.panelDisplay();
        }
        if (Input.GetKeyDown("n")) panelDisplay();
        if (Input.GetKeyDown("t")) panelDisplay();
    }

    void OnGUI(){
        Rect main_view = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
        Rect planetGrid = new Rect((int)(panel_Width / 3.42), (int)(panel_Height / 22), (int)(panel_Width / 1.887), (int)(panel_Height / 1.135));
       
        panelSlide();

        GUI.BeginGroup(main_view, mapGUIimg);
		GUI.EndGroup();
    }


    //slides the panel in our out depending on the isSlide
    void panelSlide(){
        if (canSlide & panel_Y > ((Screen.width / 40) - (Screen.width / 2)))panel_Y = panel_Y - 12;
        if (!canSlide & panel_Y < 0) panel_Y = panel_Y + 12;
    }

    //isSlides scroll enabler
    void panelDisplay()
    {
        weaponsPanel moveweapP = gameObject.GetComponent<weaponsPanel>();
        objectPanel moveshipP = gameObject.GetComponent<objectPanel>();

        moveshipP.mapAction();
        moveweapP.mapAction();

        if (canSlide) canSlide = false;
        else canSlide = true;
    }

    public void trade()
    {
        tradePanel tradeDisp = gameObject.GetComponent<tradePanel>();
        tradeDisp.panelDisplay();
        panelDisplay();
    }

    public void homeMenu()
    {
        earthPanel homeDisp = gameObject.GetComponent<earthPanel>();
        homeDisp.panelDisplay();
        panelDisplay();
    }
    public void mapMenu()
    {
         mapPanel mapDisp = gameObject.GetComponent<mapPanel>();
         mapDisp.panelDisplay();
         panelDisplay();
    }
}