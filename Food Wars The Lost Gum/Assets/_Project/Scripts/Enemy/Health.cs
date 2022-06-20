using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    private LootDrop loot;
    public GameObject DeathEffectPrefab;
    bool isDead;

    public SoundPlayer m_deathSound;
    public SoundPlayer m_spawnSound;
    public SoundPlayer[] m_damagedSounds;

    [SerializeField] private bool spawnUponDeath = false;
    [SerializeField] private GameObject[] SpawnEnemyPrefabs;

    // Start is called before the first frame update
    void Awake()
    {
        loot = GetComponent<LootDrop>();
        isDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckHealthStatus()
    {
        Debug.Log("Health: " + health);
        if (health <= 0 && !isDead)
        {
            isDead = true;
            loot.DropLoot();

            // spawn death effect
            GameObject deathEffect = Instantiate(DeathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathEffect, 0.5f);

            // Play a sound effect
            if (m_deathSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_deathSound, transform.position, Quaternion.identity);
            }

            // spawn enemy
            if (spawnUponDeath)
            {
                foreach(GameObject g in SpawnEnemyPrefabs)
                {
                    Vector3 randomPosition = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                    Instantiate(g, transform.position + randomPosition, Quaternion.identity);
                    // Play a sound effect
                    if (m_spawnSound != null)
                    {
                        SoundPlayer sound = GameObject.Instantiate(m_spawnSound, transform.position + randomPosition, Quaternion.identity);
                    }
                }
            }

            Destroy(gameObject);
        }
    }

    public void RemoveHealth(int amount)
    {
        health -= amount;

        // Play a sound effect
        if (m_damagedSounds != null && m_damagedSounds.Length > 0)
        {
            SoundPlayer sound = GameObject.Instantiate(m_damagedSounds[Random.Range(0, m_damagedSounds.Length)], transform.position, Quaternion.identity);
        }

        CheckHealthStatus();
    }
}
