using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISetPoint : ActionNode
{
    private NodeAIComponent nodeAI;
    
    public AISetPoint()
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
        if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<int>))
        {
            Debug.LogError("Parent does not supply an int (index), please use a blackboard operation to get one!");
            return TaskStatus.Failed;
        }
        nodeAI.goalIndex = ((BlackboardOperation<int>)blackboard.lastExecutedNode).value;
        return TaskStatus.Success;
    }
}
