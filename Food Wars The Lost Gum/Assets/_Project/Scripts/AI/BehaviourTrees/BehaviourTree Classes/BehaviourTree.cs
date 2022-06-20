using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// We add Create Asset Menu attribute to this, but not to Nodes. The Editor is responsible for creating nodes.
[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    /// <summary>
    /// Root node of the tree. This is the first node of the tree.
    /// </summary>
    public Node m_rootNode;

    /// <summary>
    /// List of all nodes in the tree. This can contain nodes that are not linked to root.
    /// </summary>
    public List<Node> m_nodes = new List<Node>();

    /// <summary>
    /// Status of the tree itself.
    /// </summary>
    public Node.State m_treeState = Node.State.Running;

    public bool m_firstTimeRun = true;

    /// <summary>
    /// Game Object that owns this tree.
    /// </summary>
    public GameObject Owner 
    { 
        get { return m_owner; }
        set
        {
            m_owner = value;
            // Attribute owner to all children.

            foreach (var node in m_nodes)
            {
                node.Owner = value;
            }
        }
    }

    /// <summary>
    /// The gameobject that owns this tree.
    /// </summary>
    [HideInInspector] private GameObject m_owner; 

    /// <summary>
    /// Whenever the tree is updated, this method is called.
    /// </summary>
    /// <returns>The status of the tree.</returns>
    public Node.State Update()
    {
        // Failsafe. Can't run without an owner
        if(m_owner == null)
        {
            Debug.LogError("BehaviourTree: No owner assigned. Returning to caller of Update.");
            return Node.State.Failure;
        }

        // Failsafe. If its our first time running, set the owner to all nodes.
        if(m_firstTimeRun)
        {
            foreach (var node in m_nodes)
            {
                node.Owner = Owner;
            }

            m_firstTimeRun = false;
        }

        // If the rootnode is running, we update the tree.
        // Once the root node will stop running, the tree will stop updating.
        if(m_rootNode.m_state == Node.State.Running)
        {
            m_treeState = m_rootNode.Update();
        }

        return m_treeState;
    }
    // Normally, we would only need this if wrapped around the AssetDatabase stuff, since these are only available in the editor and will crash our build.
    // But, we were last minute, so the fix is applied here.
#if UNITY_EDITOR
    /// <summary>
    /// Creates a node of the specified type and adds it to the tree.
    /// </summary>
    /// <param name="pType">Type of the node to create.</param>
    /// <returns>The newly created node.</returns>
    public Node CreateNode(System.Type pType)
    {
        Node node = ScriptableObject.CreateInstance(pType) as Node;
        node.name = pType.Name; // Makes the node appear a bit nicer in the inspector.
        node.m_guid = GUID.Generate().ToString();
        // Passing on Ownership.
        node.Owner = this.Owner;

        m_nodes.Add(node);

        // Add the node scriptable object as a sub asset of the main BehaviourTree object.
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }

    /// <summary>
    /// Deletes a certain node.
    /// </summary>
    /// <param name="pNode">Node to delete.</param>
    public void DeleteNode(Node pNode)
    {
        m_nodes.Remove(pNode);

        // Removes the node scriptable object from the main BehaviourTree object.
        AssetDatabase.RemoveObjectFromAsset(pNode);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Adds a relationship between two nodes.
    /// </summary>
    /// <param name="pParent">Node designated as parent.</param>
    /// <param name="pChild">Node designated as child.</param>
    public void AddChild(Node pParent, Node pChild)
    {
        pChild.Owner = pParent.Owner;

        // Need to check what type of node the parent is.

        DecoratorNode decorator = pParent as DecoratorNode;

        if(decorator)
        {
            decorator.m_child = pChild;
        }

        RootNode root = pParent as RootNode;

        if (root)
        {
            root.m_child = pChild;
        }

        CompositeNode compositeNode = pParent as CompositeNode;

        if(compositeNode)
        {
            compositeNode.m_children.Add(pChild);
        }
    }

    /// <summary>
    /// Removes a relationship between two nodes.
    /// </summary>
    /// <param name="pParent">Node designated as parent.</param>
    /// <param name="pChild">Node designated as child.</param>
    public void RemoveChild(Node pParent, Node pChild)
    {
        // Need to check what type of node the parent is.

        DecoratorNode decorator = pParent as DecoratorNode;

        if (decorator)
        {
            decorator.m_child = null;
        }

        RootNode root = pParent as RootNode;

        if (root)
        {
            root.m_child = null;
        }

        CompositeNode compositeNode = pParent as CompositeNode;

        if (compositeNode)
        {
            compositeNode.m_children.Remove(pChild);
        }

        pChild.Owner = null;
    }

    /// <summary>
    /// Gets all the children nodes of a parent node.
    /// </summary>
    /// <param name="pParent">The parent node to grab children from.</param>
    /// <returns>List of children nodes.</returns>
    public List<Node> GetChildren(Node pParent)
    {
        List<Node> children = new List<Node>();

        // Need to check what type of node the parent is.
        DecoratorNode decorator = pParent as DecoratorNode;

        // If the decorator has children...
        if (decorator && decorator.m_child != null)
        {
            // ... we return its child, in a list.
            children.Add(decorator.m_child);
        }

        RootNode root = pParent as RootNode;

        // If the root node has a child...
        if (root && root.m_child != null)
        {
            // ... we return its child, in a list.
            children.Add(root.m_child);
        }

        CompositeNode compositeNode = pParent as CompositeNode;

        if (compositeNode)
        {
            // If it's a composite node, just return its list of children, empty or not.
            return compositeNode.m_children;
        }

        // If we are here, we had a decorator node that had no children, so we return an empty list.
        return children;
    }
#endif
    /// <summary>
    /// Clones this BehaviourTree, making a deep copy.
    /// </summary>
    /// <returns>A copy of this BehaviourTree.</returns>
    public BehaviourTree Clone()
    {
        // Cloning the tree object itself.
        BehaviourTree tree = Instantiate(this);

        // Cloning the root node. This will create a chain reaction copying the entire tree.
        tree.m_rootNode = tree.m_rootNode.Clone();

        // Updating this cloned tree's children list, as it doesnt get updated with Clone normally.
        tree.m_nodes = tree.m_rootNode.GetAllChildrenAndSelf();

        return tree;
    }

    /// <summary>
    /// Restarts the tree's execution, if the execution has ended.
    /// </summary>
    public void RestartTree()
    {
        if(m_treeState != Node.State.Running)
        {
            if(m_rootNode.m_state != Node.State.Running)
            {
                m_rootNode.m_state = Node.State.Running;
            }
        }
    }

}
