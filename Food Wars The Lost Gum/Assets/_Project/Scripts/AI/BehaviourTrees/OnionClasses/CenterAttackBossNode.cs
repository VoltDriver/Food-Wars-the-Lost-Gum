using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterAttackBossNode : ActionNode
{

    private float startTime;
    public float totalTime;
    private OnionBossPhase1 boss;
    public float rootSpawnZValue;
    public float rootSpawnYValue;

    public SoundPlayer m_attackSound;
    public SoundPlayer[] m_angerSounds;

    private GameObject root;

    protected override void OnStart()
    {
        boss = Owner.GetComponent<OnionBossPhase1>();
        boss.AnimationWrapper(7); 
        startTime = Time.time;
        root = Instantiate(boss.floorVine, new Vector3((boss.thirdFifthX + boss.secondFifthX)/2, rootSpawnYValue, rootSpawnZValue), Quaternion.identity);

        // Play a sound effect
        if (m_attackSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_attackSound, root.transform.position, Quaternion.identity);
        }

        // Play an anger sound effect
        if (m_angerSounds != null && m_angerSounds.Length > 0)
        {
            SoundPlayer sound = GameObject.Instantiate(m_angerSounds[Random.Range(0, m_angerSounds.Length)], boss.transform.position, Quaternion.identity);
        }
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {

        if (Time.time > startTime + totalTime)
        {
            Destroy(root);
            return State.Success;
        }

        return State.Running;
    }
}
