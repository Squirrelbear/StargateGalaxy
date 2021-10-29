using UnityEngine;
using System.Collections;

public class AITest : MonoBehaviour {

    private Transform enemy;
    public Transform enemy1;
    public Transform enemy2;
    public float minimumDistance = 2.0f;
    public float speed = 20.0f;
    public float rotateSpeed = 2.0f;
    public int currentTarget = 0;

	// Use this for initialization
	void Start () {
        enemy1 = GameObject.Find("Ship2").transform;
        enemy2 = GameObject.Find("Ship3").transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (currentTarget == 0)
        {
            enemy = enemy1;
        }
        else
        {
            enemy = enemy2;
        }

        if (Vector3.Distance(enemy.position, transform.position) > minimumDistance)
        {
            Vector3 target = enemy.position;
            Vector3 moveDirection = target - transform.position;

            Vector3 velocity = rigidbody.velocity;

            if (moveDirection.magnitude < 1)
            {
                velocity = Vector3.zero;
            }
            else
            {
                velocity = moveDirection.normalized * speed;
            }

            rigidbody.velocity = velocity;
        }
        else
        {
            rigidbody.velocity = Vector3.zero;
            if (currentTarget == 0)
            {
                currentTarget = 1;
            }
            else
            {
                currentTarget = 0;
            }
        }

        transform.LookAt(enemy);
	}
}
