using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalMeleeCollision : MonoBehaviour
{
    public bool obstacleIsMeleeThere = false;
    public bool obstacleIsMeleePlayer = false;
    public BoxCollider2D MELEE_CHASE_COLLIDER;


    private void Start()
    {
        MELEE_CHASE_COLLIDER = GetComponent<BoxCollider2D>();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("Melee enemy has collided with something!");
        tag = other.tag;
        obstacleIsMeleeThere = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        obstacleIsMeleeThere = false;
    }

    public void FixedUpdate()
    {
        if (tag == "Player" && obstacleIsMeleeThere == true)
        {
            Debug.Log("ENEMY SHOULD ATTACK");
            obstacleIsMeleePlayer = true;
        }
        else
        {
            obstacleIsMeleePlayer = false;
        }
    }


}
