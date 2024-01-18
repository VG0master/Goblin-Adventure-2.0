
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    //Setting up all the variables that we are using, Header is just a fancy way for unity to show us what goes were.
    //All variable names should be self explanatory
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    private float storeMoveSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpPower;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump = true;
    private float storeJumpPower;

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
    private float speedDuration;
    private float jumpBoostDuration;
    public Transform orientation;

    //for speed/jump boosts
    private bool speedBoost;
    private bool jumpBoost;


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

    //For temporary movement multipliers.
    public void speedMult(int mult, int duration){
        if (!speedBoost){
            //Stores the movespeed and sets the temporary speed.
            storeMoveSpeed = moveSpeed;
            speedDuration = duration;
            speedBoost = true;
        
            moveSpeed = moveSpeed * mult;
        }
            
        else if (speedBoost && moveSpeed/mult == storeMoveSpeed){
            speedDuration += duration;
        }
            
        else {
            speedDuration = duration;
            speedBoost = true;
            moveSpeed = storeMoveSpeed * mult;
        }
    }

    public void jumpMult(int mult, int duration){
        if (!jumpBoost){
            //Stores the jump power and sets the temporary jump power.
            storeJumpPower = jumpPower;
            jumpBoostDuration = duration;
            jumpBoost = true;
        
            jumpPower = jumpPower * mult;
        }
            
        else if (jumpBoost && jumpPower/mult == storeJumpPower){
            speedDuration += duration;
        }
            
        else {
            jumpBoostDuration = duration;
            jumpBoost = true;
            jumpPower = storeJumpPower * mult;
        }
        
    }


    //Start is called once before first frame
    void Start()
    {
        //Setting all the variables to what they should be :)
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;

        storeMoveSpeed = moveSpeed;
        storeJumpPower = jumpPower;
        speedBoost = false;
        jumpBoost = false;
        speedDuration = 0;
        jumpBoostDuration = 0;
    }

    // Update is called once per frame 
    void Update()
    {
        //Checks if a boost is active, then ticks it down if it is.
        if (speedBoost || speedDuration > 0){
            speedDuration = speedDuration - Time.deltaTime;
            
            if (speedDuration <= 0){
                speedBoost = false;
                speedDuration = 0;
                moveSpeed = storeMoveSpeed;
            }
        }
        
        if (jumpBoost || jumpBoostDuration > 0){
            jumpBoostDuration = jumpBoostDuration - Time.deltaTime;
            
            if (jumpBoostDuration <= 0){
                jumpBoost = false;
                jumpBoostDuration = 0;
                jumpPower = storeJumpPower;
            }
        }
            
        
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

