using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class DamageHandler : MonoBehaviour
{
    //private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    //private NPCDummy dummy;
    private AIAgent agent;
    private Health health;

    // Start is called before the first frame update
    void Awake()
    {
        //boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        //dummy = GetComponent<NPCDummy>();
        agent = GetComponent<AIAgent>();
        health = GetComponent<Health>();
    }


    public void TakeDamage(int amount, Vector2 knockbackForce)
    {
        /* TESTING
        // remove NPC/Player health by amount
        dummy.health -= amount;
        // knockback the rigidbody by knockbackForce
        // A rigidbody with higher mass and linear drag will have a lesser knockback
        dummy.isHit = true;
        rigidBody.AddForce(knockbackForce, ForceMode2D.Impulse);

        dummy.CheckHealthStatus();
        */

        // Remove health and destroy npc if health <= 0
        health.RemoveHealth(amount);
        //health.CheckHealthStatus();   called in RemoveHealth

        // Temporarly disable movement for knockback effect. Reenabled in the coroutine
        //agent.m_movementAllowed = false;
        agent.applyingForce = true;
        rigidBody.AddForce(knockbackForce, ForceMode2D.Impulse);
        StartCoroutine(KnockbackRoutine());

    }

    IEnumerator KnockbackRoutine()
    {
        float knockbackTime = 0.3f;

        yield return new WaitForSeconds(knockbackTime);

        //agent.m_movementAllowed = true;
        agent.applyingForce = false;
    }
}
