using UnityEngine;
using System.Collections;

public class tradePanel : MonoBehaviour {

//Script assets
    public Vector2 scrollPos = Vector2.zero;

    private DataManager.DamageType weaponDmg;
    //images of ships
    public Texture[] shipIMGS = new Texture[3];
    //Resource icons
    public Texture[] recIcon = new Texture[7];
    public Texture[] wepImg = new Texture[9];

    //panels
    
	private Rect[] main_view;
    private Rect[] planetGrid;

    //styles
    public GUIStyle text;
    public GUIStyle text2;
    public GUIStyle bStyle;
    public GUIStyle lStyle;
    public GUIStyle lStyle2;
        
	
//Script variables
		
////size and coordinante varables
	private int panel_Width = Screen.width;
	private int panel_Height = (int)(Screen.width/2);
	private int panel_X = Screen.width/11;
	private int panel_Y = ((Screen.width / 40) - (Screen.width / 2));
	

//Other varaibles
    private int[] resourceIconPos = new int[11];
    private int resourceBox;
    private int[] purchaseButtonPos = new int[4];
    private int[] tradeButtonPos = new int[8];
    private int[] scrollBoxPos = new int[5];
    private int[] earthLabelPos = new int[14];     
    private int[] resourceVal = new int[7];
    private bool canSlide = true;

    private string[] shipName = { "Promethius", "Daedalus", "Asgard Mothership" };
    private string[] recName = { "Large Power Core", "Small Power Core", "Ori Weapon Parts", "Wraith Weapon Parts", "Goa'uld Weapon Parts", "Unknown Weapon Parts", "Ship Scraps" };
     private string[] shipString = {"Daedalus", "Asgard Mothership" };         
    private int pSelection;
    private int menuType = 0;
    private int startPos;

    private int selGridInt = 1;
    private int ActiveGridInt = 1;

    private string[] weaponString;
    private int[] weaponID;
    private bool weapOrShip = true;
    private int shipSelect;
    private int weaponSelect;


	void Start () {
        objPosAssign();
        
        text.fontSize = Screen.width / 130;
        text2.fontSize = Screen.width / 40;
        bStyle.fontSize = Screen.width / 100;
        lStyle.fontSize = Screen.width / 80;
        lStyle2.fontSize = Screen.width / 80;
        startPos = panel_Y;

	}
	

    void Update(){
        ActiveGridInt = selGridInt;

        if (Input.GetKeyDown("t")) panelDisplay();
    }

    void OnGUI(){
        Rect main_view = new Rect(panel_X, panel_Y, panel_Width, panel_Height);
        Rect planetGrid = new Rect((int)(panel_Width / 3.42), (int)(panel_Height / 22), (int)(panel_Width / 1.887), (int)(panel_Height / 1.135));
       
        panelslide();

        GUI.BeginGroup(main_view);
            tradeMenu();
            GUI.BeginGroup(planetGrid);      
                tradeMain();
            GUI.EndGroup();
        GUI.EndGroup();

    }

