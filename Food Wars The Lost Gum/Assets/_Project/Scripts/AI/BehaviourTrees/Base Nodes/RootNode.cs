using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a special node that is specifically the root of its tree.
/// </summary>
public class RootNode : Node
{
    [HideInInspector] public Node m_child;

    [HideInInspector]
    public override GameObject Owner
    {
        get { return base.Owner; }
        set
        {
            base.Owner = value;

            m_child.Owner = value;
        }
    }

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        // Forwarding the update call to its child.
        return m_child.Update();
    }

    /// <summary>
    /// Clones this node, with a deep copy.
    /// </summary>
    /// <returns>A copy of this node.</returns>
    public override Node Clone()
    {
        // Cloning ourselves.
        RootNode node = Instantiate(this);
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
