using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    // class variables up here
    [Tooltip("The speed the camera will move.")]
    public float Speed = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // make an empty vector
        Vector3 direction = Vector3.zero;
        // grab input for the x and y direction to move
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        // move the position of the camera
        transform.position += direction * Speed * Time.deltaTime;
    }
}
