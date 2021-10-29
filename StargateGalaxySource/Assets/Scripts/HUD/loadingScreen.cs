using UnityEngine;
using System.Collections;

public class loadingScreen : MonoBehaviour
{

//Script assests.
    public GUIStyle lStyle;
    public GUIStyle tStyle;

    public Texture[] hyper = new Texture[41];
    private double tmr;
    private int img = 0;

	void Start () {
        tmr = Time.time;
	}
	
	void Update () {
        if ((Time.time-tmr) > 0.07)
        {
            if (img < 40) img++;
            else img = 0;
            tmr = Time.time;
        }
	}
    
    void OnGUI(){
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hyper[img], ScaleMode.StretchToFill, true, 10.0F);     
    }

}
