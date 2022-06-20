using UnityEngine;
// CODE BY DANIEL RINALDI
// MODIFIED BY: Joel Lajoie-Corriveau
namespace AI
{
    public class LookWhereYouAreGoing : AIMovement
    {
        private void Awake()
        {
            m_movementType = AIMovementType.Angular;
        }
        public override SteeringOutput GetKinematic(AIAgent agent)
        {
            var output = base.GetKinematic(agent);

            if (agent.Velocity == Vector3.zero)
            {
                // return no rotation.
                return output;
            }

            // Now that we are in 2D, we rotate our Y axis 90 degrees.
            Vector3 rotatedYAroundZ = Quaternion.Euler(0, 0, 90) * agent.Velocity;

            output.angular = Quaternion.LookRotation(Vector3.forward, rotatedYAroundZ);

            return output;
        }

        public override SteeringOutput GetSteering(AIAgent agent)
        {
            var output = base.GetSteering(agent);

            if (agent.Velocity == Vector3.zero)
            {
                // return no rotation.
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