using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINavigatePosition : ActionNode
{
    private NodeAIComponent nodeAI;
    
    public AINavigatePosition()
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
        if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<Vector3>))
        {
            Debug.LogError("Parent does not supply a Vector3 (position), please use a blackboard operation to get one!");
            return TaskStatus.Failed;
        }
        nodeAI.NavigatePosition(((BlackboardOperation<Vector3>)blackboard.lastExecutedNode).value);
        return TaskStatus.Success;
    }
}
