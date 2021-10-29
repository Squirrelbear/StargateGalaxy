using UnityEngine;
using System.Collections;

public class methods : MonoBehaviour {
	private float shieldLevel = 58;
	private float hullLevel = 72;
	private int powerLevel = 400;
	private float exp;
	private int total_xp = 500;
	private int level = 3;
	private string shipname ="Promethius";
	private string text = "Message count ";
	private bool inc = false;
	private float clock;
	private int coltxt = 1;
			private string tName= "A big ship";
			private string tType= "ship";
			private string tFaction = "Ori";
			private int	tDistance = 594;
			private string tLevel ="BOSS";
			private float tshields = 30;
			private float thull = 90;
			private float shieldz;
	
	public int targetID = 0;
	    
	public int[,] wepData = {{0,0,10,5},{1,0,5,3},{2,0,20,6},{3,0,40,3},{4,0,6,7},{5,0,15,1},{8,0,2,21},{8,0,20,10},};
//	public string[] shipString = {"raider","big ship","raider","some other","fun ship","raider","raider","Llama"};
	public int[] shipDistance = {234,456,6786,456,789,53,678,423};
	
		

	
	// Use this for initialization
	void Start () {
		
		clock = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {
	controlPanel getStats = gameObject.GetComponent<controlPanel>();
	shieldz = getStats.getShields();
		
	displayMessage sendTxt = gameObject.GetComponent<displayMessage>();	
		if(coltxt <2)
			coltxt++;
		else coltxt =1;
		if(((Time.time - clock) >2)& (shieldz>20)){
			if(targetID<6) targetID++;
				else targetID =0;
			sendTxt.texter(text+clock+" color "+coltxt,coltxt);
			clock = Time.time;	
		}
		
		
		if(shieldLevel <0) inc =false;
		if(shieldLevel >99) inc = true;
		
		if(inc)shieldLevel--;
			else shieldLevel++;
		
		hullLevel = shieldLevel;
		exp = shieldLevel;
	}
	public void incWep(int i){
			wepData[i,1]++;
	}
	public int getTargetID(){
		return targetID;
	}
	public void decWep(int i){
			wepData[i,1]--;
	}
	public int getEnr(int i){
		return wepData[i,2];
	}
	public int getCnt(int i){
		return wepData[i,3];
	}
	
	public float shieldVal(){
		return shieldLevel;
	}
	
	public float hullVal(){
		return hullLevel;
	}
	
	public int powerVal(){
		return powerLevel;
	}
	
	public int totalXp(){
		return total_xp;
	}
	
	public float getXP(){
		return exp;
	}
	
	public string shipName(){
		return shipname;
	}
	public int getLevel(){
		return level;
	}
	
	public string getText(){
		return text;
	}
	public int targetDistance(){
		return tDistance;
	}
	
	public string targetName(){
		return tName;
	}
	public string targetFaction(){
		return tFaction;
	}
	public string targetType(){
		return tType;
	}
	public string targetLevel(){
		return tLevel;
	}
	public float targetHull(){
		return thull;
	}
	public float targetShields(){
		return tshields;
	}
}
