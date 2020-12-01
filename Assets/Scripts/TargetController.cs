using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetController : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam; //Main Camera
    public EnemyInView target; //Current Focused Enemy In List
    Image image;//Image Of Crosshair
    public PlayerController player;
    public PlayerCombat playerCombat;

    bool lockedOn;//Keeps Track Of Lock On Status    

    //Tracks Which Enemy In List Is Current Target
    int lockedEnemy;
    public UnityEngine.Vector2 crosshairPos = new UnityEngine.Vector2(-5000,5000);

    //List of nearby enemies
    public static List<EnemyInView> nearByEnemies = new List<EnemyInView>();
    void Start()
    {
        cam = Camera.main;
        image = gameObject.GetComponent<Image>();
        image.enabled = false;
        lockedOn = false;
        lockedEnemy = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Press Space Key To Lock On
        if (!lockedOn)//Input.GetKeyDown(KeyCode.Space) && 
        {
            if (nearByEnemies.Count >= 1)
            {
                lockedOn = true;
                image.enabled = true;

                //Lock On To First Enemy In List By Default
                lockedEnemy = 0;
                target = nearByEnemies[lockedEnemy];
            }
        }
        //Turn Off Lock On When Space Is Pressed Or No More Enemies Are In The List
        else if ((Input.GetKeyDown(KeyCode.Space) && lockedOn) || nearByEnemies.Count == 0)
        {
            lockedOn = false;
            image.enabled = false;
            lockedEnemy = 0;
            target = null;
        }

        //Press Z To Switch Targets
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (lockedEnemy == nearByEnemies.Count - 1 || lockedEnemy < 0)
            {
                //If End Of List Has Been Reached, Start Over
                lockedEnemy = 0;
                target = nearByEnemies[lockedEnemy];
                if(nearByEnemies[lockedEnemy] == null)
                {
                    foreach(EnemyInView enemy in nearByEnemies)
                    {
                        if (enemy != null) target = enemy;
                        lockedEnemy = nearByEnemies.IndexOf(enemy);
                    }
                    if (lockedEnemy <= 0)
                    {
                        lockedOn = false;
                        image.enabled = false;
                        lockedEnemy = 0;
                        target = null;
                    }
                }
            }
            else
            {
                //Move To Next Enemy In List
                lockedEnemy++;
                target = nearByEnemies[lockedEnemy];
            }
        }

        if (lockedOn)
        {
                target = nearByEnemies[lockedEnemy];
                //float distanceBetween = (player.GetComponent<Rigidbody2D>().position - (Vector2.up + Vector2.right) * target.transform.position).magnitude;
                //Determine Crosshair Location Based On The Current Target
                if (!playerCombat.enemyInSightBool) gameObject.transform.position = cam.WorldToScreenPoint(target.transform.position) + Vector3.up * -1000;
                else gameObject.transform.position = cam.WorldToScreenPoint(target.transform.position);

                crosshairPos = target.transform.position;
                //Rotate Crosshair
                gameObject.transform.Rotate(new Vector3(0, 0, -1));
           // else lockedOn = false;
        }
    }
}
