using UnityEngine;
using System.Collections;

public class MovementControl : MonoBehaviour
{

    private bool pressedButton;
    private Ray cursorRay; //ray to check if any GameObject was clicked.
    private Ray shipRay; //ray to get movement vector from ship to cursor location.
    GameObject player, gui;
    ShipAI playerData;
    LootableShip lootdata;
    targetSelect theGUI;
    displayMessage msgHandle;

    private RaycastHit cursorHit;
    private RaycastHit shipHit;
    private LayerMask mask;
    private Vector3 location;

    private bool msgFlag;

    /// <summary>
    /// Initilize some variables. 
    /// </summary>
    void Start()
    {
        pressedButton = false;
        mask = 1 << 8;
        player = GameObject.FindWithTag("Player");
        playerData = (ShipAI)player.GetComponent("ShipAI");
        theGUI = (targetSelect)GameObject.Find("MainHUD").GetComponent("targetSelect");
        msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
    }

    /// <summary>
    /// Ensure theGUI is initialized and get input.  
    /// </summary>
    void Update()
    {
        if (theGUI == null)
            theGUI = (targetSelect)GameObject.Find("MainHUD").GetComponent("targetSelect");

        //pressedButton = Input.GetMouseButtonDown(1);

        if (Input.GetMouseButtonDown(1)) pressedButton = true;
        if (Input.GetMouseButtonUp(1)) pressedButton = false;
    }

    /// <summary>
    /// Handle movement and collision types. 
    /// </summary>
    void LateUpdate()
    {
        theGUI = (targetSelect)GameObject.Find("MainHUD").GetComponent("targetSelect");

        if (pressedButton)
        {
            theGUI.clearMenu();

            cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(Camera.main.transform.position, cursorRay.direction, out cursorHit, 1000, mask) &&
                !string.Equals(cursorHit.collider.tag, "Projectile"))
            {
                // If collision is with player then stop the players movement.
                if (string.Equals(cursorHit.collider.tag, "Player"))
                {
                    playerData.setMoveMode(ShipAI.MoveMode.Stationary);
                    return;
                }

                // If collision is with supergate.
                // And supergate is not usable do not show use option.
                if (string.Equals(cursorHit.collider.tag, "Supergate"))
                {
                    if (playerData.inCombat)
                    {
                        displayMessage msgHandle = (displayMessage)GameObject.Find("MainHUD").GetComponent("displayMessage");
                        msgHandle.texter("You must clear all enemies before using the gate!", 1);

                        theGUI.callGUI(getIntValue(cursorHit.collider.tag), Input.mousePosition.x, Input.mousePosition.y);
                        return;
                    }

                    theGUI.callGUI(1, Input.mousePosition.x, Input.mousePosition.y);
                    return;
                }

                // If collision is with wrecked ship.
                // And wrecked ship has been looted do not show loot option.
                if (string.Equals(cursorHit.collider.tag, "WreckedShip"))
                {
                    lootdata = (LootableShip)cursorHit.collider.gameObject.GetComponent("LootableShip");
                    if (lootdata.isLooted())
                    {
                        theGUI.callGUI(1, Input.mousePosition.x, Input.mousePosition.y);
                        return;
                    }
                }

                theGUI.callGUI(getIntValue(cursorHit.collider.tag), Input.mousePosition.x, Input.mousePosition.y);

            }
            else // there is no collision so move the player to location
            {
                shipRay = new Ray(Camera.main.transform.position, cursorRay.direction);

                location = shipRay.GetPoint(Vector3.Distance(player.transform.position, Input.mousePosition));
                playerData.initiateMove(ShipAI.MoveMode.Waypoint, location);
            }
        }
    }

    /// <summary>
    ///  Takes a string of name tag and returns a pre-specified integer value.
    /// </summary>
    /// <param name="tag">
    /// A string to be parsed <see cref="System.String"/>
    /// </param>
    /// <returns>
    /// A int value of the string <see cref="System.Int32"/>
    /// </returns>
    private int getIntValue(string tag)
    {
        switch (tag)
        {
            case "Enemy":
                return 0;
            case "Ally":
                return 1;
            case "WreckedShip":
                return 2;
            case "Planet":
                return 3;
            case "Supergate":
                return 4;
            case "Homeworld":
                return 5;
            case "Tradeworld":
                return 6;
            default:
                return 1;
        }
    }

    /// <summary>
    /// Move the ship to a game object.
    /// </summary>
    public void selectedMove()
    {
        playerData.initiateMove(ShipAI.MoveMode.FlyatTarget, cursorHit.collider.gameObject);
    }

    /// <summary>
    /// Orbit the ship around a game object. 
    /// </summary>
    public void selectedOrbit()
    {
        playerData.initiateMove(ShipAI.MoveMode.Orbit, cursorHit.collider.gameObject);
    }

    /// <summary>
    /// Attack the specified game object.
    /// </summary>
    public void selectedAttack()
    {
        playerData.initiateMove(ShipAI.MoveMode.FlyatTarget, cursorHit.collider.gameObject);
    }

    /// <summary>
    /// Loot the game object. 
    /// </summary>
    public void selectedLoot()
    {
        if (shipHit.distance > 100)
        {
            msgHandle.texter("Too Far Away!", 1);
            return;
        }
        if (playerData.inCombat)
        {
            msgHandle.texter("Unable To Loot While In Combat!", 1);
            return;
        }

        lootdata.claimLoot();
    }

    /// <summary>
    /// Use the supergate if not too far away.
    /// </summary>
    public void selectedUse()
    {
        if (shipHit.distance <= 300)
            playerData.enterSupergate();
        else
            msgHandle.texter("Too Far Away!", 1);
    }

}
