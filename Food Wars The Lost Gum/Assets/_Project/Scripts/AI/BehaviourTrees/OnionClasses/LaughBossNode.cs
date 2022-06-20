using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaughBossNode : ActionNode
{

    private float startTime;
    public float totalTime;

    public SoundPlayer[] m_laughSounds;

    private OnionBossPhase1 boss;


    // Start is called before the first frame update
    protected override void OnStart()
    {
        boss = Owner.GetComponent<OnionBossPhase1>();
        boss.AnimationWrapper(5);
        startTime = Time.time;

        // Play a sound effect
        if (m_laughSounds != null && m_laughSounds.Length > 0)
        {
            SoundPlayer sound = GameObject.Instantiate(m_laughSounds[Random.Range(0, m_laughSounds.Length)], boss.transform.position, Quaternion.identity);
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

        return State.Running;
    }
}
