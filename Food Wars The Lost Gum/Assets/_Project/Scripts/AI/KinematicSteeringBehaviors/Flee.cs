using UnityEngine;
// CODE BY DANIEL RINALDI
// MODIFIED BY: Joel Lajoie-Corriveau
namespace AI
{
    public class Flee : Seek
    {
        private void Awake()
        {
            m_movementType = AIMovementType.Linear;
        }
        public override SteeringOutput GetKinematic(AIAgent agent)
        {
            var output = base.GetKinematic(agent);

            output.linear = -base.GetKinematic(agent).linear;

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
    }
}
