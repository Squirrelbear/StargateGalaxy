using UnityEngine;
using System.Collections;

/***********************************************************           
    * Manin menu camera: rotates the main menu camera      *
    *                                                      *   
    * Author:  Karlos White                                *   
    *                                                      *   
    ********************************************************/
public class cameraRotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        transform.Rotate(Time.deltaTime, 0, 0);
        transform.Rotate(0, Time.deltaTime, 0, Space.World);
    }
}
