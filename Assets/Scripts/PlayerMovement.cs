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
    private bool isLatched;
    private float jumpCharger;
    private bool jumpPressed;
    private bool jumpDischarge;
    private float horizontal;
    private float jump;
    private float vertical;
    // Start is called before the first frame update
    void Start()
    {
        mRB = gameObject.GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        jump = Input.GetAxis("Jump");
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //Color control
        if (jumpCharger == 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else if (jumpCharger < 1f && jump > 0)
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

        //Left and right movement
        mRB.velocity = new Vector2(horizontal * playerSpeed * System.Convert.ToInt32((jump == 0 || inAir) && !isLatched), mRB.velocity.y);

        //Jump behavior
        if (jump > 0 && (!inAir || isLatched))
        {
            if (isLatched)
            {
                //RayCast to figure out which side of the wall player latched to
                Ray2D leftRay = new Ray2D(transform.position, Vector2.left);
                RaycastHit2D leftHit = Physics2D.Raycast(leftRay.origin, leftRay.direction, 0.55f);
                Ray2D rightRay = new Ray2D(transform.position, Vector2.right);
                RaycastHit2D rightHit = Physics2D.Raycast(rightRay.origin, rightRay.direction, 0.55f);

                if (rightHit.collider != null && horizontal < 0)
                {
                    mRB.velocity = new Vector3(mRB.velocity.x + 2 + horizontal, jumpHeight * 0.75f + (vertical * 1.5f));
                    isLatched = false;
                }
                else if (leftHit.collider != null && horizontal > 0)
                {
                    mRB.velocity = new Vector3(mRB.velocity.x - 2 + horizontal, jumpHeight * 0.75f + (vertical * 1.5f));
                    isLatched = false;
                }
                
            }
            else //Allows for charge jump when not latched
            {
                jumpCharger += Time.deltaTime;
                jumpPressed = true;
            }
            
        }

        //Sets off FixedUpdate
        if (jump == 0 && jumpPressed && !inAir)
        {
            jumpDischarge = true;
        }

        //Latch behavior
        if (Input.GetKeyDown(KeyCode.S))
        {
            
            if (isLatched)
            {
                isLatched = false;
                mRB.gravityScale = 0.6f;
                canLatch = true;
            }
            else if (canLatch)
            {
                isLatched = true;
                mRB.gravityScale = 0.0f;
                mRB.velocity = Vector2.zero;
            }
            
        }
        
        if (mRB.velocity.y != 0)
        {
            inAir = true;
        }
        else
        {
            inAir = false;
        }

        if (inAir)
        {
            mRB.gravityScale = 0.6f;
        }
        else
        {
            canLatch = false;
        }
    }

    private void FixedUpdate()
    {
        //Jumps and resets variables
        if (jumpDischarge)
        {
            //Charge jump values
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
        
        //Fixes bug where you can launch off wall and can't move left/right
        if (isLatched && mRB.velocity.y > 0)
        {
            mRB.velocity = Vector2.zero;
        }

        

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (inAir)
        {
            canLatch = true;
        }

    }
}
