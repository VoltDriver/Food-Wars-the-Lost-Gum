using UnityEngine;
// CODE BY DANIEL RINALDI
// MODIFIED BY: Joel Lajoie-Corriveau
namespace AI
{
    public class Seek : AIMovement
    {
        private void Awake()
        {
            m_movementType = AIMovementType.Linear;
        }
        public override SteeringOutput GetKinematic(AIAgent agent)
        {
            var output = base.GetKinematic(agent);

            Vector3 desiredVelocity = agent.TargetPosition - agent.transform.position;
            desiredVelocity = desiredVelocity.normalized * agent.maxSpeed;

            output.linear = desiredVelocity;

            if (debug) Debug.DrawRay(transform.position, output.linear, Color.cyan);

            return output;
        }

        public override SteeringOutput GetSteering(AIAgent agent)
        {
            var output = base.GetSteering(agent);

            output.linear = GetKinematic(agent).linear - agent.Velocity;

            if (debug) Debug.DrawRay(transform.position + agent.Velocity, output.linear, Color.green);

            return output;
        }

        public static SteeringOutput GetStaticKinematic(AIAgent agent)
        {
            var output = new SteeringOutput();
            output.angular = Quaternion.identity;
            output.linear = Vector3.zero;

            Vector3 desiredVelocity = agent.TargetPosition - agent.transform.position;
            desiredVelocity = desiredVelocity.normalized * agent.maxSpeed;

            output.linear = desiredVelocity;

            return output;
        }

        public static SteeringOutput GetStaticSteering(AIAgent agent)
        {
            var output = new SteeringOutput();
            output.angular = Quaternion.identity;
            output.linear = Vector3.zero;

            output.linear = GetStaticKinematic(agent).linear - agent.Velocity;

            return output;
        }
    }
}
