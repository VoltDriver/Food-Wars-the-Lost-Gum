using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pea : MonoBehaviour
{
    private AIAgent m_agent;
    public GameObject DeathEffectPrefab;
    private Animator animator;

    public RuntimeAnimatorController normalAnim;
    public RuntimeAnimatorController fearAnim;

    public SoundPlayer m_deathSound;

    public SoundPlayer[] m_fleeSounds;

    /// <summary>
    /// Amount of time that must elapse between 2 triggers of the flee sound.
    /// </summary>
    public float m_fleeSoundCooldown = 10f;
    private float m_lastFleeTime = 0f;

    private void Awake()
    {
        m_agent = GetComponent<AIAgent>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // switch animation controller to fear when fleeing.
        if(m_agent.m_movementType == AIAgent.MovementType.AvoidTarget)
        {
            animator.runtimeAnimatorController = fearAnim;
        }
        // switch animation controller to normal when chasing.
        else
        {
            animator.runtimeAnimatorController = normalAnim;
        }
    }

    public void PlayFleeSound()
    {
        // Only play the sound if we just started fleeing.
        if(m_agent.m_lastMovementType == AIAgent.MovementType.AvoidTarget)
        {
            return;
        }

        if(Time.time - m_lastFleeTime >= m_fleeSoundCooldown)
        {
            // Play a sound effect
            if (m_fleeSounds != null && m_fleeSounds.Length > 0)
            {
                SoundPlayer sound = GameObject.Instantiate(m_fleeSounds[Random.Range(0, m_fleeSounds.Length)], transform.position, Quaternion.identity);
                m_lastFleeTime = Time.time;
            }
        }
    }

    // pea dies when touches player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // Play a sound effect
            if (m_deathSound != null)
            {
                SoundPlayer sound = GameObject.Instantiate(m_deathSound, transform.position, Quaternion.identity);
            }

            // spawn death effect
            GameObject deathEffect = Instantiate(DeathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathEffect, 0.5f);
            Destroy(gameObject);
        }
    }
}
