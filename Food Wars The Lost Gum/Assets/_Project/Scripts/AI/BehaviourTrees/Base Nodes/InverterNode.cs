using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverterNode : DecoratorNode
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

        if(m_child.m_state == State.Running)
        {
            // We are still running.
            return State.Running;
        }
        else
        {
            // Child is done. Invert its return state.
            switch (m_child.m_state)
            {
                case State.Failure:
                    return State.Success;
                case State.Success:
                    return State.Failure;
                default:
                    throw new System.Exception("InverterNode: Tried to return the inverse of State.Running!");
            }
        }
    }
}
