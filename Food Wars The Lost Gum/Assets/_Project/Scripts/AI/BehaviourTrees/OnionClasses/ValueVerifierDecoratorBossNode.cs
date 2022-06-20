using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Decorator node to verify if this is the correct attack to execute
public class ValueVerifierDecoratorBossNode : DecoratorNode
{
    private bool executeThisAttack;
    public int thisAttackNumber;
    protected override void OnStart()
    {

        
        executeThisAttack = thisAttackNumber == Owner.GetComponent<OnionBossPhase1>().NextAttack;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (!executeThisAttack) return State.Failure;

        m_child.Update();

        return m_child.m_state;


    }
}

