using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskStatus { Success, Failed, Running }
public abstract class BTBaseNode
{
    public WBlackboard blackboard;
    public virtual int maxChildCount { get; private set; } = 0;
    public BTBaseNode parent;
    public BTBaseNode[] children = Array.Empty<BTBaseNode>();
    public abstract TaskStatus Run();

    public BTBaseNode SetBlackboard(WBlackboard blackboard)
    {
        this.blackboard = blackboard;
        blackboard.BuildToDictionary();
        return this;
    }

    public BTBaseNode Append(BTBaseNode child)
    {
        if (children.Length < maxChildCount)
        {
            child.parent = this;
            child.blackboard = blackboard;
            children = children.Append(child).ToArray();
        }
        else
        {
            Debug.LogError(GetType().FullName + " can only have " + maxChildCount + " child nodes.");
        }
        return this;
    }
}
