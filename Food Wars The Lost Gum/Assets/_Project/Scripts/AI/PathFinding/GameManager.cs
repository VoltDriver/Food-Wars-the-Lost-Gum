using AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool m_enablePathSmoothing = false;

    public float m_closestNodeFindingDistance = 3f;
    public bool m_enableIncrementalNodeFind = true;
    public float m_nodeFindingDistanceIncrements = 3f;
    public int m_maxNodeFindingIncrements = 5;

    public List<AIAgent> m_agents;
    public Transform InitialAgentsParent;

    private Pathfinding m_pathfinding;

    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform t in InitialAgentsParent)
        {
            m_agents.Add(t.gameObject.GetComponent<AIAgent>());
        }

        m_pathfinding = FindObjectOfType<Pathfinding>();

        foreach (var agent in m_agents)
        {
            if (agent != null)
                agent.RecomputePath();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // should actively recompute the agents in scene
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //m_agents.Clear();
        //foreach(GameObject g in enemies)
        //{
        //    m_agents.Add(g.GetComponent<AIAgent>());
        //}
    }

    public List<GridGraphNode> ComputePath(GridGraphNode pStart, GridGraphNode pEnd, float pWidth = 1f, AI.AIAgent.PathfindingSizeType pPathfindingSizeType = AIAgent.PathfindingSizeType.Normal)
    {
        m_pathfinding.ResetPathfinding();
        List<GridGraphNode> path = m_pathfinding.FindPath(pStart, pEnd, m_pathfinding.Heur_ManhattanDistance, true, pPathfindingSizeType);

        List<GridGraphNode> newPath = new List<GridGraphNode>();

        if (m_enablePathSmoothing)
        {
            newPath = m_pathfinding.SmoothPath(path, pWidth);
            return newPath;
        }

        return path;
    }

    public GridGraphNode GetClosestNode(Vector3 pPosition)
    {
        // Check with physics for the closest node.
        Collider2D[] nodeColliders = Physics2D.OverlapCircleAll(pPosition, m_closestNodeFindingDistance, LayerMask.GetMask("Node"));

        if(nodeColliders.Length == 0)
        {
            if(m_enableIncrementalNodeFind)
            {
                int count = 0;
                while(nodeColliders.Length == 0 && count < m_maxNodeFindingIncrements)
                {
                    // Check with physics for the closest node.
                    nodeColliders = Physics2D.OverlapCircleAll(pPosition, m_closestNodeFindingDistance + count * m_nodeFindingDistanceIncrements, LayerMask.GetMask("Node"));
                    count++;
                }
            }

            return null;
        }
        else
        {
            float minDist = float.MaxValue;
            GridGraphNode minNode = null;
            foreach (var node in nodeColliders)
            {
                float distance = (node.gameObject.transform.position - pPosition).magnitude;

                if(distance < minDist)
                {
                    minDist = distance;
                    minNode = node.gameObject.GetComponent<GridGraphNode>();
                }
            }

            return minNode;
        }
    }
}
