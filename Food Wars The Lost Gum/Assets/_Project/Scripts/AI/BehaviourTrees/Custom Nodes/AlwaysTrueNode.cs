using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysTrueNode : DecoratorNode
{
    protected override void OnStart()
    {
        
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        m_child.Update();

        if (m_child.m_state == State.Running)
        {
            // We are still running.
            return State.Running;
        }
        else
        {
            // Child is done. Make its return state Success
            switch (m_child.m_state)
            {
                case State.Failure:
                    return State.Success;
                case State.Success:
                    return State.Success;
                default:
                    throw new System.Exception("AlwaysTrueNode: Tried to return Success when the node was of State.Running!");
            }
        }
    }
}
