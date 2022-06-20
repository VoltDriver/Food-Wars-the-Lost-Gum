using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    /// <summary>
    /// Possible states for a node. Running means it is currently executing something, Success means it has finished running and it went ok.
    /// Failure means it has finished running, but something went wrong (sometimes, that is expected with AI).
    /// </summary>
    public enum  State { Running, Failure, Success }


    public Action OnDisplayNameChange;

    /// <summary>
    /// Display name, to help coders in the editor.
    /// </summary>
    public string m_displayName = "";

    /// <summary>
    /// Current state of the node.
    /// </summary>
    [HideInInspector] public State m_state = State.Running;

    /// <summary>
    /// Signifies whether the node has started executing or not.
    /// </summary>
    [HideInInspector] public bool m_started = false;

    /// <summary>
    /// Unique identifier for the node.
    /// </summary>
    [HideInInspector] public string m_guid;

    /// <summary>
    /// Position of this node on the BehaviourTree graph view.
    /// </summary>
    public Vector2 m_position = Vector2.zero;

    // The game object that owns this node.  Usually gotten from the parent tree.
    private GameObject m_owner;

    [HideInInspector]
    public virtual GameObject Owner
    {
        get { return m_owner; }
        set
        {
            m_owner = value;
        }
    }

    /// <summary>
    /// Runs a node.
    /// Whenever this node is updated / ran, this will execute.
    /// </summary>
    /// <returns>The status of this node.</returns>
    public State Update()
    {
        // If node has not started...
        if(!m_started)
        {
            // ... initialize it.
            OnStart();
            m_started = true;
        }

        m_state = OnUpdate();

        // Check if the node finished
        if(m_state == State.Failure || m_state == State.Success)
        {
            // Execute some cleanup, or other things, when node is done.
            OnStop();
            // Node is done, so we set its started value back to false.
            m_started = false;
        }

        return m_state;
    }

    /// <summary>
    /// Clones this node, with a deep copy.
    /// </summary>
    /// <returns>A copy of this node.</returns>
    public virtual Node Clone()
    {
        Node clone = Instantiate(this);
        clone.Owner = this.Owner;
        return clone;
    }

    public virtual List<Node> GetAllChildrenAndSelf()
    {
        List<Node> nodes = new List<Node>();
        nodes.Add(this);
        return nodes;
    }

    public void OnValidate()
    {
        // We always invoke this to keep the Editor name of nodes up to date.
        if (OnDisplayNameChange != null)
            OnDisplayNameChange.Invoke();
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();

    public virtual void CollisionDetected() { }
}
