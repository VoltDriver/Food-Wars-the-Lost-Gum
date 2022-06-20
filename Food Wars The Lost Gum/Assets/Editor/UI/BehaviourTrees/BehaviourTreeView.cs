using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;

/// <summary>
/// This class allows us to use the experimental GraphView control in Unity UI Builder.
/// </summary>
public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    /// <summary>
    /// Tree that is currently being viewed.
    /// </summary>
    BehaviourTree m_tree;

    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer()); // Lets you zoom on the content
        this.AddManipulator(new ContentDragger()); // Lets you pan around the graph view.
        this.AddManipulator(new SelectionDragger()); // Lets you drag a single node.
        this.AddManipulator(new RectangleSelector()); // Let's you click and drag to rectangle select.

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/UI/BehaviourTrees/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    /// <summary>
    /// Finds the NodeView object of a certain Node.
    /// </summary>
    /// <param name=pNode">The node to find the view of.</param>
    /// <returns>The nodeview corresponding to this element.</returns>
    NodeView FindNodeView(Node pNode)
    {
        return GetNodeByGuid(pNode.m_guid) as NodeView;
    }

    /// <summary>
    /// Populates tree view with nodes from a certain behaviour tree.
    /// </summary>
    /// <param name="pTree">The tree to use to populate the view.</param>
    public void PopulateView(BehaviourTree pTree)
    {
        m_tree = pTree;

        // Unsubscribe from the event before deleting the graph contents.
        graphViewChanged -= OnGraphViewChanged;
        // Clear out anything that was created from the previous tree.
        DeleteElements(graphElements.ToList());
        // Subscribing again.
        graphViewChanged += OnGraphViewChanged;

        // Check if the tree has a root.
        if(m_tree.m_rootNode == null)
        {
            // No root on our tree. We NEED to have one.
            m_tree.m_rootNode = m_tree.CreateNode(typeof(RootNode)) as RootNode;

            // Prevents loss of changes that happen sometimes after an assembly reload.
            EditorUtility.SetDirty(m_tree);
            AssetDatabase.SaveAssets();
        }

        // Creates a node view for each node in the tree.
        m_tree.m_nodes.ForEach(n => CreateNodeView(n));

        // Creates an edge view for each edge in the graph.
        m_tree.m_nodes.ForEach(n =>
        {
            // Getting all children
            var children = m_tree.GetChildren(n);

            NodeView parentView = FindNodeView(n);

            // For each child, create its edge.
            children.ForEach(child =>
            {
                // Getting the NodeView component.
                NodeView childView = FindNodeView(child);

                // Connecting both nodes on code level.
                Edge edge = parentView.m_output.ConnectTo(childView.m_input);
                // Connecting both nodes in the graph, on a visual level
                AddElement(edge);
            });
        });
    }

    /// <summary>
    /// Gets all the compatible ports where startPort can send a message.
    /// </summary>
    /// <param name="startPort">The port that needs to send a message</param>
    /// <param name="nodeAdapter"></param>
    /// <returns>All ports the message can be sent to from the startPort.</returns>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
        endPort.direction != startPort.direction // Checking that our directions dont match, so that we can make a proper connection.
        && endPort.node != startPort.node // Check that a node isn't trying to connect to itself.
        ).ToList();
    }

    /// <summary>
    /// Function called whenever the Graph View changes.
    /// </summary>
    /// <param name="pGraphViewChange">Container about what changes were made in the graph.</param>
    /// <returns>Same container as was passed in parameters, except it may have been modified in this function.</returns>
    private GraphViewChange OnGraphViewChanged(GraphViewChange pGraphViewChange)
    {
        // We are using this function to check for any deletion of nodes in the graph while being viewed.
        // If a node is deleted, we want to remove it from the tree object itself.

        // If we are removing some items...
        if (pGraphViewChange.elementsToRemove != null)
        {
            pGraphViewChange.elementsToRemove.ForEach(elem =>
            {
                // Trying to cast the element as a NodeView
                NodeView nodeView = elem as NodeView;

                // If the element is a node view
                if(nodeView != null)
                {
                    // Delete the node from the tree.
                    m_tree.DeleteNode(nodeView.m_node);
                }

                // Trying to cast the element as an edge
                Edge edge = elem as Edge;

                // If the element is an edge
                if (edge != null)
                {
                    // Getting nodeviews of the two ends of the edge.
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    // Deleting the relationship in the tree itself (by deleting it in the nodes variables).
                    m_tree.RemoveChild(parentView.m_node, childView.m_node);
                }
            });
        }

        // If we are creating some edges, some connections between nodes...
        if(pGraphViewChange.edgesToCreate != null)
        {
            pGraphViewChange.edgesToCreate.ForEach(edge =>
            {
                // Getting nodeviews of the two ends of the edge.
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;

                // Saving the relationship in the tree itself (by saving it in the nodes variables).
                m_tree.AddChild(parentView.m_node, childView.m_node);
            });
        }

        return pGraphViewChange;
    }

    // This method is overridden to allow us to have new options on the contextual menu (right click), in order to add nodes by right clicking our grid.
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt); // We don't need this base behavior, but keeping it here to remind us it exists.

        // Using C# scopes to be able to recreate any local variables later with the same names.
        // Not needed, but hey it makes the code look more... homogeneous.
        {
            // Using Reflection to grab all the Types that derive from ActionNode, so all our ActionNode classes.
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();

            foreach (var type in types)
            {
                // Creating the context menu button for the type of node, and assigning it an action.
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        // Same as above, but for Composite nodes.
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();

            foreach (var type in types)
            {
                // Creating the context menu button for the type of node, and assigning it an action.
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        // Same as above, but for Decorator nodes.
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();

            foreach (var type in types)
            {
                // Creating the context menu button for the type of node, and assigning it an action.
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        // Adding a refresh button
        evt.menu.AppendAction("Refresh", (a) =>
        {
            // check if we have a tree defined
            if (m_tree)
            {
                // Repopulate the view
                PopulateView(m_tree);
            }
        });

    }

    /// <summary>
    /// Creates a new node in the edited (or viewed) tree.
    /// </summary>
    /// <param name="pType">Type of node to create.</param>
    void CreateNode(System.Type pType)
    {
        Node node = m_tree.CreateNode(pType);
        CreateNodeView(node);
    }

    /// <summary>
    /// Creates a view for the specified node, to be able to edit it.
    /// </summary>
    /// <param name="pNode">Node to create the view for.</param>
    void CreateNodeView(Node pNode)
    {
        NodeView nodeView = new NodeView(pNode);

        // Setting the callback for the node. This allows nodes to bubble up events back to the GUI.
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }
}
