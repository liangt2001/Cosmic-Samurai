using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{

    public bool respawnable;
    int meleeHealth = 2;

    float meleespeed = 2;

    float GROUND_DETECTION_DISTANCE = 1f;

    Animator meleeanimator;

    bool moving = true;
    bool movingRight = true;
    bool firstApproach = true;
    float waitTime = 5f;
    bool patrol = true;
    bool chase = false;
    bool attack = false;

    Transform MeleeGroundDetection;

    PolygonCollider2D MELEE_FIELD_OF_VIEW;

    Collider2D otherCollider;

    Rigidbody2D enemyBody;

    float positionOtherCollider;
    float positionEnemyCollider;

    bool meleetriggered = false;

    PlayerHealth playerhealth;

    float destroyDelay = 4f;
    float invincibilityTimer = 0;
    float invincibilityTime = 0.3f;

    LocalMeleeCollision obstacle;
    float dir;

    GameObject player;
    Transform initialPosition;

    public void EnemyHit()
    {
        Debug.Log("Enemy has been hit");
        if (invincibilityTimer <= 0)
        {
            meleeHealth -= 1;
            meleeanimator.SetBool("hit", true);
            dir = player.GetComponent<Animator>().GetFloat("Direction");
            enemyBody.AddForce(UnityEngine.Vector2.left * 400.0f * dir + UnityEngine.Vector2.up * 400.0f);
            invincibilityTimer = invincibilityTime;
        }
    }

    private void Start()
    {
        meleeanimator = GetComponent<Animator>();
        MeleeGroundDetection = gameObject.transform.Find("MeleeGroundDetection").transform;
        MELEE_FIELD_OF_VIEW = GetComponent<PolygonCollider2D>();
        obstacle = GetComponentInChildren<LocalMeleeCollision>();
        player = GameObject.Find("Player");
        playerhealth = player.GetComponent<PlayerHealth>();
        enemyBody = GetComponent<Rigidbody2D>();
        initialPosition = transform;
        meleeanimator.SetBool("hit", false);
    }

    void EnemyIdle()
    {
        moving = false;
        meleespeed = 0f;
    }

    void EnemyRight()
    {
        moving = true;
        meleespeed = 2f;
        transform.eulerAngles = new Vector3(0, 0, 0);
        movingRight = true;
    }

    void EnemyChaseRight()
    {
        moving = true;
        meleespeed = 6f;
        transform.eulerAngles = new Vector3(0, 0, 0);
        movingRight = true;
    }

    void EnemyChaseLeft()
    {
        moving = true;
        meleespeed = 6f;
        transform.eulerAngles = new Vector3(0, -180, 0);
        movingRight = false;
    }
    void EnemyLeft()
    {
        moving = true;
        meleespeed = 2f;
        transform.eulerAngles = new Vector3(0, -180, 0);
        movingRight = false;
    }

    public void OnTriggerStay2D(Collider2D other) // CHASE MECHANIC COLLIDER
    {
        meleetriggered = true;
        tag = other.tag;
        otherCollider = other;
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        meleetriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (meleeHealth > 0)
        {


            bool anyobstacle = obstacle.obstacleIsMeleeThere;
            bool playerobstacle = obstacle.obstacleIsMeleePlayer;

            if (meleetriggered)
            {
                //Debug.Log("AHHH THERE IS SOMETHING IN SIGHT");
                positionOtherCollider = otherCollider.transform.position.x; //position of the player
                positionEnemyCollider = transform.position.x; //position of the enemy
            }

            transform.Translate(Vector2.right * meleespeed * Time.deltaTime); //enemy moves to the right by a certain meleespeed 

            RaycastHit2D groundState = Physics2D.Raycast(MeleeGroundDetection.position, Vector2.down, GROUND_DETECTION_DISTANCE);

            if (playerobstacle == true) //if the player is the obstacle within the attack region
            {
                attack = true;
                patrol = false;
                chase = false;
            }

            else if (tag == "Player" && meleetriggered == true) //if the player is within sight of the enemy 
            {
                chase = true;
                patrol = false;
                attack = false;

            }

            else if (meleetriggered == false) //if player is not within sight of the enemy
            {
                meleespeed = 2f;
                moving = true;
                patrol = true;
                chase = false;
                attack = false;
            }

            else
            {
                meleespeed = 2f;
                moving = true;
                patrol = true;
                chase = false;
                attack = false;
            }

            //finite state machine: states= patrol, chase, attack

            if (patrol == true) //if enemy is patrolling
            {

                //Debug.Log("MELEE: PATROLLING AROUND");

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

                else if (anyobstacle == true && playerobstacle == false) //if there is a obstacle blocking the enemy's right of way
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

            else if (chase == true && anyobstacle == false) //if enemy is chasing
            {
                //Debug.Log("MELEE: CHASING ENEMY");
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
                meleespeed = 0f;
                moving = false;
                //Debug.Log("MELEE: ATTACKING ENEMY");
            }

            meleeanimator.SetBool("moving", moving);
            meleeanimator.SetBool("attacking", attack);

            if (attack == true && playerobstacle) playerhealth.PlayerHit();

        }

        else ////basically if the enemy is dead
        {
            meleeanimator.SetTrigger("dead");
            meleeanimator.SetBool("hit", false);
            if (destroyDelay > 0) destroyDelay -= Time.deltaTime;
            else if (respawnable)
            {
                gameObject.SetActive(false);
                Invoke("Respawn", 2);
            }
            else Destroy(gameObject);
        }
    }

    float direction;

    private void FixedUpdate()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
            meleeanimator.SetBool("hit", true);
        }
        else
        {
            invincibilityTimer = 0;
            meleeanimator.SetBool("hit", false);
        } 

        if (movingRight) direction = 1;
        else direction = -1;
    }

    void Respawn()
    {
        GameObject newEnemy = Instantiate(gameObject, initialPosition.position + Vector3.up*2, UnityEngine.Quaternion.identity);
        newEnemy.SetActive(true);
        newEnemy.GetComponent<SpriteRenderer>().enabled = true;
        newEnemy.GetComponent<MeleeEnemy>().respawnable = true;
        Destroy(gameObject);
    }

}

