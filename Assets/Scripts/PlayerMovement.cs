using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D col;
    Animator anim;

    public float groundSpeed = 10;
    public float airSpeed = 5;
    float momentum; // used for acceleration-based air movement

    [Tooltip("How many units the player can jump at max charge")]
    public float maxJumpHeight = 10;
    [Tooltip("How long in seconds the max charge takes")]
    public float maxJumpChargeTime = 1;
    [Tooltip("The largest angle the player can jump from the wall.\n0 is straight forward, 90 is straight up/down.")]
    public float maxJumpAngle = 45;

    public LayerMask levelLayer;
    bool grounded;  // touching ground
    bool latchable; // touching wall
    [Tooltip("How close to a wall the player must be to be able to latch.")]
    public float latchDistance = 0.1f;
    [Tooltip("How close to the ground the player must be to be considered grounded.")]
    public float groundedDistance = 0.1f;

    public KeyCode jumpKeyCode = KeyCode.Space;
    bool jumpButtonPressed; // controller jump input
    bool jumpPressed;       // keyboard jump input + controller jump input
    float jumpChargeTimer;

    string currentAnimation;
    string newAnimation;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }


    // controller input
    // learned from https://www.youtube.com/watch?v=IurqiqduMVQ

    PlayerControls controls;
    Vector2 joystick; // joystick input
    int joystickPressed; // bool for if their is currently joystick input

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Movement.performed += ctx =>
        {
            //Debug.Log(ctx.ReadValue<Vector2>());
            joystick = ctx.ReadValue<Vector2>();
            joystickPressed = 1;
        };
        controls.Gameplay.Movement.canceled += ctx => joystickPressed = 0;

        controls.Gameplay.Jump.performed += ctx => jumpButtonPressed = true;
        controls.Gameplay.Jump.canceled += ctx => jumpButtonPressed = false;
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Update()
    {
        float maxVelocity = Mathf.Sqrt(maxJumpHeight * -2 * rb.gravityScale * Physics2D.gravity.y); // equation for the velocity to reach a certain height

        grounded = BoxCastDraw(col.bounds.center + Vector3.down * col.bounds.extents.y, new Vector2(col.bounds.size.x, groundedDistance * 2), 0, Vector2.down, groundedDistance, levelLayer).collider != null;
        latchable = LatchCheck(1) || LatchCheck(-1);
        jumpPressed = jumpButtonPressed || Input.GetKey(jumpKeyCode);

        

        // horizontal movement
        float horizontalInput = (joystick.x * joystickPressed + Input.GetAxisRaw("Horizontal") * (1 - joystickPressed)) // allows joystick input to trump keyboard input
                              * System.Convert.ToInt32(!(jumpPressed && (grounded || latchable))); // lock movement when charging a jump
        if (horizontalInput != 0) transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1, 1);

        momentum = grounded ? horizontalInput * groundSpeed : Mathf.Clamp(momentum + horizontalInput * airSpeed * Time.deltaTime, -maxVelocity, maxVelocity);

        rb.velocity = new Vector2(grounded ? horizontalInput * groundSpeed : momentum, rb.velocity.y);


        // jumping
        if (jumpPressed && (grounded || latchable))
            jumpChargeTimer += Time.deltaTime;
        
        else if (jumpChargeTimer != 0 && (grounded || latchable))
        {
            Vector2 jumpDir = joystick.normalized * joystickPressed + Vector2.up * (1 - joystickPressed); // defaults to up if no directional input

            if (latchable && !grounded) jumpDir = jumpAngleClamp(jumpDir, maxJumpAngle); // this function keeps the jump direction within a certain range (also called clamping)

            rb.velocity = jumpDir * maxVelocity * Mathf.Clamp(jumpChargeTimer, 0, maxJumpChargeTime) / maxJumpChargeTime; // provides a percent of the max charge time reached

            momentum = rb.velocity.x;
        }

        if (!jumpPressed && jumpChargeTimer != 0) jumpChargeTimer = 0;

        if (LatchCheck((int)Mathf.Sign(momentum))) momentum = 0; // reset jump momentum if bumping a wall

        rb.constraints = RigidbodyConstraints2D.FreezeRotation | // if latchable and holding jump, freeze y position
                         (latchable && jumpPressed ? RigidbodyConstraints2D.FreezePositionY : RigidbodyConstraints2D.None);


        // animation
        // learned from https://www.youtube.com/watch?v=nBkiSJ5z-hE&t=540s

        if (jumpPressed && (grounded || latchable)) newAnimation = "PlayerCrouch" + Mathf.Ceil(3 * Mathf.Min(jumpChargeTimer / maxJumpChargeTime, 1));
        else if (grounded && horizontalInput != 0) newAnimation = "PlayerRun";
        else if (grounded && rb.velocity.y == 0) newAnimation = "PlayerIdle";
        else if (!jumpPressed && jumpChargeTimer != 0) newAnimation = "PlayerJump";
        else if (currentAnimation != "PlayerJump" && !grounded) newAnimation = "PlayerAir";

        if (currentAnimation != newAnimation) anim.Play(newAnimation);
        currentAnimation = newAnimation;


        if (Input.GetKeyDown(KeyCode.Return)) // scene reset dev tool
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    Vector2 jumpAngleClamp(Vector2 inputDir, float maxAngle)
    {
        int jumpableDir = System.Convert.ToInt32(LatchCheck(-1)) * 2 - 1; // detects if wall is left/right facing
        float clampedAngle = Mathf.Clamp(Vector2.SignedAngle(Vector2.right, inputDir * jumpableDir), -maxAngle, maxAngle); // converts to angle and clamps it
        return new Vector2(Mathf.Cos(clampedAngle * Mathf.Deg2Rad), Mathf.Sin(clampedAngle * Mathf.Deg2Rad)) * jumpableDir; // converts back to vector
    }

    bool LatchCheck(int dir)
    {
        return BoxCastDraw(transform.position, col.bounds.size / 2, 0, dir * Vector2.right, latchDistance + col.bounds.size.x / 4, levelLayer).collider != null;
    }

    // debug tools

    RaycastHit2D RayCastDraw(Vector2 origin, Vector2 direction, float distance, LayerMask mask)
    {
        Debug.DrawLine(origin, origin + direction * distance);
        return Physics2D.Raycast(origin, direction, distance, mask);
    }

    RaycastHit2D BoxCastDraw(Vector2 origin, Vector2 size, float angle, Vector2 direction, float distance, LayerMask mask)
    {
        // from https://forum.unity.com/threads/visualize-a-2d-boxcast-with-this-simple-wrapper-method.415356/

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, angle, direction, distance, mask);

        //Setting up the points to draw the cast
        Vector2 p1, p2, p3, p4, p5, p6, p7, p8;
        float w = size.x * 0.5f;
        float h = size.y * 0.5f;
        p1 = new Vector2(-w, h);
        p2 = new Vector2(w, h);
        p3 = new Vector2(w, -h);
        p4 = new Vector2(-w, -h);

        Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        p1 = q * p1;
        p2 = q * p2;
        p3 = q * p3;
        p4 = q * p4;

        p1 += origin;
        p2 += origin;
        p3 += origin;
        p4 += origin;

        Vector2 realDistance = direction.normalized * distance;
        p5 = p1 + realDistance;
        p6 = p2 + realDistance;
        p7 = p3 + realDistance;
        p8 = p4 + realDistance;


        //Drawing the cast
        Color castColor = hit ? Color.red : Color.green;
        Debug.DrawLine(p1, p2, castColor);
        Debug.DrawLine(p2, p3, castColor);
        Debug.DrawLine(p3, p4, castColor);
        Debug.DrawLine(p4, p1, castColor);

        Debug.DrawLine(p5, p6, castColor);
        Debug.DrawLine(p6, p7, castColor);
        Debug.DrawLine(p7, p8, castColor);
        Debug.DrawLine(p8, p5, castColor);

        Debug.DrawLine(p1, p5, Color.grey);
        Debug.DrawLine(p2, p6, Color.grey);
        Debug.DrawLine(p3, p7, Color.grey);
        Debug.DrawLine(p4, p8, Color.grey);
        if (hit)
        {
            Debug.DrawLine(hit.point, hit.point + hit.normal.normalized * 0.2f, Color.yellow);
        }

        return hit;
    }
}