    void tradeMain()
    {
        GameController game = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");

        //item details
        //ships
        if (weapOrShip)
        {
            shipSelect = ActiveGridInt + 1;

            GUI.DrawTexture(new Rect(0, 0, earthLabelPos[12], earthLabelPos[13]), shipIMGS[shipSelect], ScaleMode.StretchToFill, true, 10.0f);

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[2], panel_Width, earthLabelPos[7])), "Ship Name:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[2], panel_Width, earthLabelPos[7])), game.shipDatabase.ships[shipSelect].shipName, lStyle2);

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[3], panel_Width, earthLabelPos[7])), "Ship Level:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[3], panel_Width, earthLabelPos[7])), "" + game.player.ships[shipSelect].shipLevel, lStyle2);

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[4], panel_Width, earthLabelPos[7])), "Ship Power level:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[4], panel_Width, earthLabelPos[7])), "" + game.shipDatabase.ships[shipSelect].AIStats[shipSelect].power, lStyle2);

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[5], panel_Width, earthLabelPos[7])), "Hull strength:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[5], panel_Width, earthLabelPos[7])), "" + game.shipDatabase.ships[shipSelect].AIStats[shipSelect].hull, lStyle2);

            resourceVal = game.shipDatabase.ships[shipSelect].resources;

            if (game.canBuyShip(shipSelect))
            {
                if (GUI.Button(new Rect(purchaseButtonPos[0], purchaseButtonPos[1], purchaseButtonPos[2], purchaseButtonPos[3]), "Purchase", bStyle))
                {
                    game.buyShip(shipSelect);
                }
            }
            else
            {
                GUI.Button(new Rect(purchaseButtonPos[0], purchaseButtonPos[1], purchaseButtonPos[2], purchaseButtonPos[3]), "Not Available", bStyle);
            }
        }
        else if (weaponID.Length > 0)
        {
            weaponSelect = weaponID[ActiveGridInt];
            resourceVal = game.shipDatabase.weapons[weaponSelect].resources;
            GUI.DrawTexture(new Rect(earthLabelPos[0], tradeButtonPos[3], earthLabelPos[12] / 2, earthLabelPos[13] / 2), wepImg[weaponSelect], ScaleMode.StretchToFill, true, 10.0f); //FIX THIS, JUST A TEMP ATM

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[2], panel_Width, earthLabelPos[7])), "Weapon Name:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[2], panel_Width, earthLabelPos[7])), game.shipDatabase.weapons[weaponSelect].weaponName, lStyle2);

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[3], panel_Width, earthLabelPos[7])), "Weapon Type:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[3], panel_Width, earthLabelPos[7])), game.shipDatabase.weapons[weaponSelect].damageType.ToString(), lStyle2);

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[4], panel_Width, earthLabelPos[7])), "Weapon Effect:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[4], panel_Width, earthLabelPos[7])), game.shipDatabase.weapons[weaponSelect].effectType.ToString(), lStyle2);

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[5], panel_Width, earthLabelPos[7])), "Equiped/Available:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[5], panel_Width, earthLabelPos[7])), game.player.ships[game.player.currentShip].shipWeapons[weaponSelect] + " / " + (game.player.ships[game.player.currentShip].shipWeapons[weaponSelect] + game.getPurchasableWeapons()[weaponSelect]), lStyle2);

            GUI.Label((new Rect(earthLabelPos[0], earthLabelPos[6], panel_Width, earthLabelPos[7])), "Power Cost:", lStyle);
            GUI.Label((new Rect(earthLabelPos[1], earthLabelPos[6], panel_Width, earthLabelPos[7])), game.shipDatabase.weapons[weaponSelect].powerValue + "§", lStyle2);

            if (game.canBuyWeapon(weaponSelect))
            {
                if (GUI.Button(new Rect(purchaseButtonPos[0], purchaseButtonPos[1], purchaseButtonPos[2], purchaseButtonPos[3]), "Purchase", bStyle))
                {
                    game.purchaseWeapon(weaponSelect);
                }
            }
            else
            {
                GUI.Button(new Rect(purchaseButtonPos[0], purchaseButtonPos[1], purchaseButtonPos[2], purchaseButtonPos[3]), "Not Available", bStyle);
            }
        }

        //Resource icons and tool tip
        for (int i = 0; i < resourceVal.Length; i++)
        {
            GUI.Button(new Rect(resourceIconPos[i], resourceIconPos[7], resourceBox, resourceBox), new GUIContent(recIcon[i], recName[i]), text);
            GUI.Label(new Rect(resourceIconPos[i], resourceIconPos[8], resourceBox, resourceBox), resourceVal[i] + "", text);
        }

        GUI.Label(new Rect(resourceIconPos[4], resourceIconPos[9], resourceIconPos[10], resourceIconPos[10]), GUI.tooltip, text);
        //************************************
    }
    void tradeMenu()
    {
        topPanel topOverlay = gameObject.GetComponent<topPanel>();
        weaponList();

        if (GUI.Button(new Rect(tradeButtonPos[0], tradeButtonPos[2], tradeButtonPos[5], tradeButtonPos[2]), "Buy Ships", bStyle))
        {
            selGridInt = 0;
            ActiveGridInt = 0;
            weapOrShip = true;
        }
        if (GUI.Button(new Rect(tradeButtonPos[0], tradeButtonPos[3], tradeButtonPos[5], tradeButtonPos[2]), "Buy Weapons", bStyle))
        {
            selGridInt = 0;
            ActiveGridInt = 0;
            weapOrShip = false;
        }
        if (GUI.Button(new Rect(tradeButtonPos[1], tradeButtonPos[4], tradeButtonPos[6], tradeButtonPos[7]), "Exit Trade", bStyle))
            topOverlay.trade();

        scrollPos = GUI.BeginScrollView(new Rect(scrollBoxPos[0], scrollBoxPos[1], scrollBoxPos[2], scrollBoxPos[3]), scrollPos, new Rect(0, 0, scrollBoxPos[4], shipString.Length * 20));
        if (weapOrShip) selGridInt = GUI.SelectionGrid(new Rect(0, 0, (int)(panel_Width / 6), shipString.Length * 30), ActiveGridInt, shipString, 1);
        else selGridInt = GUI.SelectionGrid(new Rect(0, 0, (int)(panel_Width / 6), weaponString.Length * 30), ActiveGridInt, weaponString, 1);

        GUI.EndScrollView();
    }


        //slides the panel in our out depending on the isSlide
    void panelslide(){
        if (canSlide & panel_Y > ((Screen.width / 40) - (Screen.width / 2)))panel_Y = panel_Y - 12;    
        if (!canSlide & panel_Y < 0)panel_Y = panel_Y + 12;
    }


    public void panelDisplay()
    {
        if (canSlide) canSlide = false;
        else canSlide = true;
    }



    //Un comment lines to implament weapon restrictions.
    void weaponList()
    {
       int tmpInt=0;
        GameController game = (GameController)GameObject.FindGameObjectWithTag("GameController").GetComponent("GameController");
        for (int i = 0; i < game.shipDatabase.weapons.Length; i++)
        {
            if (game.getPurchasableWeapons()[i] > 0)
                tmpInt++;
        }
        
        
        weaponString = new string[tmpInt];
        weaponID = new int[tmpInt];
        int y = 0;
        for(int i = 0; i< game.shipDatabase.weapons.Length;i++){
            if (game.getPurchasableWeapons()[i] > 0)
            {

                weaponString[y] = game.shipDatabase.weapons[i].weaponName;
                weaponID[y] = game.shipDatabase.weapons[i].weaponID;
                y++;
            }
        }
    }

       void objPosAssign()
    {
        //Resource icon positions
        resourceBox = panel_Width / 45;
        resourceIconPos[0] = (int)(panel_Width / 3.2);
        resourceIconPos[1] = (int)(panel_Width / 2.95);
        resourceIconPos[2] = (int)(panel_Width / 2.728);
        resourceIconPos[3] = (int)(panel_Width / 2.53);
        resourceIconPos[4] = (int)(panel_Width / 2.35);
        resourceIconPos[5] = (int)(panel_Width / 2.2);
        resourceIconPos[6] = (int)(panel_Width / 2.06);
        resourceIconPos[7] = (int)(panel_Height / 1.55);
        resourceIconPos[8] = (int)(panel_Height / 1.44);
        resourceIconPos[9] = (int)(panel_Height / 1.7);
        resourceIconPos[10] = (int)(panel_Width / 10); 

        //purchase button
        purchaseButtonPos[0] = (int)(panel_Width / 2.7);
        purchaseButtonPos[1] = (int)(panel_Height / 1.28);
        purchaseButtonPos[2] = (int)(panel_Width / 7);
        purchaseButtonPos[3] = (int)(panel_Height / 12);

        //trade buttons
        tradeButtonPos[0] = (int)(panel_Width / 25);
        tradeButtonPos[1] = (int)(panel_Width / 15);
        tradeButtonPos[2] = (int)(panel_Height / 10);
        tradeButtonPos[3] = (int)(panel_Height / 4.5);
        tradeButtonPos[4] = (int)(panel_Height / 2.9);
        tradeButtonPos[5] = (int)(panel_Width / 5);
        tradeButtonPos[6] = (int)(panel_Width / 7);
        tradeButtonPos[7] = (int)(panel_Height / 12);
        
        //scroll frame positions
        scrollBoxPos[0] = (int)(panel_Width / 18);
        scrollBoxPos[1] = (int)(panel_Height / 2);
        scrollBoxPos[2] = (int)(panel_Width / 5.5);
        scrollBoxPos[3] = (int)(panel_Height / 2.6);
        scrollBoxPos[4] = (int)(panel_Width / 6);

        //Earth menu lable positions
        earthLabelPos[0] = (int)(panel_Width / 25);
        earthLabelPos[1] = (int)(panel_Width / 4);
        earthLabelPos[2] = (int)(panel_Height / 1.4);
        earthLabelPos[3] = (int)(panel_Height / 1.35);
        earthLabelPos[4] = (int)(panel_Height / 1.3);
        earthLabelPos[5] = (int)(panel_Height / 1.25);
        earthLabelPos[6] = (int)(panel_Height / 1.2);
        earthLabelPos[7] = (int)(panel_Height / 7);
        earthLabelPos[8] = (int)(panel_Width / 6);
        earthLabelPos[9] = (int)(panel_Height / 1.6);
        earthLabelPos[10] = (int)(panel_Width / 5);
        earthLabelPos[11] = (int)(panel_Height / 12);
        earthLabelPos[12] = (int)(panel_Width / 1.887);
        earthLabelPos[13] = (int)(panel_Height / 1.6);

    }

}

