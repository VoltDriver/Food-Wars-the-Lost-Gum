using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    /// <summary>
    /// Callback that the node will call when a node is selected.
    /// This allows an event to bubble up to the GUI.
    /// </summary>
    public Action<NodeView> OnNodeSelected;

    /// <summary>
    /// Node viewed by this view, i.e. currently edited node.
    /// </summary>
    public Node m_node;

    /// <summary>
    /// Input port of this nodeview. Basically, recipient for an edge.
    /// </summary>
    public Port m_input;

    /// <summary>
    /// Output port of this nodeview. It's the start of an edge going towards another node.
    /// </summary>
    public Port m_output;



    public NodeView(Node pNode)
    {
        m_node = pNode;
        this.title = pNode.m_displayName + " (" + pNode.name + ")";

        // Metadata used to retrieve the NodeView from the main graph view.
        this.viewDataKey = m_node.m_guid;

        // Set the position of the node. This is how we move the node in the graph view.
        style.left = m_node.m_position.x;
        style.top = m_node.m_position.y;

        // Initialize ports.
        CreateInputPorts();
        CreateOutputPorts();

        m_node.OnDisplayNameChange += OnDisplayNameChanged;
    }

    /// <summary>
    /// Creates input ports needed to make edges between nodes in the graph, and have them communicate together.
    /// </summary>
    private void CreateInputPorts()
    {
        // Input port behavior COULD be different depending on the type of node. But in our case, atm, it's not.
        if (m_node is ActionNode)
        {
            m_input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        } 
        else if (m_node is CompositeNode)
        {
            m_input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (m_node is DecoratorNode)
        {
            m_input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (m_node is RootNode)
        {
            // This is the only type of node that has no inputs.
        }

        // If we have an input port.
        if(m_input != null)
        {
            m_input.portName = ""; // Clearing the silly pre generated port name for our port.
            inputContainer.Add(m_input);
        }
    }

    /// <summary>
    /// Creates output ports needed to make edges between nodes in the graph, and have them communicate together.
    /// </summary>
    private void CreateOutputPorts()
    {
        // Output port behavior is different depending on the type of node.
        if (m_node is ActionNode)
        {
            // ActionNode has no children, so should not have any output.
           
        }
        else if (m_node is CompositeNode)
        {
            // CompositeNode can have multiple children
            m_output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (m_node is DecoratorNode)
        {
            // Decorator node has only a single child.
            m_output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (m_node is RootNode)
        {
            // Root node has only a single child.
            m_output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        // If we have an input port.
        if (m_output != null)
        {
            m_output.portName = ""; // Clearing the silly pre generated port name for our port.
            outputContainer.Add(m_output);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        // Save the position of where the node is in the graph directly to the object, to save it between reloads of the BehaviourTree.
        m_node.m_position.x = newPos.xMin;
        m_node.m_position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();

        // If the listener is set...
        if(OnNodeSelected != null)
        {
            // ... Throw the event.
            OnNodeSelected.Invoke(this);
        }
    }

    public void OnDisplayNameChanged()
    {
        this.title = m_node.m_displayName + " (" + m_node.name + ")";
    }
}
