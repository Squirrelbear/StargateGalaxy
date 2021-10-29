using UnityEngine;
using System.Collections;

public class ShipCamera : MonoBehaviour
{
    public Transform target;
    public float idleDistance = 10f;
    public float height;
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;
    public float scaleFactor = 15f;

    private Transform myTransform;
    private float x, y;
    private bool buttonDown = false;

    void Start()
    {
        myTransform = transform;
    }

    void Update()
    {
        // Ensure target is not used if is null.
        if (target == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");

            if (obj == null) return;

            target = obj.transform;
        }

        // Check for input.
        if (Input.GetMouseButtonDown(2)) buttonDown = true;
        if (Input.GetMouseButtonUp(2)) buttonDown = false;

        // Handle being able to zoom using mouse wheel.
        if (Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            idleDistance -= (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * scaleFactor * Mathf.Abs(idleDistance);
            idleDistance = (float)Mathf.Clamp(idleDistance, 7, 900);
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (buttonDown)
        {
            x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        }

        Quaternion rotation = Quaternion.Euler(y, x, 0);

        Vector3 position;

        position = rotation * new Vector3(0.0f, 0.0f, -idleDistance) + target.position;

        myTransform.rotation = rotation;
        myTransform.position = position;
    }
}