using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogNode : BTBaseNode
{
    private string message;
    public DebugLogNode(string message)
    {
        this.message = message;
    }
    public override TaskStatus Run()
    {
        Debug.Log(message);
        return TaskStatus.Success;
    }
}
