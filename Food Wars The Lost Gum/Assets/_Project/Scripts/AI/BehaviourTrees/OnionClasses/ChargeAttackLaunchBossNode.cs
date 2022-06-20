using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackLaunchBossNode : ActionNode
{
    private float startTime;
    public float totalTime;
    private OnionBossPhase1 boss;
    public float rootSpawnZValue;
    public float rootSpawnYValue;

    public SoundPlayer m_chargeAttackSound;

    private List<GameObject> roots;

    protected override void OnStart()
    {
        boss = Owner.GetComponent<OnionBossPhase1>();
        boss.AnimationWrapper(7);
        startTime = Time.time;
        roots = new List<GameObject>();

        roots.Add(Instantiate(boss.floorVine, new Vector3((boss.firstFifthX + boss.leftWallX)/2, rootSpawnYValue, rootSpawnZValue), Quaternion.identity));
        roots.Add(Instantiate(boss.floorVine, new Vector3((boss.secondFifthX + boss.firstFifthX)/2, rootSpawnYValue, rootSpawnZValue), Quaternion.identity));
        roots.Add(Instantiate(boss.floorVine, new Vector3((boss.thirdFifthX + boss.secondFifthX)/2, rootSpawnYValue, rootSpawnZValue), Quaternion.identity));
        roots.Add(Instantiate(boss.floorVine, new Vector3((boss.fourthFifthX + boss.thirdFifthX)/2, rootSpawnYValue, rootSpawnZValue), Quaternion.identity));
        roots.Add(Instantiate(boss.floorVine, new Vector3((boss.rightWallX + boss.fourthFifthX)/2, rootSpawnYValue, rootSpawnZValue), Quaternion.identity));

        // Play a sound effect
        if (m_chargeAttackSound != null)
        {
            SoundPlayer sound = GameObject.Instantiate(m_chargeAttackSound, boss.transform.position, Quaternion.identity);
        }
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {

        if (Time.time > startTime + totalTime)
        {
            foreach (GameObject r in roots)
            {
                Destroy(r);
            }
            roots.Clear();
            return State.Success;
        }

        return State.Running;
    }
}
