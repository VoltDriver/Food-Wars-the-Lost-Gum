using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI;
using UnityEngine;

namespace Assets._Project.Scripts.BehaviourTrees.Custom_Nodes
{
    public class TargetOutOfRangeNode : ActionNode
    {
        public bool m_considerTrackedEnemy;

        private AIAgent m_agent;
        private Shooter m_shooter;

        protected override void OnStart()
        {
            // Possible that this sometimes react unfortunately to a change in the owner object.
            // If that ends up being the case, we need to make it so that the BehaviourTree owner change calls a "OwnerChange()" method on all children.
            m_agent = Owner.GetComponent<AIAgent>();

            if (m_agent == null)
            {
                throw new System.Exception(this.GetType().Name + ": No AIAgent associated with this node's owner.");
            }

            m_shooter = Owner.GetComponent<Shooter>();

            if (m_shooter == null)
            {
                throw new System.Exception(this.GetType().Name + ": No Shooter script associated with this node's owner.");
            }
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            Vector3 distanceToTarget = m_agent.TargetPosition - m_agent.transform.position;

            if(m_considerTrackedEnemy)
            {
                if(m_agent.trackedEnemy != null)
                    distanceToTarget = m_agent.EnemyPosition - m_agent.transform.position;
            }

            // Checking if the target is outside of our shooting range.
            if (distanceToTarget.magnitude > m_shooter.m_shootingRange)
            {
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }
    }
}
