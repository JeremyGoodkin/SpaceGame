using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    [Tooltip("Drag the game object you want the camera to follow into here")]
    public GameObject Target;
    [Tooltip("How snappy the camera is from 0 to 1")]
    public float LerpTVal = 0.5f;

    public float ShakeTime = 0;
    public float ShakeMagnitude = 0;

    public void TriggerShake(float time, float magnitude)
    {
        if (magnitude > ShakeMagnitude)
        {
            ShakeMagnitude = magnitude;
        }
        if (time > ShakeTime)
        {
            ShakeTime = time;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TriggerShake(1, 2);
        }

        if (Target != null)
        {
            //calculate the position to aim for
            Vector3 newPos = Target.transform.position;
            newPos.z = transform.position.z;
            //lerp (linearly interpolate) towards that point which smooths it
            transform.position = Vector3.Lerp(transform.position, newPos, LerpTVal);
            if (ShakeTime > 0)
            {
                //decrease shake timer
                ShakeTime -= Time.fixedDeltaTime;
                Vector3 shakeDir = Random.insideUnitCircle;
                transform.position += shakeDir * ShakeMagnitude;
            }
        }
    }

}
