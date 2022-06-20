using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Assets._Project.Scripts;
using System.Linq;

// CODE BY: DANIEL RINALDI
// MODIFIED BY: JOEL LAJOIE-CORRIVEAU

public class Pathfinding : MonoBehaviour
{
    public bool debug;
    [SerializeField] private GridGraph graph;

    public delegate float Heuristic(Transform start, Transform end);

    public delegate void PathComputed(List<GridGraphNode> pPath);
    public event PathComputed NewPathComputed; // Event that is triggered everytime we compute a path.

    public GridGraphNode startNode;
    public GridGraphNode goalNode;
    public GameObject openPointPrefab;
    public GameObject closedPointPrefab;
    public GameObject pathPointPrefab;

    public float m_defaultOrthogonalMovementCost = 1;
    public float m_defaultDiagonalMovementCost = 1;

    public enum HeuristicAlgorithm { None, ManhattanDistance }
    public HeuristicAlgorithm m_heuristicAlgorithm;

    public float m_pathLength = 0;


    private void Update()
    {
        
    }

    public List<GridGraphNode> FindPath(GridGraphNode start, GridGraphNode goal, Heuristic heuristic = null, bool isAdmissible = true, 
        AI.AIAgent.PathfindingSizeType pPathfindingSizeType = AI.AIAgent.PathfindingSizeType.Normal)
    {
        if (graph == null) return new List<GridGraphNode>();

        // if no heuristic is provided then set heuristic = 0
        if (heuristic == null) heuristic = (Transform s, Transform e) => 0;

        List<GridGraphNode> path = null;
        bool solutionFound = false;

        // dictionary to keep track of g(n) values (movement costs)
        Dictionary<GridGraphNode, float> gnDict = new Dictionary<GridGraphNode, float>();
        gnDict.Add(start, default);

        // dictionary to keep track of f(n) values (movement cost + heuristic)
        Dictionary<GridGraphNode, float> fnDict = new Dictionary<GridGraphNode, float>();
        fnDict.Add(start, heuristic(start.transform, goal.transform) + gnDict[start]);

        // dictionary to keep track of our path (came_from)
        Dictionary<GridGraphNode, GridGraphNode> pathDict = new Dictionary<GridGraphNode, GridGraphNode>();
        pathDict.Add(start, null);

        List<GridGraphNode> openList = new List<GridGraphNode>();
        openList.Add(start);

        OrderedDictionary closedODict = new OrderedDictionary();

        while (openList.Count > 0)
        {
            // mimic priority queue and remove from the back of the open list (lowest fn value)
            GridGraphNode current = openList[openList.Count - 1];
            openList.RemoveAt(openList.Count - 1);

            closedODict[current] = true;

            // early exit
            if (current == goal && isAdmissible)
            {
                solutionFound = true;
                break;
            }
            else if (closedODict.Contains(goal))
            {
                // early exit strategy if heuristic is not admissible (try to avoid this if possible)
                float gGoal = gnDict[goal];
                bool pathIsTheShortest = true;

                foreach (GridGraphNode entry in openList)
                {
                    if (gGoal > gnDict[entry])
                    {
                        pathIsTheShortest = false;
                        break;
                    }
                }

                if (pathIsTheShortest) break;
            }

            List<GridGraphNode> neighbors = graph.GetNeighbors(current, pPathfindingSizeType);
            foreach (GridGraphNode n in neighbors)
            {
                // All nodes here are assumed to have a cost
                float movement_cost = m_defaultOrthogonalMovementCost;

                /* 
                 * This section is commented out. It was in the original lab, 
                 * but I believe that A* needs us to reevaluate nodes even if they are in the closed list
                 * because if we get a lower cost, we need to put them back into the open list.
                // if neighbor is in closed list then skip
                if(closedODict.Contains(n) && (bool)closedODict[n])
                {
                    continue;
                }*/

                // find gNeighbor (g_next)
                float g_n = gnDict[current] + movement_cost;

                // if needed: update tables, calculate fn, and update open_list using FakePQListInsert() function
                if(!gnDict.ContainsKey(n) || g_n < gnDict[n])
                {
                    // Updating dictionaries
                    gnDict[n] = g_n;
                    fnDict[n] = g_n + heuristic(n.transform, goal.transform);

                    // Removing the node from the closed list.
                    if(closedODict.Contains(n) && (bool)closedODict[n])
                    {
                        closedODict[n] = false;
                    }

                    // Updating Open List. Note, this does not remove the older entry of the openlist of this node. We might want to remove the older entry.
                    FakePQListInsert(openList, fnDict, n);

                    // Updating path
                    pathDict[n] = current;
                }
            }

            // Adding the current node to the closed list.
            closedODict[current] = true;
        }

        // if the closed list contains the goal node then we have found a solution
        //      This clause is technically useless. But was in the original lab. I think Daniel removed this clause in his lab.
        if (!solutionFound && closedODict.Contains(goal))
            solutionFound = true;

        if (solutionFound)
        {
            // create the path by traversing the previous nodes in the pathDict
            // starting at the goal and finishing at the start
            path = new List<GridGraphNode>();

            GridGraphNode current = goal;
            
            path.Add(goal); // Adding goal node
            while(pathDict[current] != null)
            {
                path.Add(pathDict[current]);
                current = pathDict[current];
            }
            //path.Add(current); // Adding start node


            // reverse the path since we started adding nodes from the goal 
            path.Reverse();
        }

        if (debug)
        {
            ClearPoints();

            List<Transform> openListPoints = new List<Transform>();
            foreach (GridGraphNode node in openList)
            {
                openListPoints.Add(node.transform);
            }
            SpawnPoints(openListPoints, openPointPrefab, Color.magenta);

            List<Transform> closedListPoints = new List<Transform>();
            foreach (DictionaryEntry entry in closedODict)
            {
                GridGraphNode node = (GridGraphNode) entry.Key;
                if (solutionFound && !path.Contains(node))
                    closedListPoints.Add(node.transform);
            }
            SpawnPoints(closedListPoints, closedPointPrefab, Color.red);

            if (solutionFound)
            {
                List<Transform> pathPoints = new List<Transform>();

                foreach (GridGraphNode node in path)
                {
                    pathPoints.Add(node.transform);
                }

                // Draw lines between the nodes
                for (int i = 0; i < path.Count; i++)
                {
                    if(i != path.Count - 1)
                    {
                        Vector3 offset = new Vector3(0, 0.25f, 0);
                        Debug.DrawRay(path[i].transform.position + offset, (path[i+1].transform.position + offset) - (path[i].transform.position + offset), Color.green, 20);
                    }

                    path[i].isPartOfSolution = true;
                }

                SpawnPoints(pathPoints, pathPointPrefab, Color.green);
            }
        }

        if (path != null)
            m_pathLength = path.Count - 1;
        else
            m_pathLength = 0;

        if(path != null && path.Count > 0)
        {
            NewPathComputed?.Invoke(path);
        }

        return path;
    }

