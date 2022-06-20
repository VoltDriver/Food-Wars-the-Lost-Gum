using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public float m_shootingRange = 10f;
    public float m_shootingCooldown = 1f;

    public Bullets m_bulletPrefab;
    public float m_canonOffset = 1f;

    private AIAgent m_agent;
    private bool m_shootingOnCooldown = false;

    private void Awake()
    {
        m_agent = GetComponent<AIAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_agent.m_isAgressive)
        {
            if(m_agent.trackedEnemy != null)
            {
                if((m_agent.EnemyPosition - transform.position).magnitude < m_shootingRange)
                {         
                    if(!m_shootingOnCooldown)
                    {
                        // Shoot the target
                        StartCoroutine(ShootCooldown(m_shootingCooldown));
                        Shoot();
                    }           
                }
            }
        }
    }

    public void Shoot()
    {
        if (m_bulletPrefab != null)
        {
            // This is a bullet prefab. We need to orient it.
            Vector3 targetDirection;

            if (m_agent.trackedEnemy != null)
            {
                targetDirection = (m_agent.EnemyPosition - transform.position).normalized;
            }          
            else
            {
                targetDirection = (m_agent.TargetPosition - transform.position).normalized;
            }
            targetDirection = new Vector3(targetDirection.x, targetDirection.y, 0); // Stripping z component.

            /*float angle = Vector3.Angle(transform.up, targetDirection);
            Quaternion directionOfPrefab = Quaternion.AngleAxis(angle, Vector3.forward);*/
            Quaternion directionOfPrefab = Quaternion.LookRotation(Vector3.forward, targetDirection);
            directionOfPrefab = Quaternion.AngleAxis(0f, Vector3.forward) * directionOfPrefab;

            Bullets bullet = Instantiate(m_bulletPrefab, transform.position + m_canonOffset * targetDirection, directionOfPrefab);
        }
    }

    // Taken from StackOverFlow ( https://answers.unity.com/questions/796881/c-how-can-i-let-something-happen-after-a-small-del.html) 
    // Modified by Joel.
    IEnumerator ShootCooldown(float pTimeToWait)
    {
        if (m_shootingOnCooldown)
            yield break;

        m_shootingOnCooldown = true;

        yield return new WaitForSeconds(pTimeToWait);

        m_shootingOnCooldown = false;
    }
}
