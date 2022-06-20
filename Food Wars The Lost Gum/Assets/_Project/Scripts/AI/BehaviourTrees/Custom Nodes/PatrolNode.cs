using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;


public class PatrolNode : ActionNode
{

    private AIAgent m_agent;
    private PatrolPoints m_patrolPoints;
    private float arriveRadius = 0.2f;

    protected override void OnStart()
    {
        // Possible that this sometimes react unfortunately to a change in the owner object.
        // If that ends up being the case, we need to make it so that the BehaviourTree owner change calls a "OwnerChange()" method on all children.
        m_agent = Owner.GetComponent<AIAgent>();
        m_patrolPoints = Owner.GetComponent<PatrolPoints>();

        if (m_agent == null)
        {
            throw new System.Exception(this.GetType().Name + ": No AIAgent associated with this node's owner.");
        }

        if (m_patrolPoints == null)
        {
            throw new System.Exception(this.GetType().Name + ": No PatrolPoints associated with this node's owner.");
        }

        if (m_patrolPoints.patrolPoints.Count <= 0)
        {
            throw new System.Exception(this.GetType().Name + ": 0 PatrolPoints");
        }

        m_agent.trackedTarget = m_patrolPoints.patrolPoints[m_patrolPoints.currTargetIndex];
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        
        if(m_patrolPoints.patrolPoints.Count <= 0)
        {
            return State.Failure;
        }

        float distance = Vector3.Distance(m_agent.transform.position, m_patrolPoints.patrolPoints[m_patrolPoints.currTargetIndex].position);
        if (distance <= arriveRadius)
        {
            m_patrolPoints.updateIndex();
            m_agent.trackedTarget = m_patrolPoints.patrolPoints[m_patrolPoints.currTargetIndex];
        }

        Debug.Log("Patrol");
        return State.Success;
    }
}
