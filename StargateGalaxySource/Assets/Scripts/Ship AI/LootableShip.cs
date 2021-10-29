using UnityEngine;
using System.Collections;

public class LootableShip : MonoBehaviour
{
    public bool looted;
    public int[] loot;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setResources(int[] resources)
    {
        loot = new int[resources.Length];

        for (int i = 0; i < loot.Length; i++)
        {
            loot[i] = resources[i];
        }
    }

    public int[] getResources()
    {
        return loot;
    }

    public bool isLooted()
    {
        return looted;
    }

    public void claimLoot()
    {
        if (looted) return;

        GameObject game = GameObject.Find("GameController");
        GameController controller = (GameController)game.GetComponent("GameController");
        controller.addResources(loot);
        looted = true;
    }
}
