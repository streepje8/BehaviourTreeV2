using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TaskStatus { Success, Failed, Running }
public abstract class BTBaseNode
{
    public WBlackboard blackboard;
    public MonoBehaviour executor;
    public virtual int maxChildCount { get; private set; } = 0;
    public BTBaseNode parent;
    public BTBaseNode[] children = Array.Empty<BTBaseNode>();

    public virtual TaskStatus RunNode()
    {
        TaskStatus stat = Run();
        blackboard.lastExecutedNode = this;
        return stat;
    }
    public abstract TaskStatus Run();

    public virtual void Initialize()
    {
        for (int i = 0; i < children.Length; i++)
        {
            children[i].Initialize();
        }
    }
    
    public BTBaseNode Build()
    {
        blackboard?.BuildToDictionary();
        Initialize();
        return this;
    }
    
    public BTBaseNode SetBlackboard(WBlackboard blackboard)
    {
        this.blackboard = blackboard;
        foreach (BTBaseNode child in children)
        {
            child.SetBlackboard(blackboard);
        }
        return this;
    }

    public BTBaseNode SetExecutor(MonoBehaviour executor)
    {
        this.executor = executor;
        foreach (BTBaseNode child in children)
        {
            child.SetExecutor(executor);
        }
        return this;
    }

    public BTBaseNode Append(BTBaseNode child)
    {
        if (children.Length < maxChildCount)
        {
            child.parent = this;
            child.SetBlackboard(blackboard);
            child.SetExecutor(executor);
            children = children.Append(child).ToArray();
        }
        else
        {
            Debug.LogError(GetType().FullName + " can only have " + maxChildCount + " child nodes.");
        }
        return this;
    }
}
