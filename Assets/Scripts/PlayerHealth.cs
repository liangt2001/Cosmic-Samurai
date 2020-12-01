using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    const int maxHealth = 3;
    const float invincibilityTime = 1.0f;
    float invincibilityTimer = 0;
    int currentHealth = 3;
    const float forceTime = 0.5f;
    float forceTimer = 0.0f;
    Rigidbody2D rigidbody2D;
    public HealthBarScript healthBar;
    Animator animator;
    // Start is called before the first frame update


    public void PlayerHit()
    {
        if (invincibilityTimer > 0) return;
        else
        {
            currentHealth = Mathf.Clamp(currentHealth - 1, 0, maxHealth);
            healthBar.setHealth(currentHealth);
            if (currentHealth > 0)
            {
                rigidbody2D.AddForce(Vector2.left * 400.0f * animator.GetFloat("Direction") + Vector2.up * 400.0f);
                animator.SetTrigger("Hurt");
                invincibilityTimer = invincibilityTime;
                FindObjectOfType<AudioManager>().Play("PlayerHit");
            }
            //When player's health is equal to 0, then they die
            else
            {
                animator.SetTrigger("Death");
                PlayerController player = GetComponent<PlayerController>();
                player.enabled = false;
                PlayerCombat playerCombat = GetComponent<PlayerCombat>();
                playerCombat.enabled = false;
                //Game over should play here

                FindObjectOfType<AudioManager>().Play("Death");
            }

        }

    }

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (invincibilityTimer > 0) invincibilityTimer -= Time.deltaTime;
        else invincibilityTimer = 0;
        if (forceTimer > 0)
        {
            forceTimer -= Time.fixedDeltaTime;
            // rigidbody2D.AddForce(transform.right * 15.0f * -animator.GetFloat("Direction") + transform.up * 30.0f);
        }
        else forceTimer = 0;
    }
}