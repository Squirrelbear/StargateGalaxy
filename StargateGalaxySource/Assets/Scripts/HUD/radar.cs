using UnityEngine;
using System.Collections;
public class radar : MonoBehaviour {

    public Texture testbox;

    private int panel_Width = (int)(Screen.width / 1.11);
    private int panel_Height = (int)(Screen.width / 5.3);
    private int panel_X = (int)(Screen.width / 20);
    private int panel_Y = Screen.height - (int)(Screen.width / 6);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {

        Rect radarBox = new Rect(panel_Width, panel_Y,panel_Width, panel_Height);
        GUI.BeginGroup(radarBox,testbox);

        GUI.EndGroup();

    }
}
