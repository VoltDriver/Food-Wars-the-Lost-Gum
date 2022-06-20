using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareAttackBossNode : ActionNode
{
    public int animToPlay;
    private float startTime;
    public float totalTime;

    public SoundPlayer[] m_prepareAttackSounds;

    private OnionBossPhase1 boss;


    // Start is called before the first frame update
    protected override void OnStart()
    {
        boss = Owner.GetComponent<OnionBossPhase1>();
        boss.AnimationWrapper(animToPlay);
        startTime = Time.time;

        // Play a sound effect
        if (m_prepareAttackSounds != null && m_prepareAttackSounds.Length > 0)
        {
            SoundPlayer sound = GameObject.Instantiate(m_prepareAttackSounds[Random.Range(0, m_prepareAttackSounds.Length)], boss.transform.position, Quaternion.identity);
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
