using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class AcquireTargetNode : ActionNode
{
    public string m_targetTag;
    public float m_checkRadius;
    public bool m_isEnemy = false;
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
        // Acquire target with specified tag
        GameObject target = m_agent.AcquireTarget(m_targetTag, m_checkRadius);

        if(target != null)
        {
            if (m_isEnemy)
                m_agent.trackedEnemy = target;
            else
                m_agent.TrackTarget(target.transform);

            return State.Success;
        }
        else
        {
            if (m_isEnemy)
                m_agent.trackedEnemy = null;
            else
                m_agent.UnTrackTarget();

            return State.Failure;
        }
    }
}
