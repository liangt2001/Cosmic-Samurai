using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    Animator animator;
    public Transform attackPoint;
    Rigidbody2D rigidbody2D;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    const float offsetAttackPoint = 1.0f;
    public TargetController targetController;
    UnityEngine.Vector2 directionPlayerToEnemy;
    public float attackingSpeed = 30.0f;
    [SerializeField] private LayerMask platformLayerMask;
    UnityEngine.Vector2 tempOffset = new UnityEngine.Vector2(0, 0.5f);
    const float homingAttackTime = 1.0f;
    float homingAttackTimer = 0.0f;

    //Takes 2 seconds for the recharging meter to fill up
    const float rechargeTime = 2.0f;
    float rechargeTimer = 0.0f;
    public RechargeTargetScript rechargeTarget;
    public ParticleSystem teleportationEffect;
    public ParticleSystem teleportationInEffect;
    private ParticleSystem clonedEffect;
    private ParticleSystem clonedInEffect;
    public PointsScript points;

    public bool enemyInSightBool = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        directionPlayerToEnemy = (rigidbody2D.position - targetController.crosshairPos).normalized;
        enemyInSightBool = enemyInSight(directionPlayerToEnemy, rigidbody2D.position - targetController.crosshairPos - tempOffset);
        RaycastHit2D raycast = Physics2D.Raycast(rigidbody2D.position, directionPlayerToEnemy, 2.0f, enemyLayers);

        attackPoint.localPosition = new UnityEngine.Vector2(Mathf.Abs(attackPoint.localPosition.x) * animator.GetFloat("Direction"), attackPoint.localPosition.y);
        if (Input.GetKeyDown(KeyCode.C))
        {            
            animator.SetTrigger("GroundAttack");
            FindObjectOfType<AudioManager>().Play("PlayerAttack");
            Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, 1f);
            foreach (Collider2D enemy in enemiesHit)
            {
                Debug.Log(enemy.tag);
                Debug.Log("We got this far");
                points.updatePoints(100);
                enemy.GetComponent<MeleeEnemy>().EnemyHit();
            }
        }
        //If the enemy is in sight and you decide to teleport to them, makes sure there are meters in the bar to fill up
        if(enemyInSightBool && homingAttackTimer <= 0 && Input.GetKeyDown(KeyCode.X) && rechargeTarget.slider.value > 0)
        {
            homingAttackTimer = homingAttackTime;
            //Decrease the slider by 1 when it reaches this conditional
            rechargeTarget.setRecharge(rechargeTarget.slider.value - 1.0f);
            rechargeTimer = rechargeTime;
            UnityEngine.Vector2 temp = rigidbody2D.position;
            clonedInEffect = Instantiate(teleportationInEffect, temp, UnityEngine.Quaternion.identity);
            rigidbody2D.position = targetController.crosshairPos + UnityEngine.Vector2.up;
            targetController.target.transform.position = temp + UnityEngine.Vector2.up;
            //Creates particle effect at player's NEW Position
            clonedEffect = Instantiate(teleportationEffect, targetController.crosshairPos + UnityEngine.Vector2.up, UnityEngine.Quaternion.identity);
            clonedEffect.transform.position = new UnityEngine.Vector3(clonedInEffect.transform.position.x, clonedInEffect.transform.position.y, 100);
        }

        //Delay the homing attack (the switch mechanic) so you can't use it again till it reaches a certain time
        if (rechargeTimer > 0.0f) rechargeTimer -= Time.deltaTime;
        //If all three are already filled up, then no need to set the recharge again
        else if (rechargeTarget.slider.value >= 3) rechargeTimer = 0.0f;
        //If there are less than 3 bars filled up, fills up the next one
        else
        {
            rechargeTimer = rechargeTime;
            rechargeTarget.setRecharge(rechargeTarget.slider.value + 1.0f);
        }

        if (homingAttackTimer > 0.0f)
        {
            homingAttackTimer -= Time.deltaTime;
        }

        else
        {
            homingAttackTimer = 0.0f;
            Destroy(clonedEffect);
            Destroy(clonedInEffect);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //Rotate across y-axis instead of transforming (alternative implementation to consider)
        if(animator.GetFloat("Direction") == 1)
        {
            attackPoint.position = rigidbody2D.position + UnityEngine.Vector2.right * offsetAttackPoint + UnityEngine.Vector2.up * 0.5f;
        }
        else
            attackPoint.position = rigidbody2D.position + UnityEngine.Vector2.left * offsetAttackPoint + UnityEngine.Vector2.up * 0.5f;
    }

    private Boolean enemyInSight(UnityEngine.Vector2 direction, UnityEngine.Vector2 distance)
    {
        //Original dimesnsions of collider
        //UnityEngine.Vector2 boxDimensions = new UnityEngine.Vector2(Mathf.Clamp(Math.Abs(rigidbody2D.position.x - targetController.crosshairPos.x), 8.0f, 15) + 10, Mathf.Clamp(Math.Abs(rigidbody2D.position.y - targetController.crosshairPos.y), 0.0f, 10.5f) + 5.0f);
        UnityEngine.Vector2 boxDimensions = new UnityEngine.Vector2(distance.magnitude, 0.5f);
        RaycastHit2D newTargetCollider = Physics2D.Raycast(rigidbody2D.position + tempOffset*-1, direction * -1, 10.0f, enemyLayers);
        RaycastHit2D platformCollider = Physics2D.Raycast(rigidbody2D.position + tempOffset * (1.1f), direction * -1, 10.0f, platformLayerMask);
        Debug.DrawRay(rigidbody2D.position + tempOffset*-1, direction * 10.0f * -1, Color.green);
        Debug.DrawRay(rigidbody2D.position + tempOffset*(1.1f), direction * 10.0f * -1, Color.blue);
        //Makes sure there is a minimum distance that the player can attack 
        return (platformCollider.collider == null || platformCollider.distance > newTargetCollider.distance) && newTargetCollider.collider != null && distance.magnitude >= 1.0f && distance.magnitude <= 10.0f;
    }

    //Draws the hitbox player attacks the enemy on(only for debugging, comment this out later)
    /*void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        UnityEngine.Vector2 distance = rigidbody2D.position - targetController.crosshairPos;
        UnityEngine.Vector2 boxDimensions = new UnityEngine.Vector2(distance.magnitude, 0.5f);
    }*/
}
