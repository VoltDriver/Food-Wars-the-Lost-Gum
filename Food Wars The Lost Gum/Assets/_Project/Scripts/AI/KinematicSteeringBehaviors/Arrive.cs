using UnityEngine;
// CODE BY DANIEL RINALDI
// MODIFIED BY: Joel Lajoie-Corriveau
namespace AI
{
    public class Arrive : AIMovement
    {
        public bool m_overrideAgent = false;
        public float slowRadius = 5f;
        public float stopRadius = 1f;
        /// <summary>
        /// Determines the minimum speed the agent can be at inside the stop radius before being stopped.
        /// </summary>
        public float stopSpeed = 0.5f;

        private void Awake()
        {
            m_movementType = AIMovementType.Linear;
        }

        private void DrawDebug(AIAgent agent)
        {
            if (debug)
            {
                DebugUtil.DrawCircle(agent.TargetPosition, transform.forward, Color.yellow, stopRadius);
                DebugUtil.DrawCircle(agent.TargetPosition, transform.forward, Color.magenta, slowRadius);
            }
        }

        public override SteeringOutput GetKinematic(AIAgent agent)
        {
            DrawDebug(agent);

            SetValues(agent);

            var output = base.GetKinematic(agent);

            Vector3 fromTargetToCharacter = agent.TargetPosition - agent.transform.position;
            Vector3 directionVector = fromTargetToCharacter.normalized * agent.maxSpeed;

            if (fromTargetToCharacter.magnitude > agent.m_satisfactionRadius)
            {
                float desiredVelocity = Mathf.Min(agent.maxSpeed, fromTargetToCharacter.magnitude / agent.m_timeToTarget);
                directionVector = Vector3.ClampMagnitude(directionVector, desiredVelocity);
            }
            else
            {
                directionVector = Vector3.zero;
            }

            output.linear = directionVector;

            return output;
        }

        public override SteeringOutput GetSteering(AIAgent agent)
        {
            DrawDebug(agent);

            SetValues(agent);

            var output = base.GetSteering(agent);

            Vector3 fromTargetToCharacter = agent.TargetPosition - agent.transform.position;

            Vector3 movementVec = fromTargetToCharacter.normalized;

            if (fromTargetToCharacter.magnitude > agent.m_slowdownRadius)
            {
                // Getting Seek behavior
                output.linear = Seek.GetStaticSteering(agent).linear;
            }
            else if (fromTargetToCharacter.magnitude > agent.m_satisfactionRadius)
            {
                Vector3 goalVelocityVector = ((fromTargetToCharacter.magnitude / agent.m_slowdownRadius) * agent.maxSpeed) * movementVec;
                //goalVelocityVector = Vector3.ClampMagnitude(goalVelocityVector, agent.maxSpeed);

                Vector3 acceleration = (goalVelocityVector - agent.Velocity) / agent.m_timeToTarget;

                output.linear = acceleration;
            }
            else
            {
                // We need to come to a stop.

                // Applying brakes.
                if(agent.Velocity != Vector3.zero &&
                    agent.Velocity.magnitude > stopSpeed)
                {
                    output.linear = -(movementVec * agent.m_maxAcceleration);
                }
                else
                {
                    agent.Velocity = Vector3.zero;
                    output.linear = Vector3.zero;
                }
            }

            return output;
        }

        private void SetValues(AIAgent pAgent)
        {
            if(!m_overrideAgent)
            {
                slowRadius = pAgent.m_slowdownRadius;
                stopRadius = pAgent.m_satisfactionRadius;
            }
        }
    }
}
