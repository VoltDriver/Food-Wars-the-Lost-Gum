using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node that logs a message to the console when executed.
/// </summary>
public class DebugLogNode : ActionNode
{
    public string m_message;

    protected override void OnStart()
    {
        Debug.Log($"OnStart{m_message}");
    }

    protected override void OnStop()
    {
        Debug.Log($"OnStop{m_message}");
    }

    protected override State OnUpdate()
    {
        Debug.Log($"OnUpdate{m_message}");

        // There is basically no way Debug.Log can fail, so we just return success.
        return State.Success;
    }
}
