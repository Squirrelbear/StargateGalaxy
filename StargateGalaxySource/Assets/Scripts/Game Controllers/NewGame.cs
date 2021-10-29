using UnityEngine;
using System.Collections;

public class NewGame : MonoBehaviour {

    // keep this object alive between scenes
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
