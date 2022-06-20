using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingObjectCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider2D _collider;
    public Node parentNode;

    public void Awake()
    {
        _collider = transform.GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // player take damage if not invincible
            if (!collision.gameObject.GetComponent<PlayerHealth>().isInvincible)
                collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(1);
            if (parentNode != null)
                parentNode.CollisionDetected();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // player take damage if not invincible
            if (!collision.gameObject.GetComponent<PlayerHealth>().isInvincible)
                collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(1);
            if (parentNode != null)
                parentNode.CollisionDetected();
        }
    }

}
