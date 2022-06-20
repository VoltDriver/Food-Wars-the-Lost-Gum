using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasLineOfSightNode : ActionNode
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
        Vector3 targetPosition;

        if(m_agent.trackedEnemy != null)
        {
            targetPosition = m_agent.EnemyPosition;
        }
        else
        {
            targetPosition = m_agent.TargetPosition;
        }


        Vector3 direction = (targetPosition) - (m_agent.transform.position);

        // Checks if the path to the next node is clear of obstacles
        float colliderCheckHeight = direction.magnitude;

        Vector2 colliderCheckCenter = new Vector2(m_agent.transform.position.x, m_agent.transform.position.y) + new Vector2(direction.x / 2, direction.y / 2);
        bool obstacleInTheWay = Physics2D.OverlapBox(colliderCheckCenter,
                                        new Vector2(m_agent.m_pathfindingWidth, colliderCheckHeight),
                                        360f - Vector3.Angle(new Vector3(0, 1, 0), direction),
                                        LayerMask.GetMask("Obstacle")
                                        );

        if (obstacleInTheWay)
            return State.Failure;
        else
            return State.Success;
    }
}
