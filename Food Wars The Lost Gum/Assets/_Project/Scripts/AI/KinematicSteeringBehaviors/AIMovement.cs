using System;
using UnityEngine;
// CODE BY DANIEL RINALDI
// MODIFIED BY: Joel Lajoie-Corriveau

namespace AI
{
    public abstract class AIMovement : MonoBehaviour
    {
        public bool debug;
        public enum AIMovementType { Linear, Angular }
        public AIMovementType m_movementType;

        // Made to be overidden by the implementation.
        public virtual SteeringOutput GetKinematic(AIAgent agent)
        {
            return new SteeringOutput { angular = Quaternion.identity };
            //return new SteeringOutput { angular = agent.transform.rotation };
        }

        public virtual SteeringOutput GetSteering(AIAgent agent)
        {
            return new SteeringOutput { angular = Quaternion.identity };
        }
    }
}
