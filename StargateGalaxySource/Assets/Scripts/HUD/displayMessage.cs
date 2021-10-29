using UnityEngine;
using System.Collections;

/***********************************************************           
    * Screen Messages: Displays all combat and warning     *
    * text on screen                                       *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    * Purpose:  Display all ingame warnings/messages       *   
    *                                                      *   
    * Usage:                                               *   
    *      This script displays all ingame warnings and    *
    *      messages at the top of the screen in colord     *
    *      scrolling text.  The script is activated when   *
    *      called apon by another method and displays      *
    *      the supplied string.  the text scrolls out of   *
    *      view and deletes itself after a set time        *
    ********************************************************/
public class displayMessage : MonoBehaviour {
	
	
//Script assets
    public GUIStyle msg;
    private Rect[] screenText;

		
//Other variables
	private bool scroll = false;
	private string dispText;
	private float clock;
	private int pos=0;
	private string[] holder = new string[6];
	private int[] txtColor = new int[6];
	private double[] slide = new double[6];
    private bool update = true;
    private bool newMessage = false;

	void Start () {
		msg.fontSize = 30;
	}
	
	void Update () {

	}
	
	void OnGUI(){
		
		if((Time.time - clock) >5){
			arrayMove();
			clock = Time.time;	
		}

        if (newMessage)
        {
            messJump();
            newMessage = false;
        }
        else
        {
            textSroll();
            
        }
            printText();

		
	}

	public void texter(string tmpTxt, int tmpColor){
        
        newMessage = true;
		holder[pos] = tmpTxt;
		txtColor[pos] = tmpColor;
		slide[pos] = 0;
		pos++;
		if(pos>4) arrayMove();
        messJump();
	}
	
	void arrayMove(){
		for(int i = 0; i<pos;i++){
			holder[i] = holder[i+1];
			txtColor[i] = txtColor[i+1];
			slide[i] = slide[i+1];
		}
		if(pos>0)pos--;
	}
	
	void setColor(int i){
		switch(i)
		{
		case 1: msg.normal.textColor = Color.red;
			break;
		case 2: msg.normal.textColor = Color.yellow;
			break;
		case 3: msg.normal.textColor = Color.green;
			break;
		case 4: msg.normal.textColor = Color.blue;
			break;
		default: msg.normal.textColor = Color.white;
			break;
		}
	}
	
	void printText(){
		for(int i = pos; i>=0;i--){		
			setColor(txtColor[i]);
			msg.fontSize = 30-((int)(slide[i]/10));
			Rect screenText = new Rect(0,(Screen.height/3)-(int)(Screen.height/4+(slide[i])),Screen.width, Screen.height/4);
			GUI.Label(screenText,holder[i],msg);
		}
        clock = Time.time;
	}
	
	void textSroll(){
		
		for(int i = pos; i>=0;i--){
			slide[i]=slide[i]+0.3;			//controlls the scroll speed and text shrink speed
		}
		slide[pos]=0.0;
	}
    void messJump()
    {

        for (int i = pos; i >= 0; i--)
        {
            slide[i] = slide[i] + 20;			//controlls the scroll speed and text shrink speed
        }
        slide[pos] = 0.0;
    }
}
