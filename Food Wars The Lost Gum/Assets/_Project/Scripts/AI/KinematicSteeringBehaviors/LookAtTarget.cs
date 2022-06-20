using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AI
{
    public class LookAtTarget : AIMovement
    {
        private void Awake()
        {
            m_movementType = AIMovementType.Angular;
        }
        public override SteeringOutput GetKinematic(AIAgent agent)
        {
            var output = base.GetKinematic(agent);

            Vector3 fromAgentToTarget = (agent.TargetPosition - agent.transform.position).normalized;

            if(fromAgentToTarget.magnitude == 0)
            {
                // The vectors are invalid. Return no rotation
                //fromAgentToTarget = agent.transform.forward;
                return output;
            }

            // Now that we are in 2D, we rotate our Y axis 90 degrees.
            Vector3 rotatedYAroundZ = Quaternion.Euler(0, 0, 90) * fromAgentToTarget;

            output.angular = Quaternion.LookRotation(Vector3.forward, rotatedYAroundZ);

            return output;
        }

        public override SteeringOutput GetSteering(AIAgent agent)
        {
            var output = base.GetSteering(agent);

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
