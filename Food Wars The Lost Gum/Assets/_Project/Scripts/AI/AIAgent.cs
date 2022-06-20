using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// CODE BY DANIEL RINALDI
// MODIFIED BY: Joel Lajoie-Corriveau


namespace AI
{
    public class AIAgent : MonoBehaviour
    {
        /// <summary>
        /// Maximum speed of this agent
        /// </summary>
        public float maxSpeed = 5f;
        /// <summary>
        /// Agent's max rotation speed
        /// </summary>
        public float maxDegreesDelta = 720;
        public bool lockZ = true;
        /// <summary>
        /// Whether or not to show debug information about this agent.
        /// </summary>
        public bool debug;
        // This is also called the "arrival  Radius"
        public float m_satisfactionRadius = 0.3f;
        /// <summary>
        /// Determines the distance, between the AI and its target, at which the AI should stop moving if it's running from a target.
        /// </summary>
        public float m_runAwaySatisfactionRadius = 15f;
        /// <summary>
        /// Range at which the agent slows down to try to reach its target, when using Steering behaviors.
        /// </summary>
        public float m_slowdownRadius = 10f;
        public float m_timeToTarget = 2f;
        public float m_maxAcceleration = 14f;

        /// <summary>
        /// Represents the distance at which the agent will switch over to the next node.
        /// </summary>
        public float m_nextNodeRadius = 0.2f;

        public enum EBehaviorType { Kinematic, Steering }
        public EBehaviorType behaviorType;

        public enum MovementType { ReachTarget, AvoidTarget, None }
        // Determines whether we are trying to go towards or target or away from it.
        public MovementType m_movementType = MovementType.None;
        [HideInInspector]
        public MovementType m_lastMovementType = MovementType.None;

        // Determines whether the character has the right to rotate at all, despite its behaviors.
        // If set to true, any restriction on movement based on rotation will be deactivated.
        public bool m_rotationAllowed = true;

        // Determines whether the character has the right to move at all, despite its behaviors.
        public bool m_movementAllowed = true;

        /// <summary>
        /// Speed at which the agent is considered to be very slow.
        /// </summary>
        public float m_verySlowSpeed = 0.3f;
        /// <summary>
        /// Speed at which the agent is considered to be running.
        /// </summary>
        public float m_runSpeed = 2f;

        /// <summary>
        /// Whether or not this agent is agressive towards the player at the moment.
        /// </summary>
        public bool m_isAgressive = true;

        /// <summary>
        /// Width of the agent when considering paths to the player or other targets.
        /// </summary>
        public float m_pathfindingWidth = 1f;

        public enum PathfindingSizeType { Normal, Size2, Size3 }
        public PathfindingSizeType m_pathfindingSizeType = PathfindingSizeType.Normal;


        /// <summary>
        /// Delay until the agent recalculates its path.
        /// </summary>
        public float m_recomputeDelay = 1f;
        private float m_lastRecomputeTime;

        /// <summary>
        /// Maximum turning angle an AI is allowed to do when following the first 2 nodes of a path.
        /// </summary>
        public float m_maxTurnAngleOnPath = 175;

        /// <summary>
        /// Maximum range from agent to target at which the agent will try to compute a path to the target.
        /// </summary>
        public float m_maxComputePathRange = 15f;

        /// <summary>
        /// If true, the agent will stop moving when at a distance from its target equal to its satisfaction radius.
        /// </summary>
        public bool m_stopAnyMovementAtSatisfactionRadius = true;

        /// <summary>
        /// Strength of the impulse applied to the agent to try to unstuck it.
        /// </summary>
        public float m_bumpAwayForceStrength = 5f;

        private GameManager m_gameManager;

        /// <summary>
        /// The path this AI is currently following.
        /// </summary>
        private List<GridGraphNode> m_currentPath = null;

        /// <summary>
        /// Represents the node the agent is currently going for
        /// </summary>
        private GridGraphNode m_currentNode;

        /// <summary>
        /// Index of the current node object, in the current path.
        /// </summary>
        private int m_currentNodeIndex;

        /// <summary>
        /// The last node this AI was trying to reach.
        /// </summary>
        private GridGraphNode m_lastNode = null;

        private Animator animator;

        private Rigidbody2D m_rigidBody;

        [SerializeField] public GameObject trackedEnemy;
        public bool applyingForce;

