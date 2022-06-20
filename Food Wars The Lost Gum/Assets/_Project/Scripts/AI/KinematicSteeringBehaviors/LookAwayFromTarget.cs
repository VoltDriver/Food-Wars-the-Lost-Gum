using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class LookAwayFromTarget : LookAtTarget
    {
        private void Awake()
        {
            m_movementType = AIMovementType.Angular;
        }
        public override SteeringOutput GetKinematic(AIAgent agent)
        {
            var output = base.GetKinematic(agent);

            // Add 180 degrees to face away.
            output.angular = output.angular * Quaternion.Euler(0, 0, 180);

            return output;
        }

        public override SteeringOutput GetSteering(AIAgent agent)
        {
            var output = new SteeringOutput();
            output.angular = Quaternion.identity;
            output.linear = Vector3.zero;

            Vector3 fromAgentToTarget = (agent.TargetPosition - agent.transform.position).normalized;

            if (fromAgentToTarget.magnitude == 0)
            {
                // The vectors are invalid. Return no rotation
                //fromAgentToTarget = agent.transform.forward;
                return output;
            }

            if (agent.lockZ)
            {
                // get the rotation around the z-axis
                Vector3 from = Vector3.ProjectOnPlane(agent.transform.right, -Vector3.forward);
                Vector3 to = GetKinematic(agent).angular * Vector3.right;
                float angleZ = Vector3.SignedAngle(from, to, -Vector3.forward);
                output.angular = Quaternion.AngleAxis(angleZ, -Vector3.forward);
            }
            else
                output.angular = Quaternion.FromToRotation(agent.transform.forward, GetKinematic(agent).angular * -Vector3.forward);

            return output;
        }
    }
}

