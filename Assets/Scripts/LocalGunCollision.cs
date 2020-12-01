using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGunCollision : MonoBehaviour
{
    public bool obstacleIsThere;
    public bool obstacleIsPlayer;
    public BoxCollider2D CHASE_COLLIDER;

    public void OnTriggerStay2D(Collider2D collider)
    {
        tag = collider.tag;
        obstacleIsThere = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        obstacleIsThere = false;
    }

    public void Update()
    { 
        if (tag == "Player")
        {
            obstacleIsPlayer = true;
        }
        else
        {
            obstacleIsPlayer = false;
        }


        if (obstacleIsThere == false)
        {
            obstacleIsPlayer = false;
        }
    }
    


    
}
