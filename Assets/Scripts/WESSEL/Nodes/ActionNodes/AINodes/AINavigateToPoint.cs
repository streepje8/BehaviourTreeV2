using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINavigateToPoint : ActionNode
{
    private NodeAIComponent nodeAI;
    
    public AINavigateToPoint()
    {
        onRun = Execute;
    }

    public override void Initialize()
    {
        nodeAI = executor.GetComponent<NodeAIComponent>();
        if (!nodeAI) nodeAI = executor.gameObject.AddComponent<NodeAIComponent>();
        base.Initialize();
    }

    public TaskStatus Execute()
    {
        nodeAI.Navigate();
        return TaskStatus.Success;
    }
}
