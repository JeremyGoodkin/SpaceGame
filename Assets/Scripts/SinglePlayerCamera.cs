using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerCamera : MonoBehaviour
{
    public GameObject playerObject;

    public float cameraSpeed;
    public float cameraOffset = 3;
    public float maxTiltAngle = 10;
    public float tiltSpeed = 10;
        
    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, maxTiltAngle * Mathf.Sin(Time.time * tiltSpeed));

        transform.position = new Vector3(0, Mathf.Lerp(transform.position.y, playerObject.transform.position.y + cameraOffset, cameraSpeed), -10);
    }
}
