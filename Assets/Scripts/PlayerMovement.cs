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
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

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

        if (((horizontal > 0 && jump == 0) || (horizontal > 0 && inAir)) && !isLatched)
        {
            mRB.velocity = new Vector3(playerSpeed, mRB.velocity.y);
        }
        else if (((horizontal < 0 && jump == 0) || (horizontal < 0 && inAir)) && !isLatched)
        {
            mRB.velocity = new Vector3(-playerSpeed, mRB.velocity.y);
        }
        else
        {
            mRB.velocity = new Vector3(mRB.velocity.x * 0.75f, mRB.velocity.y);
        }

        if (jump > 0 && (!inAir || isLatched))
        {
            if (isLatched)
            {
                if (GameObject.Find("Wall").transform.position.x - transform.position.x < 0)
                {
                    mRB.velocity = new Vector3(mRB.velocity.x + 2 + horizontal, jumpHeight * 0.75f + vertical);
                }
                else if (GameObject.Find("Wall").transform.position.x - transform.position.x > 0)
                {
                    mRB.velocity = new Vector3(mRB.velocity.x - 2 + horizontal, jumpHeight * 0.75f + vertical);
                }
                
                isLatched = false;
            }
            else
            {
                jumpCharger += Time.deltaTime;
                jumpPressed = true;
            }
            
        }

        if (jump == 0 && jumpPressed && !inAir)
        {
            jumpDischarge = true;
        }

        if (Input.GetKeyDown(KeyCode.S) && canLatch)
        {
            if (isLatched)
            {
                isLatched = false;
                mRB.gravityScale = 0.6f;
            }
            else
            {
                isLatched = true;
                mRB.gravityScale = 0.05f;
                mRB.velocity = Vector2.zero;
            }
            
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
            isLatched = false;
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
