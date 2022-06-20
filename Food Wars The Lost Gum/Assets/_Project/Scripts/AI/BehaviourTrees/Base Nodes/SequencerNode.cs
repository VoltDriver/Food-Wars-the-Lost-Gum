using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of node that iterates over its children and executes them. If one child returns a failure, the whole sequence fails.
/// All children need to succeed to return a success.
/// </summary>
public class SequencerNode : CompositeNode
{
    /// <summary>
    /// Current index of the child being executed.
    /// </summary>
    protected int m_currentChildIndex;

    protected override void OnStart()
    {
        m_currentChildIndex = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {       
        var child = m_children[m_currentChildIndex];

        // Execute the current child.
        switch (child.Update())
        {
            case State.Running:
                // The child is not done, so this node is not done.
                return State.Running;
            case State.Failure:
                // If ONE child fails, the state is failure.
                return State.Failure;
            case State.Success:
                // Move on to the next child to execute.
                m_currentChildIndex++;
                break;
        }

        // If we have executed all children, we can return success. Otherwise, we are still running.
        return m_currentChildIndex == m_children.Count ? State.Success : State.Running;
    }
}