        public Vector3 EnemyPosition
        {
            get
            {
                if(trackedEnemy == null)
                {
                    throw new System.Exception("No tracked enemy!");
                }
                else
                {
                    return trackedEnemy.transform.position;
                }
            }
        }

        [SerializeField] public Transform trackedTarget;
        [SerializeField] private Vector3 targetPosition;

        public Vector3 TargetPosition
        {
            get => trackedTarget != null ? trackedTarget.position : targetPosition;
        }
        public Vector3 TargetRight
        {
            get => trackedTarget != null ? trackedTarget.right : Vector3.right;
        }
        public Vector3 TargetVelocity
        {
            get
            {
                Vector3 v = Vector3.zero;
                if (trackedTarget != null)
                {
                    AIAgent targetAgent = trackedTarget.GetComponent<AIAgent>();
                    if (targetAgent != null)
                        v = targetAgent.Velocity;
                }

                return v;
            }
        }

        public Vector3 Velocity { get; set; }

        public List<GridGraphNode> Path
        {
            get { return m_currentPath; }
            set
            {
                m_currentPath = value;

                m_currentNode = null;
                m_currentNodeIndex = -1;

                // Moving agent to start point.
                if (m_currentPath != null && m_currentPath.Count > 0)
                {
                    // Resetting velocity
                    Velocity = Vector3.zero;

                    // Clearing target
                    UnTrackTarget();
                }
                else
                {
                    // Our path is now null. Check if we are tracking an enemy.
                    if(trackedEnemy != null)
                    {
                        // We are tracking an enemy. Just go directly to it.
                        TrackTarget(trackedEnemy.transform);
                    }
                }
            }
        }

        public void TrackTarget(Transform targetTransform)
        {
            trackedTarget = targetTransform;
        }

        public void UnTrackTarget()
        {
            trackedTarget = null;
        }

        public void ResetTargetPosition()
        {
            targetPosition = Vector3.zero;
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            m_gameManager = GameObject.FindObjectOfType<GameManager>();
            m_rigidBody = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Ensure proper enable/disable of behaviors on start.
            SwitchMovementType(m_movementType, true);
            // initial offset time to distribute the recompute path time over different frames.
            m_lastRecomputeTime = Random.Range(0f, 0.5f);
            //m_lastRecomputeTime = 0;
        }

        private void Update()
        {
            if (debug)
                Debug.DrawRay(transform.position, Velocity, Color.red);

            if (applyingForce)
                return;

            // Defining if we need to apply Move or Rotation
            bool applyRotation = true;
            bool applyMove = true;
            bool stopMoving = false;

            // Calculating vector from agent to the old target
            Vector3 fromAgentToTarget = TargetPosition - transform.position;

            if (stopMoving)
            {
                Velocity = Vector3.zero;
            }

            if(!m_rotationAllowed)
            {
                // Rotation is not allowed. But movement must ALWAYS be.
                applyRotation = false;
                applyMove = true;
            }
            if(!m_movementAllowed || stopMoving)
            {
                applyMove = false;
            }

            if (behaviorType == EBehaviorType.Kinematic)
            {
                // Declaring the variable as we pass it to the function. The Out keyword specifies a pass by reference, and then we have this variable to play with in this scope.
                GetKinematicAvg(out Vector3 kinematicAvg, out Quaternion rotation);               
                
                if(applyMove)
                    ApplyKinematicMove(kinematicAvg);
                if (applyRotation)
                    ApplyKinematicRotation(rotation);
            }
            else
            {
                GetSteeringSum(out Vector3 steeringSum, out Quaternion rotation);

                if (applyMove)
                    ApplySteeringMove(steeringSum);
                if (applyRotation)
                    ApplySteeringRotation(rotation);
            }

            // Following path
            if (m_currentPath != null)
            {
                if (fromAgentToTarget.magnitude <= m_nextNodeRadius ||
                    trackedTarget == null)
                {
                    GridGraphNode nextNode = GetNextNodeOnPath();

                    if (nextNode != null)
                    {
                        TrackTarget(nextNode.transform);
                    }
                    else
                    {
                        // We are at the end of the path. Go directly to target.

                        // Remove current path.
                        this.Path = null;

                        // Go to target
                        if(trackedEnemy != null)
                        {
                            // We are tracking an enemy.
                            TrackTarget(trackedEnemy.transform);
                        }
                        else
                        {
                            // We are tracking a position
                            UnTrackTarget();
                        }
                    }
                }
            }

            // Recompute path regularly
            if (Time.time - m_lastRecomputeTime >= m_recomputeDelay)
            {
                RecomputePath();
            }

            // Now we have a new target. If we reached this new target, we should stop moving if we are using satisfaction radius.
            if(m_stopAnyMovementAtSatisfactionRadius)
            {
                // Only do this behavior if we have no next node on a path, or are not on a path.
                if(Path == null)
                {
                    if (m_movementType == MovementType.ReachTarget)
                    {
                        if (trackedEnemy != null)
                        {
                            if ((trackedEnemy.transform.position - transform.position).magnitude < m_satisfactionRadius)
                            {
                                m_movementAllowed = false;
                            }
                            else
                            {
                                m_movementAllowed = true;
                            }
                        }
                        else
                        {
                            if ((TargetPosition - transform.position).magnitude < m_satisfactionRadius)
                            {
                                m_movementAllowed = false;
                            }
                            else
                            {
                                m_movementAllowed = true;
                            }
                        }
                    }


                    if (m_movementType == MovementType.AvoidTarget)
                    {
                        if (trackedEnemy != null)
                        {
                            if ((trackedEnemy.transform.position - transform.position).magnitude > m_runAwaySatisfactionRadius)
                            {
                                m_movementAllowed = false;
                            }
                            else
                            {
                                m_movementAllowed = true;
                            }
                        }
                        else
                        {
                            if ((TargetPosition - transform.position).magnitude > m_runAwaySatisfactionRadius)
                            {
                                m_movementAllowed = false;
                            }
                            else
                            {
                                m_movementAllowed = true;
                            }
                        }
                    }
                }
            }


            //animator.SetBool("walking", Velocity.magnitude > 0);
            //animator.SetBool("running", Velocity.magnitude > m_runSpeed);
        }

