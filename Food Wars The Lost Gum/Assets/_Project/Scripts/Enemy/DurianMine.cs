using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurianMine : MonoBehaviour
{
    [HideInInspector]
    public GameObject m_player;
    public float m_armingRange = 1f;
    public float m_detonationDelay = 1f;
    public int m_numberOfProjectiles = 4;
    public float m_radiusOfProjectiles = 1f;


    public GameObject m_bulletPrefab;
    public GameObject m_explosionEffectPrefab;
    public Vector3 m_explosionEffectSize = new Vector3(1, 1, 1);

    public SoundPlayer m_durianExplosionSound;

    private bool m_isExploding = false;
    private float m_timePlayerCameInRange;

    private void Awake()
    {
        m_player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    void Start()
    {
        if(m_player == null)
        {
            m_player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distanceToPlayer = m_player.transform.position - this.transform.position;
        if(distanceToPlayer.magnitude <= m_armingRange)
        {
            if(!m_isExploding)
            {
                m_isExploding = true;
                m_timePlayerCameInRange = Time.time;
            }
        }

        if(m_isExploding)
        {
            if (Time.time - m_timePlayerCameInRange > m_detonationDelay)
            {

                // Time is up. Blow up.
                // Play a sound effect
                if (m_durianExplosionSound != null)
                {
                    SoundPlayer sound = GameObject.Instantiate(m_durianExplosionSound, transform.position, Quaternion.identity);
                }

                // Do an explosion effect.
                if (m_explosionEffectPrefab != null)
                {
                    GameObject explosion = GameObject.Instantiate(m_explosionEffectPrefab, transform.position, Quaternion.identity);
                    explosion.transform.localScale = m_explosionEffectSize;
                }

                // Spawn projectiles all around
                if (m_bulletPrefab != null)
                {
                    SpawnProjectilesInACircle(m_numberOfProjectiles, transform.position, m_radiusOfProjectiles, m_bulletPrefab);
                }

                Destroy(this.gameObject);
            }
        }
    }

    // Credits to: Bunny83 (https://answers.unity.com/questions/1068513/place-8-objects-around-a-target-gameobject.html) for the base script.
    // Modified by: Joel Lajoie-Corriveau

    /// <summary>
    /// Spawns a set amount of GameObjects evenly spread in a certain radius around target position.
    /// </summary>
    /// <param name="pNumberOfProjectiles">Number of objects to spawn.</param>
    /// <param name="pPosition">Initial position of the spawning circle (center).</param>
    /// <param name="pRadius">Radius of the spawning circle.</param>
    /// <param name="pProjectile">Prefab of the gameobject to instantiate.</param>
    public void SpawnProjectilesInACircle(int pNumberOfProjectiles, Vector3 pPosition, float pRadius, GameObject pProjectile)
    {
        // Loop once for each projectile...
        for (int i = 0; i < pNumberOfProjectiles; i++)
        {
            // Calculates the angle, in Radians, at which to spawn the projectile.
            // Mathf.PI * 2 is a full circle in radian. So we get how much of an angle is for one projectile, then multiply by the projectile
            // index we are at.
            float angle = i * Mathf.PI * 2f / pNumberOfProjectiles;
            // Cos of the angle is the X position, on the trigonometry circle, at which we spawn our projectile. Sin is the Y.
            Vector3 newPos = new Vector3(Mathf.Cos(angle) * pRadius, Mathf.Sin(angle) * pRadius, 0);
            // We add to the calculated position the circle center, to move it in the world.
            newPos = pPosition + newPos;

            Quaternion directionOfPrefab = Quaternion.identity;

            if(pProjectile.GetComponent<Bullets>() != null)
            {
                // This is a bullet prefab. We need to orient it.
                directionOfPrefab = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.forward);
            }

            GameObject gameObject = GameObject.Instantiate(pProjectile, newPos, directionOfPrefab);
        }
    }
}
