using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of node that serves for control flow (for loops, switches...). Has multiple children.
/// </summary>
public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> m_children = new List<Node>();

    [HideInInspector]
    public override GameObject Owner
    {
        get { return base.Owner; }
        set
        {
            base.Owner = value;

            // Attribute owner to all children.

            foreach (var node in m_children)
            {
                node.Owner = value;
            }
        }
    }

    /// <summary>
    /// Clones this node, with a deep copy.
    /// </summary>
    /// <returns>A copy of this node.</returns>
    public override Node Clone()
    {
        // Cloning ourselves.
        CompositeNode node = Instantiate(this);
        // Cloning the all children.
        node.m_children = m_children.ConvertAll(child => child.Clone()); // Iterates over each child, transforms them (making a copy), and aggregates returned results into a list.

        return node;
    }

    public override List<Node> GetAllChildrenAndSelf()
    {
        List<Node> nodes = new List<Node>();
        nodes.Add(this);

        foreach (var child in m_children)
        {
            nodes.AddRange(child.GetAllChildrenAndSelf());
        }

        return nodes;
    }
}
