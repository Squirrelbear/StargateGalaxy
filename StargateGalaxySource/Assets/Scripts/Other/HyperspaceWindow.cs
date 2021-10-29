using UnityEngine;
using System.Collections;

public class HyperspaceWindow : MonoBehaviour
{

    float cooldown;

    // Use this for initialization
    void Start()
    {
        cooldown = 10;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
            DestroyObject(gameObject);
    }
}
