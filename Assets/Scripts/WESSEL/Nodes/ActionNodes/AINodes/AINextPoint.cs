using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINextPoint : ActionNode
{
    private NodeAIComponent nodeAI;
    
    public AINextPoint()
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
        nodeAI.goalIndex++;
        if (nodeAI.goalIndex >= nodeAI.currentPath.waypoints.Count) nodeAI.goalIndex = 0;
        return TaskStatus.Success;
    }
}
