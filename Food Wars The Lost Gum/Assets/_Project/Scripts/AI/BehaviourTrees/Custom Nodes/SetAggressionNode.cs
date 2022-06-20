using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class SetAggressionNode : ActionNode
{
    public bool m_setAgressive;

    private AIAgent m_agent;

    protected override void OnStart()
    {
        // Possible that this sometimes react unfortunately to a change in the owner object.
        // If that ends up being the case, we need to make it so that the BehaviourTree owner change calls a "OwnerChange()" method on all children.
        m_agent = Owner.GetComponent<AIAgent>();

        if (m_agent == null)
        {
            throw new System.Exception(this.GetType().Name + ": No AIAgent associated with this node's owner.");
        }
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        // Change aggressiveness of shooter.
        m_agent.m_isAgressive = m_setAgressive;

        return State.Success;
    }
}