        private void GetKinematicAvg(out Vector3 kinematicAvg, out Quaternion rotation)
        {
            kinematicAvg = Vector3.zero;
            Vector3 eulerAvg = Vector3.zero;
            AIMovement[] movements = GetComponents<AIMovement>();
            int linearCount = 0;
            int angularCount = 0;
            foreach (AIMovement movement in movements)
            {
                // Skip over disabled movements
                if (!movement.enabled)
                    continue;

                Vector3 linearComponent = movement.GetKinematic(this).linear;
                Vector3 angularComponent = movement.GetKinematic(this).angular.eulerAngles;

                kinematicAvg += linearComponent;
                eulerAvg += angularComponent;

                if(movement.m_movementType == AIMovement.AIMovementType.Linear &&
                    linearComponent != Vector3.zero)
                {
                    linearCount++;
                }
                else if (movement.m_movementType == AIMovement.AIMovementType.Angular &&
                    angularComponent != Vector3.zero)
                {
                    angularCount++;
                }
            }

            if ((linearCount + angularCount) > 0)
            {
                if(linearCount > 0)
                    kinematicAvg /= linearCount;
                if(angularCount > 0)
                    eulerAvg /= angularCount;

                rotation = Quaternion.Euler(eulerAvg);
            }
            else
            {
                kinematicAvg = Velocity;
                rotation = transform.rotation;
            }
        }

        private void GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation)
        {
            steeringForceSum = Vector3.zero;
            rotation = Quaternion.identity;
            AIMovement[] movements = GetComponents<AIMovement>();
            foreach (AIMovement movement in movements)
            {
                // Skip over disabled movements
                if (!movement.enabled)
                    continue;

                steeringForceSum += movement.GetSteering(this).linear;
                rotation *= movement.GetSteering(this).angular;
            }
        }

        private void ApplyKinematicMove(Vector3 pKinematicAvg)
        {
            Velocity = pKinematicAvg.normalized * maxSpeed;
            //Velocity = Vector3.ClampMagnitude(pKinematicAvg, maxSpeed);

            if(m_rigidBody != null)
            {
                m_rigidBody.MovePosition(transform.position + Velocity * Time.fixedDeltaTime);
            }
            else
            {
                transform.position += Velocity * Time.deltaTime;
            }
        }

