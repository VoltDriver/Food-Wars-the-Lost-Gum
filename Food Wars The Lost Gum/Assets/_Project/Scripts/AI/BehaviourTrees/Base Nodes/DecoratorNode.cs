using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of node that augments the child's behavior. Has only one child.
/// </summary>
public abstract class DecoratorNode : Node
{
    [HideInInspector] public Node m_child;

    [HideInInspector]
    public override GameObject Owner
    {
        get { return base.Owner; }
        set
        {
            base.Owner = value;

            if(m_child != null)
                m_child.Owner = value;
        }
    }


    /// <summary>
    /// Clones this node, with a deep copy.
    /// </summary>
    /// <returns>A copy of this node.</returns>
    public override Node Clone()
    {
        // Cloning ourselves.
        DecoratorNode node = Instantiate(this);
        // Cloning the first child.
        node.m_child = m_child.Clone();

        return node;
    }

    public override List<Node> GetAllChildrenAndSelf()
    {
        List<Node> nodes = new List<Node>();
        nodes.Add(this);
        nodes.AddRange(m_child.GetAllChildrenAndSelf());
        return nodes;
    }
}
