using UnityEngine;

using System.Collections.Generic;

// CODE BY: DANIEL RINALDI
// MODIFIED BY: JOEL LAJOIE-CORRIVEAU

/// <summary>
/// Very quick basic graph implementation that was created to be used only for COMP 476 Lab on pathfinding.
/// It is most likely not suitable for more practical use cases without modification.
/// </summary>
public class GridGraph : MonoBehaviour
{
    [SerializeField, HideInInspector] public List<GridGraphNode> nodes = new List<GridGraphNode>();
    [SerializeField] public GameObject nodePrefab;

    public int Count => nodes.Count;

    public void Clear()
    {
        nodes.Clear();
        gameObject.DestroyChildren();
    }

    public void Remove(GridGraphNode node)
    {
        if (node == null || !nodes.Contains(node)) return;

        foreach (GridGraphNode n in node.adjacencyList)
            n.adjacencyList.Remove(node);

        nodes.Remove(node);
    }

    public void GenerateGrid(bool checkCollisions = true)
    {
        Clear();

        GridGraphNode[,] nodeGrid = new GridGraphNode[generationGridRows, generationGridColumns];

        float width = (generationGridColumns > 0 ? generationGridColumns - 1 : 0) * generationGridCellSize;
        float height = (generationGridRows > 0 ? generationGridRows - 1 : 0) * generationGridCellSize;
        Vector3 genPosition = new Vector3(transform.position.x - (width / 2), transform.position.y - (height / 2), transform.position.z);

        // first pass : generate nodes
        for (int r = 0; r < generationGridRows; ++r)
        {
            float startingX = genPosition.x;
            for (int c = 0; c < generationGridColumns; ++c)
            {
                if (checkCollisions)
                {
                    if (Physics2D.OverlapBox(genPosition, Vector3.one / 2, 0, LayerMask.GetMask("Obstacle")))
                    {
                        genPosition = new Vector3(genPosition.x + generationGridCellSize, genPosition.y, genPosition.z);
                        continue;
                    }
                }

                GameObject obj;
                if (nodePrefab == null)
                    obj = new GameObject("Node", typeof(GridGraphNode));
                else
                    obj = Instantiate(nodePrefab);

                obj.name = $"Node ({nodes.Count})";
                obj.tag = "Node";
                obj.transform.parent = transform;
                obj.transform.position = genPosition;

                GridGraphNode addedNode = obj.GetComponent<GridGraphNode>();                
                nodes.Add(addedNode);
                nodeGrid[r, c] = addedNode;

                genPosition = new Vector3(genPosition.x + generationGridCellSize, genPosition.y, genPosition.z);
            }
            genPosition = new Vector3(startingX, genPosition.y + generationGridCellSize, genPosition.z);
        }

        // second pass : create adjacency lists (edges)
        int[,] operations = new int[,] { { 0, 1 }, { 0, -1 }, { 1, 0 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
        for (int r = 0; r < generationGridRows; ++r)
        {
            for (int c = 0; c < generationGridColumns; ++c)
            {
                if (nodeGrid[r, c] == null) continue;

                for (int i = 0; i < operations.GetLength(0); ++i)
                {
                    int[] neighborId = new int[2] { r + operations[i, 0], c + operations[i, 1] };

                    // check to see if operation brings us out of bounds
                    if (neighborId[0] < 0 || neighborId[0] >= nodeGrid.GetLength(0) || neighborId[1] < 0 || neighborId[1] >= nodeGrid.GetLength(1))
                        continue;

                    GridGraphNode neighbor = nodeGrid[neighborId[0], neighborId[1]];

                    if (neighbor != null)
                    {
                        if (checkCollisions)
                        {
                            // nodeGrid[r, c] is the CURRENT node.
                            // use a box check to see if we can connect to our neighbor, without obstacles.
                            Vector3 direction = neighbor.transform.position - nodeGrid[r, c].transform.position;

                            float colliderCheckHeight = direction.magnitude;

                            Vector2 colliderCheckCenter = new Vector2(nodeGrid[r, c].transform.position.x, nodeGrid[r, c].transform.position.y) + new Vector2(direction.x / 2, direction.y / 2);

                            if (!Physics2D.OverlapBox(colliderCheckCenter,
                                                new Vector2(generationGridAdjacencyCheckWidth, colliderCheckHeight),
                                                360f - Vector3.Angle(new Vector3(0, 1, 0), direction),
                                                LayerMask.GetMask("Obstacle")
                                                ))
                            {
                                nodeGrid[r, c].adjacencyList.Add(neighbor);
                            }


                            if (!Physics2D.OverlapBox(colliderCheckCenter,
                                                new Vector2(generationGridAdjacencyCheckWidthSize2, colliderCheckHeight),
                                                360f - Vector3.Angle(new Vector3(0, 1, 0), direction),
                                                LayerMask.GetMask("Obstacle")
                                                ))
                            {
                                nodeGrid[r, c].m_size2AdjacencyList.Add(neighbor);
                            }

                            if (!Physics2D.OverlapBox(colliderCheckCenter,
                                                new Vector2(generationGridAdjacencyCheckWidthSize3, colliderCheckHeight),
                                                360f - Vector3.Angle(new Vector3(0, 1, 0), direction),
                                                LayerMask.GetMask("Obstacle")
                                                ))
                            {
                                nodeGrid[r, c].m_size3AdjacencyList.Add(neighbor);
                            }
                        }
                        else
                        {
                            nodeGrid[r, c].adjacencyList.Add(neighbor);
                            nodeGrid[r, c].m_size2AdjacencyList.Add(neighbor);
                            nodeGrid[r, c].m_size3AdjacencyList.Add(neighbor);
                        }                  
                    }
                }
            }
        }
    }

    public List<GridGraphNode> GetNeighbors(GridGraphNode node, AI.AIAgent.PathfindingSizeType pPathfindingSizeType = AI.AIAgent.PathfindingSizeType.Normal)
    {
        if (pPathfindingSizeType == AI.AIAgent.PathfindingSizeType.Size2)
            return node.m_size2AdjacencyList;
        else if (pPathfindingSizeType == AI.AIAgent.PathfindingSizeType.Size3)
            return node.m_size3AdjacencyList;
        else
            return node.adjacencyList;
    }

#region grid_generation_properties

    // grid generation properties
    [HideInInspector, Min(0)] public int generationGridColumns = 1;
    [HideInInspector, Min(0)] public int generationGridRows = 1;
    [HideInInspector, Min(0)] public float generationGridCellSize = 1;
    [HideInInspector, Min(0)] public float generationGridAdjacencyCheckWidth = 1f;
    [HideInInspector, Min(0)] public float generationGridAdjacencyCheckWidthSize2 = 2f;
    [HideInInspector, Min(0)] public float generationGridAdjacencyCheckWidthSize3 = 3f;

#if UNITY_EDITOR
    [Header("Gizmos")]
    /// <summary>WARNING: This property is used by Gizmos only and is removed from the build. DO NOT reference it outside of Editor-Only code.</summary>
    public float _nodeGizmoRadius = 0.5f;
    /// <summary>WARNING: This property is used by Gizmos only and is removed from the build. DO NOT reference it outside of Editor-Only code.</summary>
    public Color _edgeGizmoColor = Color.white;
    public Color smoothedPathColor = Color.cyan;

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        
        // nodes
        foreach (GridGraphNode node in nodes)
        {
            if (node == null) continue;

            if(node.isPartOfSmoothedPath)
            {
                Gizmos.color = smoothedPathColor;
            }
            else
            {
                Gizmos.color = node._nodeGizmoColor;
            }
            Gizmos.DrawSphere(node.transform.position, _nodeGizmoRadius);

            Gizmos.color = _edgeGizmoColor;
            List<GridGraphNode> neighbors = GetNeighbors(node);
            foreach (GridGraphNode neighbor in neighbors)
            {
                if(node.isPartOfSolution && neighbor.isPartOfSolution)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(node.transform.position, neighbor.transform.position);
                    Gizmos.color = _edgeGizmoColor;
                }
                else
                {
                    Gizmos.DrawLine(node.transform.position, neighbor.transform.position);
                }
            }
        }
    }
#endif
#endregion
}