    private void SpawnPoints(List<Transform> points, GameObject prefab, Color color)
    {
        for (int i = 0; i < points.Count; ++i)
        {
#if UNITY_EDITOR
            // Scene view visuals
            //points[i].GetComponent<GridGraphNode>()._nodeGizmoColor = color;
#endif

            // Game view visuals
            /*GameObject obj = Instantiate(prefab, points[i].position, Quaternion.identity, points[i]);
            obj.name = "DEBUG_POINT";
            obj.transform.localPosition += Vector3.up * 0.5f;*/
        }
    }

    private void ClearPoints()
    {
        foreach (GridGraphNode node in graph.nodes)
        {
            for (int c = 0; c < node.transform.childCount; ++c)
            {
                // Reset gizmo color for points that aren't searched over.
                //node._nodeGizmoColor = Color.black;

                node.isPartOfSolution = false;
                node.isPartOfSmoothedPath = false;

                if (node.transform.GetChild(c).name == "DEBUG_POINT")
                {
                    Destroy(node.transform.GetChild(c).gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Resets the values linked to this class.
    /// </summary>
    public void ResetPathfinding()
    {
        startNode = null;
        goalNode = null;
        ClearPoints();
    }

    /// <summary>
    /// mimics a priority queue here by inserting at the right position using a loop
    /// not a very good solution but ok for this lab example
    /// </summary>
    /// <param name="pqList"></param>
    /// <param name="fnDict"></param>
    /// <param name="node"></param>
    private void FakePQListInsert(List<GridGraphNode> pqList, Dictionary<GridGraphNode, float> fnDict, GridGraphNode node)
    {
        if (pqList.Count == 0)
            pqList.Add(node);
        else
        {
            for (int i = pqList.Count - 1; i >= 0; --i)
            {
                if (fnDict[pqList[i]] > fnDict[node])
                {
                    pqList.Insert(i + 1, node);
                    break;
                }
                else if (i == 0)
                    pqList.Insert(0, node);
            }
        }
    }

    /// <summary>
    /// mimics a priority queue here by inserting at the right position using a loop
    /// not a very good solution but ok for this lab example
    /// </summary>
    /// <param name="pqList"></param>
    /// <param name="fnDict"></param>
    /// <param name="node"></param>
    private void FakePQListInsert(List<ClusterNode> pqList, Dictionary<ClusterNode, float> fnDict, ClusterNode node)
    {
        if (pqList.Count == 0)
            pqList.Add(node);
        else
        {
            for (int i = pqList.Count - 1; i >= 0; --i)
            {
                if (fnDict[pqList[i]] > fnDict[node])
                {
                    pqList.Insert(i + 1, node);
                    break;
                }
                else if (i == 0)
                    pqList.Insert(0, node);
            }
        }
    }

    public float Heur_ManhattanDistance(Transform pStart, Transform pGoal)
    {
        // Mathematical formula:
        // h(n) = D * (|current.x - goal.x| + |current.y - goal.y|) 
        // Where D is the cost of an orthogonal movement.

        // In our case, we use the default move cost for D.

        // Calculating absolute values of differences
        float absoluteXDiff = Mathf.Abs(pStart.position.x - pGoal.position.x);
        float absoluteYDiff = Mathf.Abs(pStart.position.z - pGoal.position.z); // Y in our case is the z axis.

        // Calculating and returning the distance
        return m_defaultOrthogonalMovementCost * (absoluteXDiff + absoluteYDiff);
    }

    public List<GridGraphNode> SmoothPath(List<GridGraphNode> pPath, float pWidth = 1f)
    {
        if(pPath == null)
        {
            return null;
        }

        if (pPath.Count <= 2)
            return pPath;

        List<GridGraphNode> newPath = new List<GridGraphNode>();

        GridGraphNode current = pPath[0];
        GridGraphNode next;
        int currentIndex = pPath.Count - 1;

        newPath.Add(current);

        Vector3 offset = new Vector3(0, 0, 0);

        // Perform path smoothing as outlined on slides 67 and 68 of Pathfinding_AI_02

        // Keep looping while we haven't reached the goal
        while(newPath[newPath.Count - 1] != pPath[pPath.Count - 1])
        {
            bool obstacleInTheWay = false;
            do
            {
                next = pPath[currentIndex];

                Vector3 direction = (next.transform.position + offset) - (current.transform.position + offset);

                // Checks if the path to the next node is clear of obstacles
                float colliderCheckHeight = direction.magnitude;

                Vector2 colliderCheckCenter = new Vector2(current.transform.position.x + offset.x, current.transform.position.y + offset.y) + new Vector2(direction.x / 2, direction.y / 2);

                //Collider2D[] bobo = Physics2D.OverlapCapsuleAll(capsuleCenter,
                //    new Vector2(pWidth, capsuleHeight),
                //    CapsuleDirection2D.Vertical,
                //    //Quaternion.LookRotation(direction.normalized, -Vector3.forward).eulerAngles.z,
                //    360f - Vector3.Angle(new Vector3(0, 1, 0), direction),
                //    LayerMask.GetMask("Obstacle")
                //    );

                //obstacleInTheWay = Physics2D.OverlapCapsule(capsuleCenter,
                //    new Vector2(pWidth, capsuleHeight),
                //    CapsuleDirection2D.Vertical,
                //    //Quaternion.LookRotation(direction.normalized, -Vector3.forward).eulerAngles.z,
                //    360f - Vector3.Angle(new Vector3(0, 1, 0), direction),
                //    LayerMask.GetMask("Obstacle")
                //    );

                obstacleInTheWay = Physics2D.OverlapBox(colliderCheckCenter,
                                                new Vector2(pWidth, colliderCheckHeight),
                                                360f - Vector3.Angle(new Vector3(0, 1, 0), direction),
                                                LayerMask.GetMask("Obstacle")
                                                );

                currentIndex--;

                // The path crashed. We cannot smooth it.
                if (currentIndex < 0)
                    return null;
            }
            while (obstacleInTheWay);

            // No obstacle from current to next node.

            // Adding the next node in the path
            newPath.Add(next);
            next.isPartOfSmoothedPath = true;

            // Resetting variables
            current = next;
            currentIndex = pPath.Count - 1;

            if(newPath.Count > pPath.Count)
            {
                // We are now making a longer path. This is not normal. We crashed.
                return null;
            }
        }

        // Visually showing the smoothed path
        /*
        for (int i = 0; i < newPath.Count; i++)
        {
            if(i + 1 < newPath.Count - 1)
            {
                GridGraphNode firstNode = newPath[i];
                GridGraphNode secondNode = newPath[i + 1];
                Debug.DrawRay(firstNode.transform.position + offset, secondNode.transform.position - firstNode.transform.position + offset, Color.cyan, 20);
            }
        } */
            
        return newPath;
    }
}
