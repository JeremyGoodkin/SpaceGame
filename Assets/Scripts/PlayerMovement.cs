using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D mRB;
    public float playerSpeed = 3f;
    public float jumpHeight = 7f;
    private bool inAir = true;
    private bool canLatch;
    private float jumpCharger;
    private bool jumpPressed;
    private bool jumpDischarge;
    private float horizontal;
    private float vertical;
    // Start is called before the first frame update
    void Start()
    {
        mRB = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxis("Jump");
        horizontal = Input.GetAxis("Horizontal");
        if (jumpCharger == 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (jumpCharger < 1f && vertical > 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if (jumpCharger < 2f)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
        }

        if ((horizontal > 0 && vertical == 0) || (horizontal > 0 && inAir))
        {
            mRB.velocity = new Vector3(playerSpeed, mRB.velocity.y);
        }
        else if ((horizontal < 0 && vertical == 0) || (horizontal < 0 && inAir))
        {
            mRB.velocity = new Vector3(-playerSpeed, mRB.velocity.y);
        }
        else
        {
            mRB.velocity = new Vector3(mRB.velocity.x, mRB.velocity.y);
        }

        if (vertical > 0 && !inAir)
        {
            jumpCharger += Time.deltaTime;
            jumpPressed = true;
        }

        if (vertical == 0 && jumpPressed && !inAir)
        {
            jumpDischarge = true;
        }

        if (Input.GetKeyDown(KeyCode.S) && canLatch)
        {
            mRB.gravityScale = 0.05f;
            mRB.velocity = Vector2.zero;
        }
        

    }

    private void FixedUpdate()
    {
        if (jumpDischarge)
        {
            if (jumpCharger <= 1f)
            {
                mRB.velocity = new Vector3(mRB.velocity.x, jumpHeight * 0.5f);
            }
            else if (jumpCharger <= 2f)
            {
                mRB.velocity = new Vector3(mRB.velocity.x, jumpHeight);
            }
            else
            {
                mRB.velocity = new Vector3(mRB.velocity.x, jumpHeight * 1.5f);
            }

            jumpDischarge = false;
            jumpCharger = 0f;
            jumpPressed = false;
            inAir = true;

        }
        
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Floor")
        {
            inAir = false;
            mRB.gravityScale = 0.6f;
        }

        if (collision.gameObject.name == "Wall")
        {
            canLatch = true;
        }

        
       
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Wall")
        {
            mRB.gravityScale = 0.6f;
            canLatch = false;
        }
    }
}
