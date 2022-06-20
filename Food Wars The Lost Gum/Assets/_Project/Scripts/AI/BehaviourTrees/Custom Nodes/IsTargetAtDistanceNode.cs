using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

public class IsTargetAtDistanceNode : ActionNode
{
    public float m_targetDistance = 10f;
    public float m_maxDelta = 0.1f;

    public bool m_considerTrackedEnemy = false;

    private float m_minDistance;
    private float m_maxDistance;

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

        m_minDistance = m_targetDistance - m_maxDelta;
        m_maxDistance = m_targetDistance + m_maxDelta;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        Vector3 distanceToTarget = m_agent.transform.position - m_agent.TargetPosition;

        if (m_considerTrackedEnemy)
        {
            if (m_agent.trackedEnemy != null)
            {
                distanceToTarget = m_agent.transform.position - m_agent.trackedEnemy.transform.position;
            }
        }

        if (distanceToTarget.magnitude >= m_minDistance &&
            distanceToTarget.magnitude <= m_maxDistance)
        {
            // Target is between m_minDistance and m_maxDistance.
            return State.Success;
        }
        else
        {
            return State.Failure;
        }
    }
}
