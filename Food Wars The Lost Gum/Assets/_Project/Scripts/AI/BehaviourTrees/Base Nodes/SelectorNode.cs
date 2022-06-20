using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
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
                // Move on to the next child to execute.
                m_currentChildIndex++;
                break;
            case State.Success:
                // If ONE child SUCCEEDS, the state is success.
                return State.Success;
        }

        // If we have executed all children, we can return failure as it means none of them succeeded. Otherwise, we are still running.
        return m_currentChildIndex == m_children.Count ? State.Failure : State.Running;
    }
}
