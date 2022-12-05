using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStopNode : ActionNode
{
    private NodeAIComponent nodeAI;
    
    public AIStopNode()
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
        nodeAI.agent.SetDestination(nodeAI.transform.position);
        return TaskStatus.Success;
    }
}
