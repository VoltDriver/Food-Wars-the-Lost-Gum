using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public int m_lowHealth = 1;
    public bool isInvincible;   // for dash
    bool tookDamage;            // for damage
    [SerializeField]
    float flashingTime = 0.1f;
    float flashTimer;
    [SerializeField]
    float preventDamageTime = 2f;
    float timer;
    SpriteRenderer renderer;
    private Animator animator;

    public SoundPlayer m_playerDamageSound;
    public SoundPlayer m_deathSound;
    public SoundPlayer m_gameOverSound;

    public SoundPlayer m_lowHealthSound;
    private SoundPlayer m_lowHealthSoundInstance;

    public SoundPlayer m_healthPickupSound;

    public GameOverScreen GameOverPanel;

    public AudioSource m_ambientMusic;

    private PlayerControl playerControl;

    // Start is called before the first frame update
    void Awake()
    {
        isInvincible = false;
        tookDamage = false;
        currentHealth = maxHealth;
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (tookDamage && currentHealth > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                renderer.enabled = true;
                tookDamage = false;
            }
            else
            {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0)
                {
                    renderer.enabled = !renderer.enabled;
                    flashTimer = flashingTime;
                }
            }

        }
    }

    void RegenerateHealth(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }

        if(currentHealth > m_lowHealth)
        {
            if(m_lowHealthSoundInstance != null)
            {
                m_lowHealthSoundInstance.Stop();
                m_lowHealthSoundInstance = null;
            }
        }

        // Play a sound effect
        if (m_healthPickupSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_healthPickupSound, transform.position, Quaternion.identity);
        }

        CheckHealthStatus();
    }

    public void DamagePlayer(int amount)
    {
        if (!tookDamage && !playerControl.isGodModeActive)
        {
            currentHealth = Mathf.Max(currentHealth - amount, 0);
            Debug.Log("Player took damage");

            if (currentHealth <= m_lowHealth && m_lowHealthSoundInstance == null)
            {
                // Play a sound effect
                if (m_lowHealthSound != null)
                {
                    SoundPlayer sound = GameObject.Instantiate(m_lowHealthSound, transform.position, Quaternion.identity);
                    m_lowHealthSoundInstance = sound;
                }
            }

            // Play a sound effect
            if (m_playerDamageSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_playerDamageSound, transform.position, Quaternion.identity);
            }

            if (timer <= 0)
            {
                tookDamage = true;
                timer = preventDamageTime;
                flashTimer = flashingTime;
            }

            CheckHealthStatus();
        }
    }

    void CheckHealthStatus()
    {
        Debug.Log("Player's Health: " + currentHealth + "/" + maxHealth);
        if (currentHealth == 0)
        {
            Debug.Log("Player is dead. How pathetic...");

            // Stop ambient music.
            if (m_ambientMusic != null && m_ambientMusic.isPlaying)
            {
                m_ambientMusic.Stop();
            }

            // Play a sound effect
            if (m_deathSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_deathSound, transform.position, Quaternion.identity);
            }

            // Play a sound effect
            if (m_gameOverSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_gameOverSound, transform.position, Quaternion.identity);
            }

            // Stop low health sound.
            if (m_lowHealthSoundInstance != null)
            {
                m_lowHealthSoundInstance.Stop();
                m_lowHealthSoundInstance = null;
            }

            // play death animation
            animator.SetTrigger("death");
            // disable player control
            playerControl.canMove = false;
        }
    }

    // Collect health
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SmallHealthDrop"))
        {
            RegenerateHealth(1); 
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("BigHealthDrop"))
        {
            RegenerateHealth(3);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("TriggerAttack"))
        {
            if(!isInvincible)
                DamagePlayer(1);
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            DamagePlayer(1);
        }
    }

    public void ShowGameOverPanel()
    {
        GameOverPanel.Pause();
    }

}
