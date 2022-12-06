using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISetSpeed : ActionNode
{
    private NodeAIComponent nodeAI;
    private float speed;
    
    public AISetSpeed(float speed)
    {
        this.speed = speed;
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
        nodeAI.agent.speed = speed;
        return TaskStatus.Success;
    }
}
