using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullets : MonoBehaviour
{
    public GameObject explosionPrefab;
    public GameObject bulletHitEffect;
    public float timeToLive = 5f;
    public float speed;
    public int damage;
    public float knockbackForce;
    public bool isExplosive;
    public bool spin;

    public SoundPlayer m_explosionSound;

    private bool flaggedForDestruction = false;
    private Vector3 up;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DelayedDestruction(timeToLive));
        up = transform.up;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += up * speed * Time.deltaTime;

        if (spin)
            transform.Rotate(Vector3.forward * 1000 * Time.deltaTime);
    }

    IEnumerator DelayedDestruction(float pTimeToWait)
    {
        if (flaggedForDestruction)
            yield break;

        flaggedForDestruction = true;

        yield return new WaitForSeconds(pTimeToWait);

        Destroy(this.gameObject);
    }

    private void Explosion(int damage, float force, Vector3 position)
    {
        // Play a sound effect
        if (m_explosionSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_explosionSound, transform.position, Quaternion.identity);
        }

        float circleRadius = 3f;
        LayerMask layer = LayerMask.GetMask("Characters");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, circleRadius, layer);
        foreach (Collider2D coll in colliders)
        {
            Vector2 direction = new Vector2(coll.transform.position.x - position.x, coll.transform.position.y - position.y);
            float distance = direction.magnitude;

            Vector2 explosionForce = direction.normalized * (force / Mathf.Max(1, distance));
            int explosionDamage = damage - (int)(distance * damage / 10);

            coll.gameObject.GetComponent<DamageHandler>().TakeDamage(explosionDamage, explosionForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit a player!");
            // player take damage if not invincible
            if(!collision.gameObject.GetComponent<PlayerHealth>().isInvincible)
                collision.gameObject.GetComponent<PlayerHealth>().DamagePlayer(1);
        }
        else if (collision.gameObject.CompareTag("NPC") || collision.gameObject.CompareTag("Enemy"))
        {
            Vector2 force = up * knockbackForce;
            collision.gameObject.GetComponent<DamageHandler>().TakeDamage(damage, force);

            if (isExplosive)
            {
                Explosion(damage, knockbackForce, collision.transform.position);
            }
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            collision.gameObject.GetComponent<OnionBossPhase1>().HurtTheBoss(damage);
        }
        else
        {
            Debug.Log("Hit something else.");
        }

        // generate Effect
        if (!isExplosive)
        {
            GameObject effect = Instantiate(bulletHitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);
        }
        else
        {
            // Play a sound effect
            if (m_explosionSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_explosionSound, transform.position, Quaternion.identity);
            }
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(this.gameObject);
    }
}
