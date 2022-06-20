using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of node that repeats its child node a set number of times, or infinitely.
/// </summary>
public class RepeatNode : DecoratorNode
{
    /// <summary>
    /// Maximum amount of repetitions this node will trigger.
    /// -1 means an infinite amount of repetitions.
    /// </summary>
    public int m_maxLoops = -1;

    /// <summary>
    /// How many loops this node has done in its current execution.
    /// </summary>
    protected int m_currentLoopCount = 0;

    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        // Check if we have executed our desired number of loops
        if(m_currentLoopCount < m_maxLoops || m_maxLoops == -1)
        {
            // Executing a loop.

            m_child.Update();

            m_currentLoopCount++;

            return State.Running;
        }
        else
        {
            // We are done with our loop executions.

            // Resetting our loop count
            m_currentLoopCount = 0;

            // Returning our state, which is done (so success or failure).
            return m_child.m_state;
        }
    }
}
