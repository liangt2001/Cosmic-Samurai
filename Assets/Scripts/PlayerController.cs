using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayerMask;
    Rigidbody2D rigidbody2D;
    BoxCollider2D boxCollider2D;
    Animator animator;
    const float playerSpeed = 5.0f;
    const float jumpSpeed = 10.0f;
    const float jumpTimer = 0.40f;
    const float fallMultiplier = 2.5f;
    const float lowJumpMultiplier = 1.5f;
    float countingJumpTimer = 0.0f;
    const float jumpDelay = 0.2f;
    float jumpDelayTimer = 0.0f;

    /// <summary>
    /// //ALL THESE BOOLEANS ARE POTENTIALLY GARBAGE
    /// </summary>
    Boolean jumpRequest;
    Boolean falling = false;
    Boolean onGround = true;
    Boolean continueJump = false;
    Boolean lowJump = false;
    Boolean jumping = false;
    float verticalInput = 0;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        //if(Input.GetAxis("Vertical"))
        /*if (verticalInput > 0 && isGrounded() && countingJumpTimer <= 0 && jumpDelayTimer <= 0) jumpRequest = true;
        else jumpRequest = false;

        if (verticalInput > 0 && !isGrounded() && countingJumpTimer > 0 && jumpDelayTimer <= 0) continueJump = true;
        else continueJump = false;

        if (rigidbody2D.velocity.y < 0 && jumpDelayTimer <= 0) falling = true;
        else if (rigidbody2D.velocity.y > 0 && verticalInput <= 0 && !isGrounded() && jumpDelayTimer <= 0) lowJump = true;
        else
        {
            lowJump = false;
            falling = false;
        }

        //if()

        if (rigidbody2D.velocity.y < 0 && jumpDelayTimer <= 0) falling = true;*/

        if (countingJumpTimer > 0) countingJumpTimer -= Time.deltaTime;
        else
        {
            countingJumpTimer = 0;
        }
        if (jumpDelayTimer > 0) jumpDelayTimer -= Time.deltaTime;
        else jumpDelayTimer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ///////////////////
        ///MOVEMENT////////
        ///////////////////
        UnityEngine.Vector2 position = rigidbody2D.position;
        animator.SetFloat("Direction", Math.Sign(animator.GetFloat("Move X")));
        //Move X is with the running animation, so set a running animation if player is moving right(this works by itself)
        if (Input.GetAxis("Horizontal") != 0)
        {
            animator.SetFloat("Speed", 1.0f);
            animator.SetFloat("Move X", Math.Sign(Input.GetAxis("Horizontal")));
        }
        //Set an Idle animation if player stops
        else
        {
            animator.SetFloat("Speed", 0);
            //Makes sure the default direction is to the right (i.e. if direction was undetermined)
            //Sets the idle animation to what the player moved at last time (i.e. if they moved right, plays the right idle animation)
            //animator.SetFloat("Direction", Math.Sign(animator.GetFloat("Move X")));
        }
        //Basic horizontal movement
        position.x += Input.GetAxis("Horizontal") * Time.fixedDeltaTime * playerSpeed;
        /////////////////////
        /////JUMPING/////////
        ////////////////////
       
        //Added a very minor delay to prevent player from being able to jump immediately after landing 
        if(isGrounded() && jumping)
        {
            animator.SetBool("Jumping", false);
            jumping = false;
            jumpDelayTimer = jumpDelay;
        }
        //Player can jump if they're not already jumping, and they're ON the ground too
        if (verticalInput > 0 && isGrounded() && countingJumpTimer <= 0 && jumpDelayTimer <= 0)//verticalInput > 0 && isGrounded() && countingJumpTimer <= 0 && jumpDelayTimer <= 0
        {
            rigidbody2D.velocity = UnityEngine.Vector2.up * jumpSpeed;
            //position.y += jumpSpeed * Time.deltaTime;
            jumping = true;
            countingJumpTimer = jumpTimer;
            animator.SetBool("Jumping", true);
            // jumpRequest = false;
        }
        //Player continues moving upward
        if(verticalInput > 0 && !isGrounded() && countingJumpTimer > 0 && jumpDelayTimer <= 0 && !hitCeiling())//verticalInput > 0 && !isGrounded() && countingJumpTimer > 0 && jumpDelayTimer <= 0
        {
            //Debug.Log("1");
            rigidbody2D.velocity = UnityEngine.Vector2.up * jumpSpeed;
        }

        //When the player starts falling
        if (rigidbody2D.velocity.y < 0 && jumpDelayTimer <= 0 && !isGrounded())//rigidbody2D.velocity.y < 0 && jumpDelayTimer <= 0
        {
            //Debug.Log("2");
            countingJumpTimer = 0;
            rigidbody2D.velocity += UnityEngine.Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        //Initial low jump
        else if (rigidbody2D.velocity.y > 0 && verticalInput <= 0 && !isGrounded() && jumpDelayTimer <= 0)//rigidbody2D.velocity.y > 0 && verticalInput <= 0 && !isGrounded() && jumpDelayTimer <= 0
        {
            countingJumpTimer = 0;
            //Debug.Log("3");
            rigidbody2D.velocity += UnityEngine.Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        //This statement actually modifies the final position
        rigidbody2D.position = position;
    }

    private Boolean isGrounded()
    {
        float extraHeight = 0.3f;
        Collider2D hitCollider = Physics2D.OverlapBox(boxCollider2D.bounds.center + UnityEngine.Vector3.down*extraHeight, transform.localScale/4, 0.0f, platformLayerMask);
        return hitCollider != null; 
    }

    private Boolean hitCeiling()
    {
        float extraHeight = 0.3f;
        Collider2D hitCollider = Physics2D.OverlapBox(boxCollider2D.bounds.center + UnityEngine.Vector3.up * extraHeight, transform.localScale/4,0.0f, platformLayerMask);
        return hitCollider != null;
    }

    //Draw the Box Overlap as a gizmo to show where it currently is testing (KEEP FOR DEBUGGING)

   void OnDrawGizmos()
    {
        float extraHeight = 0.1f;
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        Gizmos.DrawWireCube(boxCollider2D.bounds.center + UnityEngine.Vector3.down * 0.3f, transform.localScale / 4);
        Gizmos.DrawWireCube(boxCollider2D.bounds.center + UnityEngine.Vector3.up * 0.3f, transform.localScale / 4);
    }
}
