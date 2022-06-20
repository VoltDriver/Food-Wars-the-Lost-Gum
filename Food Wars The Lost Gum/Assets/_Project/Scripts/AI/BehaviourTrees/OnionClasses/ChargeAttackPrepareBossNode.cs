using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackPrepareBossNode : ActionNode
{

    private float startTime;
    public float totalTime;
    private OnionBossPhase1 boss;
    private int startingHealth;
    public int damageToStopCharge;

    public SoundPlayer m_chargingSound;
    private SoundPlayer m_chargingSoundInstance;


    // Start is called before the first frame update
    protected override void OnStart()
    {
        boss = Owner.GetComponent<OnionBossPhase1>();
        boss.AnimationWrapper(8);
        startTime = Time.time;
        startingHealth = boss.Health;

        // Play a sound effect
        if (m_chargingSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_chargingSound, boss.transform.position, Quaternion.identity);
            m_chargingSoundInstance = sound;
        }
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        // Check if the time elapsed is greater than our wait time.
        if (Time.time > startTime + totalTime)
        {
            // We have waited long enough. Return success.
            return State.Success;
        }
        if (boss.Health <= startingHealth - damageToStopCharge)
        {
            if (m_chargingSoundInstance != null)
                m_chargingSoundInstance.Stop();

            return State.Failure;
        }
        return State.Running;
    }
}

