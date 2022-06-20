using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackChooserBossNode : ActionNode
{
    public float m_range = 10f;
    public int m_minimumRequired = 1;

    private OnionBossPhase1 boss;

    protected override void OnStart()
    {
        // Possible that this sometimes react unfortunately to a change in the owner object.
        // If that ends up being the case, we need to make it so that the BehaviourTree owner change calls a "OwnerChange()" method on all children.
        boss = Owner.GetComponent<OnionBossPhase1>();

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {

        if (boss.LastAttack < 0)
        {
            boss.NextAttack = 0;
            boss.LastAttack = 0;
        }

        List<int> potential = new List<int>();

        if (boss.LastAttack != 0 && boss.agentParent.transform.childCount < 1) potential.Add(0);
        if (boss.LastAttack != 1 && boss.GetPlayerPosition().x < boss.thirdFifthX && boss.GetPlayerPosition().x > boss.secondFifthX)
            potential.Add(1);
        if (boss.LastAttack != 2 && boss.LastAttack != 3 && boss.GetPlayerPosition().x < boss.thirdFifthX)
            potential.Add(2);
        if (boss.LastAttack != 2 && boss.LastAttack != 3 && boss.GetPlayerPosition().x > boss.secondFifthX)
            potential.Add(3);
        if (boss.LastAttack != 4)
            potential.Add(4);
        if (boss.LastAttack != 5 && boss.Health <= boss.MaxHealth / 2)
            potential.Add(5);

        int choice = Random.Range(0, potential.Count);

        boss.NextAttack = potential[choice];
        boss.LastAttack = potential[choice];

        //Always returns failure, will need to set the value for next attack and continue
        return State.Failure;

    }
}
