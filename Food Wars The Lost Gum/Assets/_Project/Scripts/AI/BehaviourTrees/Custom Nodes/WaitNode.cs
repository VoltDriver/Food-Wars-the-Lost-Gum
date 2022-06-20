using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Type of node that adds a delay to its behavior tree. Waits a set amount of time.
/// </summary>
public class WaitNode : ActionNode
{
    /// <summary>
    /// Time to wait before returning a success.
    /// </summary>
    public float m_waitTime = 1;

    /// <summary>
    /// Time this node's execution was started.
    /// </summary>
    protected float m_startTime;

    protected override void OnStart()
    {
        m_startTime = Time.time;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        // Check if the time elapsed is greater than our wait time.
        if(Time.time - m_startTime >= m_waitTime)
        {
            // We have waited long enough. Return success.
            return State.Success;
        }

        return State.Running;
    }
}
