using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEnemy : MonoBehaviour
{

    public int gunHealth = 2;

    float speed = 2f;

    private float GROUND_DETECTION_DISTANCE = 4f;

    public Animator animator;

    private bool moving = true;
    private bool movingRight = true;
    private bool firstApproach = true;
    public float waitTime = 5f;
    private bool patrol = true;
    private bool chase = false;
    private bool attack = false;
    float distanceBetweenEnemyAndPlayer = 10;

    public Transform groundDetection;

    public PolygonCollider2D FIELD_OF_VIEW;

    Collider2D otherCollider;

    float positionOtherCollider;
    float positionEnemyCollider;

    bool triggered = false;

    bool obstacle;
    bool playerobstacle;
    public bool playerhit;
    

    void EnemyIdle()
    {
        moving = false;
        speed = 0f;
    }

    void EnemyRight()
    {
        moving = true;
        speed = 2f;
        transform.eulerAngles = new Vector3(0, 0, 0);
        movingRight = true;
    }

    void EnemyChaseRight()
    {
        moving = true;
        speed = 4f;
        transform.eulerAngles = new Vector3(0, 0, 0);
        movingRight = true;
    }

    void EnemyChaseLeft()
    {
        moving = true;
        speed = 4f;
        transform.eulerAngles = new Vector3(0, -180, 0);
        movingRight = false;
    }
    void EnemyLeft()
    {
        moving = true;
        speed = 2f;
        transform.eulerAngles = new Vector3(0, -180, 0);
        movingRight = false;
    }

    public void OnTriggerStay2D(Collider2D other) // CHASE MECHANIC COLLIDER
    {
        triggered = true;
        tag = other.tag;
        otherCollider = other;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        triggered = false;
    }


    // Update is called once per frame
    void Update()
    {

        obstacle = GameObject.Find("LocalCollider").GetComponent<LocalGunCollision>().obstacleIsThere; // Identify whether or not there is an obstacle in front of the enemy
        playerobstacle = GameObject.Find("LocalCollider").GetComponent<LocalGunCollision>().obstacleIsPlayer; // the player IS the obstacle in front of enemy (post-chase region)

        //to see if the player is within attack distance
        if (playerobstacle)
        {
            //Debug.Log("The enemy SHOULD attack, but will it?");
        }

        if (triggered)
        {
            //Debug.Log("AHHH THERE IS SOMETHING IN SIGHT");
            positionOtherCollider = otherCollider.transform.position.x; //position of the player
            positionEnemyCollider = transform.position.x; //position of the enemy
        }



        //scripting the enemy to identify the player within FOV and then chase the player


        transform.Translate(Vector2.right * speed * Time.deltaTime); //enemy moves to the right by a certain speed 

        RaycastHit2D groundState = Physics2D.Raycast(groundDetection.position, Vector2.down, GROUND_DETECTION_DISTANCE);

        if (playerobstacle == true) //if the player is the obstacle within the attack region
        {
            attack = true;
            patrol = false;
            chase = false;
        }

        else if (tag == "Player" && triggered == true) //if the player is within sight of the enemy 
        {
            //Debug.Log("GUN: PLAYER IS SEEN");

            chase = true;
            patrol = false;
            attack = false;
        }

        else if (triggered == false && obstacle == false) //if player is not within sight of the enemy
        {
            speed = 2f;
            moving = true;
            patrol = true;
            chase = false;
            attack = false;
        }

        else
        {
            moving = true;
            patrol = true;
            chase = false;
            attack = false;
        }

        /*Debug.Log(attack);
        Debug.Log(chase);
        Debug.Log(patrol);
        Debug.Log(moving);*/

        //finite state machine: states= patrol, chase, attack

        if (patrol == true) //if enemy is patrolling
        {

            //Debug.Log("GUN PATROLLING AROUND");

            if (groundState.collider == false) //if there is no platform detected in front of the enemy to walk on
            {

                //Debug.Log("WHERE'S THE GODDAMN GROUND.");

                if (firstApproach == true && waitTime > 0) //if in idle state
                {
                    EnemyIdle();
                    waitTime = waitTime - 1f * Time.deltaTime;
                }

                else if (movingRight == true) //if moving right
                {
                    EnemyLeft();
                    firstApproach = true;
                    waitTime = 5f;
                }

                else if (movingRight == false) //if moving left
                {
                    EnemyRight();
                    firstApproach = true;
                    waitTime = 5f;
                }
            }

            else if (obstacle) //if there is a obstacle blocking the enemy's right of way
            {
                //Debug.Log("obstacle detected");
                firstApproach = true;
                waitTime = 5f;

                if (movingRight == true) //if moving right then turn left
                {
                    //Debug.Log("TURNING LEFT");
                    EnemyLeft();
                }
                else if (movingRight == false) //if moving left then turn right
                {
                    //Debug.Log("TURNING RIGHT");
                    EnemyRight();
                }
            }
        }

        else if (chase == true && obstacle == false) //if enemy is chasing
        {
            //Debug.Log("GUN: CHASING ENEMY");
            if (positionEnemyCollider < positionOtherCollider)
            {
                EnemyChaseRight();
            }
            else if (positionEnemyCollider > positionOtherCollider)
            {
                EnemyChaseLeft();
            }
        }

        else if (attack == true) //if enemy is attacking
        {
            speed = 0f;
            moving = false;
            //Debug.Log("GUN: ATTACKING ENEMY");
        }

        animator.SetBool("moving", moving);
        animator.SetBool("attacking", attack);

        if (attack == true && playerobstacle)
        {
            //Debug.Log("Player is currently BEING ATTACKED");
            playerhit = true;
        }

        else
        {
            playerhit = false;
        }


        if (gunHealth <= 0)
        {
            //Debug.Log("GUNALALA ENEMY IS DONE GOT DEADD");
            Destroy(gameObject);
        }
    }
}
