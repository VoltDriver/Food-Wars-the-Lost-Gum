using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class MoveTowardsNode : ActionNode
{
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
        // Set the agent to reach target.
        m_agent.SwitchMovementType(AIAgent.MovementType.ReachTarget);
        m_agent.m_movementAllowed = true;
        return State.Success;
    }
}
