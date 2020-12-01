using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//CREDIT TO GRIMOIRE
public class EnemyInView : MonoBehaviour
{
    // Start is called before the first frame update
    Camera cam;//Camera Used To Detect Enemies On Screen      
    bool addOnlyOnce;//This Boolean Is Used To Only Allow The Enemy To Be Added To The List Once
    void Start()
    {
        cam = Camera.main;
        addOnlyOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        //First Create A Vector3 With Dimensions Based On The Camera's Viewport
        Vector3 enemyPosition = cam.WorldToViewportPoint(gameObject.transform.position);

        //If The X And Y Values Are Between 0 And 1, The Enemy Is On Screen
        bool onScreen = enemyPosition.z > 0 && enemyPosition.x > 0 && enemyPosition.x < 1 && enemyPosition.y > 0 && enemyPosition.y < 1;

        //If The Enemy Is On Screen Add It To The List Of Nearby Enemies Only Once
        if (onScreen && addOnlyOnce)
        {
            addOnlyOnce = false;
            TargetController.nearByEnemies.Add(this);
        }
    }
}
