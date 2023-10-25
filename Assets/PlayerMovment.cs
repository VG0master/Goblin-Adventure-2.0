
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovment : MonoBehaviour
{
    //Setting up all the variables that we are using, Header is just a fancy way for unity to show us what goes were.
    //All variable names should be self explanatory
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpPower;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump = true;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;


    [Header("KeyBinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsground;
    private bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;


    private float horizontalInput;
    private float verticalInput;
    public Transform orientation;


    private Vector3 moveDirection;
    private Rigidbody rb;
    public MovementState state;
    public enum MovementState
    {
        WALKING,
        SPRINTING,
        CROUCHING,
        AIR
    }

    //Start is called once before first frame
    void Start()
    {
        //setting all the variables to what they should be :)
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame 
    void Update()
    {
        //Raycast is funny thing that is basically a line from A to B.
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsground);

        //Start all the functions
        Inputs();
        SpeedCheck();
        StateHandler();

        //I aint explaining what drag is in one comment search it up
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }
    //This is constant, its always to same so if the computer lags so does this, which basically makes sure that the movement is in sync with computer
    private void FixedUpdate()
    {
        MovePlayer();
    }
    //Gets all the inputs and sets the custom movment functions
    private void Inputs()
    {
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    //Moves the player using WASD
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if(!grounded){
            
		float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
		if (angle <= maxSlopeAngle){
			rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
		} 
        }

        //Preventing sliding on slopes (again we hate realistic physics)
        rb.useGravity = !OnSlope();
    }

    //Making sure we can go over the speed limit (we hate realistic physics)
    private void SpeedCheck()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }
    //Are you going to make me explain this one
    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x,0f,rb.velocity.z);
        rb.AddForce(transform.up * jumpPower, ForceMode.Impulse);
    }
    //Need a function for this because we want to invoke it in the Inputs() method.
    private void ResetJump() {
        readyToJump = true;
        exitingSlope = false;
    }
    //I dont want to be passive agressive but this is also pretty self explanatory
    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.CROUCHING;
            moveSpeed = crouchSpeed;
        }

        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.SPRINTING;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.WALKING;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.AIR;
        }
    }
    //Detects if we are currently on a slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}