        private void ApplyKinematicRotation(Quaternion pRotation)
        {
            pRotation = Quaternion.Euler(0, 0, pRotation.eulerAngles.z); // Doubling down to make sure our rotation is ONLY around the z axis.

            if (m_rigidBody != null)
            {
                m_rigidBody.MoveRotation(pRotation);
            }
            else
            {
                transform.rotation = pRotation;
            }
        }

        private void ApplySteeringMove(Vector3 pSteeringSum)
        {
            Vector3 acceleration = pSteeringSum / 1; // 1 here is the mass of our agent. Here, we assume it is 1. But we could change it to be the real mass of our agent.
                                                    // Checking that accel doesn't exceed the maximum
            acceleration = Vector3.ClampMagnitude(acceleration, m_maxAcceleration);
            Velocity += acceleration * Time.deltaTime;
            Velocity = Vector3.ClampMagnitude(Velocity, maxSpeed); // Ensure we don't exceed our max speed.

            if (m_rigidBody != null)
            {
                m_rigidBody.MovePosition(transform.position + Velocity * Time.fixedDeltaTime);
            }
            else
            {
                transform.position += Velocity * Time.deltaTime;
            }
        }

        private void ApplySteeringRotation(Quaternion pRotation)
        {
            pRotation = Quaternion.Euler(0, 0, pRotation.eulerAngles.z); // Doubling down to make sure our rotation is ONLY around the z axis.

            if (m_rigidBody != null)
            {
                m_rigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, // RotateTowards Helps to gradually rotate from one position to the other.
                                                            transform.rotation * pRotation, // To add quaternions, you multiply them together.
                                                            maxDegreesDelta * Time.fixedDeltaTime)); // MaxDegreesDelta is actually our agent's max rotation speed, kinda.
                                                                                               // Time Delta time allows the transition to be frame independent, since this is now a smooth transition.);
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, // RotateTowards Helps to gradually rotate from one position to the other.
                                                            transform.rotation * pRotation, // To add quaternions, you multiply them together.
                                                            maxDegreesDelta * Time.deltaTime); // MaxDegreesDelta is actually our agent's max rotation speed, kinda.
                                                                                               // Time Delta time allows the transition to be frame independent, since this is now a smooth transition. 
            }
        }

        public void SwitchMovementType(MovementType pMovementType, bool pOverrideStatusQuo = false)
        {
            m_lastMovementType = m_movementType;

            // If we are already in the movement type, we don't change anything, EXCEPT if we are overriding status quo.
            if (m_movementType == pMovementType && !pOverrideStatusQuo)
                return;

            if(pMovementType == MovementType.ReachTarget)
            {
                foreach (AIMovement movement in GetComponents<AIMovement>())
                {
                    // Disabling avoiding type movements
                    if(movement is Flee)
                    {
                        movement.enabled = false;
                    }
                    else if(movement is LookAwayFromTarget)
                    {
                        movement.enabled = false;
                    }
                    // Enabling Reaching type movements
                    else if(movement is Arrive)
                    {
                        movement.enabled = true;
                    }
                    else if(movement is LookAtTarget)
                    {
                        movement.enabled = true;
                    }
                    else if(movement is Seek)
                    {
                        movement.enabled = true;
                    }
                }
            }
            else if(pMovementType == MovementType.AvoidTarget)
            {
                foreach (AIMovement movement in GetComponents<AIMovement>())
                {
                    // Enabling avoiding type movements
                    if (movement is Flee)
                    {
                        movement.enabled = true;
                    }
                    else if (movement is LookAwayFromTarget)
                    {
                        movement.enabled = true;
                    }
                    // Disabling Reaching type movements
                    else if (movement is Arrive)
                    {
                        movement.enabled = false;
                    }
                    else if (movement is LookAtTarget)
                    {
                        movement.enabled = false;
                    }
                    else if (movement is Seek)
                    {
                        movement.enabled = false;
                    }
                }
            }
            else if (pMovementType == MovementType.None)
            {
                foreach (AIMovement movement in GetComponents<AIMovement>())
                {
                    // Disabling all movements
                    if (movement is AIMovement)
                    {
                        movement.enabled = false;
                    }
                }
            }

            m_movementType = pMovementType;
        }

        public GridGraphNode GetNextNodeOnPath()
        {
            if (m_currentNodeIndex >= m_currentPath.Count)
                return null;

            m_currentNodeIndex++;

            if (m_currentNodeIndex <= m_currentPath.Count - 1)
                m_currentNode = m_currentPath[m_currentNodeIndex];
            else
                m_currentNode = null;

            m_lastNode = m_currentNode;

            return m_currentNode;
        }

        public void ComputePathToTarget()
        {
            if (m_gameManager == null)
                return;

            if (CheckIfPathIsClear(TargetPosition))
            {
                // We can reach the target directly. No path needed.
                this.Path = null;
                return;
            }

            GridGraphNode closestToTarget = m_gameManager.GetClosestNode(TargetPosition);
            GridGraphNode closestToMe = m_gameManager.GetClosestNode(transform.position);

            if (closestToTarget != null)
            {
                List<GridGraphNode> path = m_gameManager.ComputePath(closestToMe, closestToTarget, m_pathfindingWidth, m_pathfindingSizeType);

                if(path != null && path.Count != 0)
                {
                    CheckPathTurnAround(path);
                    this.Path = path;
                }
            }
        }

        public void ComputePathToEnemy()
        {
            if (m_gameManager == null)
                return;

            if (trackedEnemy == null)
            {
                throw new System.Exception("ComputePathToEnemy: No enemy tracked!");
            }

            if (CheckIfPathIsClear(EnemyPosition))
            {
                // We can reach the target directly. No path needed.
                this.Path = null;
                return;
            }

            GridGraphNode closestToTarget = m_gameManager.GetClosestNode(EnemyPosition);
            GridGraphNode closestToMe = m_gameManager.GetClosestNode(transform.position);

            if (closestToTarget != null)
            {
                List<GridGraphNode> path = m_gameManager.ComputePath(closestToMe, closestToTarget, m_pathfindingWidth, m_pathfindingSizeType);

                if (path != null && path.Count != 0)
                {
                    CheckPathTurnAround(path);
                    this.Path = path;
                }
            }
        }

        public void ComputePathAwayFromTarget()
        {
            if (m_gameManager == null)
                return;

            Vector3 fromAgentToTarget = TargetPosition - gameObject.transform.position;
            Vector3 awayFromTarget = -fromAgentToTarget;

            Vector3 positionAwayFromTarget = gameObject.transform.position + awayFromTarget;

            if (CheckIfPathIsClear(positionAwayFromTarget))
            {
                // We can reach the target directly. No path needed.
                this.Path = null;
                return;
            }

            GridGraphNode closestToMe = m_gameManager.GetClosestNode(gameObject.transform.position);

            GridGraphNode closestToTarget = m_gameManager.GetClosestNode(positionAwayFromTarget);

            if (closestToTarget != null)
            {
                List<GridGraphNode> path = m_gameManager.ComputePath(closestToMe, closestToTarget, m_pathfindingWidth, m_pathfindingSizeType);

                if (path != null && path.Count != 0)
                {
                    CheckPathTurnAround(path);
                    this.Path = path;
                }
            }
        }

        public void ComputePathAwayFromEnemy()
        {
            if (m_gameManager == null)
                return;

            if (trackedEnemy == null)
            {
                throw new System.Exception("ComputePathAwayFromEnemy: No enemy tracked!");
            }

            Vector3 fromAgentToTarget = EnemyPosition - gameObject.transform.position;
            Vector3 awayFromTarget = -fromAgentToTarget;

            Vector3 positionAwayFromTarget = gameObject.transform.position + awayFromTarget;

            if (CheckIfPathIsClear(positionAwayFromTarget))
            {
                // We can reach the target directly. No path needed.
                this.Path = null;
                return;
            }

            GridGraphNode closestToMe = m_gameManager.GetClosestNode(gameObject.transform.position);

            GridGraphNode closestToTarget = m_gameManager.GetClosestNode(positionAwayFromTarget);

            if (closestToTarget != null)
            {
                List<GridGraphNode> path = m_gameManager.ComputePath(closestToMe, closestToTarget, m_pathfindingWidth, m_pathfindingSizeType);

                if (path != null && path.Count != 0)
                {
                    CheckPathTurnAround(path);
                    this.Path = path;
                }
            }
        }

        public void RecomputePath()
        {
            Vector3 distanceToTarget;

            if(trackedEnemy != null)
            {
                distanceToTarget = EnemyPosition - transform.position;
            }
            else
            {
                distanceToTarget = TargetPosition - transform.position;
            }

            // If our target is too far, dont compute a path to it.
            if(distanceToTarget.magnitude > m_maxComputePathRange)
            {
                Path = null;
                return;
            }

            switch (m_movementType)
            {
                case MovementType.ReachTarget:
                    if(trackedEnemy != null)
                    {
                        ComputePathToEnemy();
                    }
                    else
                    {
                        ComputePathToTarget();
                    }
                    break;
                case MovementType.AvoidTarget:
                    if (trackedEnemy != null)
                    {
                        ComputePathAwayFromEnemy();
                    }
                    else
                    {
                        ComputePathAwayFromTarget();
                    }
                    break;
                default:
                    // Do nothing.
                    Path = null;
                    break;
            }

            if(Path != null)
            {
                if(m_lastNode != null)
                {
                    // If we have direct access to our last node...

                    if(CheckIfPathIsClear(m_lastNode.transform.position))
                    {
                        // Adding our last node we were trying to reach as the first node of the path.
                        Path.Insert(0, m_lastNode);
                    }
                }
            }

            m_lastRecomputeTime = Time.time;
        }

        public GameObject AcquireTarget(string pTag, float pRadius)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, pRadius);

            // Find closest target.
            GameObject target = null;
            float minDistance = float.MaxValue;
            foreach (var collider in colliders)
            {
                if(collider.gameObject.CompareTag(pTag))
                {
                    // We found a target.

                    float distanceToTarget = (collider.gameObject.transform.position - transform.position).magnitude;

                    if (distanceToTarget < minDistance)
                    {
                        target = collider.gameObject;
                        minDistance = distanceToTarget;
                    }    
                }
            }

            return target;
        }

        /// <summary>
        /// Verifies if a path requires the agent to go backwards, then forwards again at almost 180, at the start of it.
        /// If it does, it removes the first node of that path.
        /// </summary>
        private void CheckPathTurnAround(List<GridGraphNode> pPath)
        {
            // Can't happen with less than 2 nodes.
            if(pPath.Count < 2)
            {
                return;
            }

            Vector3 fromAgentToFirstNode = pPath[0].transform.position - transform.position;
            Vector3 fromFirstNodeToSecondNode = pPath[1].transform.position - pPath[0].transform.position;

            float angleDifference = Vector3.Angle(fromAgentToFirstNode, fromFirstNodeToSecondNode);

            // If we are turning more than a certain angle in our 2 first nodes...
            if(angleDifference > m_maxTurnAngleOnPath)
            {
                // Removing the first node so we dont get a turnaround
                pPath.RemoveAt(0);
            }
        }

        /// <summary>
        /// Checks if the AI can reach the point specified without bumping into an obstacle when going in a straight line.
        /// </summary>
        /// <returns>True if the AI can reach the target in a straight line, false otherwise.</returns>
        public bool CheckIfPathIsClear(Vector3 pPosition)
        {
            Vector3 direction = pPosition - transform.position;

            float colliderCheckHeight = direction.magnitude;

            Vector2 colliderCheckCenter = new Vector2(transform.position.x, transform.position.y) + new Vector2(direction.x / 2, direction.y / 2);

            if (Physics2D.OverlapBox(colliderCheckCenter,
                                                new Vector2(m_pathfindingWidth, colliderCheckHeight),
                                                360f - Vector3.Angle(new Vector3(0, 1, 0), direction),
                                                LayerMask.GetMask("Obstacle")
                                                ))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Prevent two enemies from getting stuck together.
            AIAgent otherAgent = collision.gameObject.GetComponent<AIAgent>();
            if(otherAgent != null)
            {
                // We collided with an agent.
                Vector3 otherAgentDirection = otherAgent.TargetPosition - otherAgent.gameObject.transform.position;
                Vector3 myDirection = TargetPosition - transform.position;
                
                // We use the max turn angle on path to determine if they are close to opposite directions of movement.
                if (Vector3.Angle(otherAgentDirection, myDirection) > m_maxTurnAngleOnPath)
                {
                    // Our two objects collided, and are going in opposite directions. They are stuck.
                    if(m_rigidBody != null)
                    {
                        // Apply some impulse to unstuck.
                        // We get some impulse to our "right"
                        Vector3 directionOfForce = Quaternion.Euler(0, 0, 90) * myDirection.normalized;

                        m_rigidBody.AddForce(directionOfForce * m_bumpAwayForceStrength, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }
}