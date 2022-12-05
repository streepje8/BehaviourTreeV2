using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINavigatePosition : ActionNode
{
    private NodeAIComponent nodeAI;
    private bool isGameObject;

    public AINavigatePosition(bool isGameObject = false)
    {
        onRun = Execute;
        this.isGameObject = isGameObject;
    }

    public override void Initialize()
    {
        nodeAI = executor.GetComponent<NodeAIComponent>();
        if (!nodeAI) nodeAI = executor.gameObject.AddComponent<NodeAIComponent>();
        base.Initialize();
    }

    public TaskStatus Execute()
    {
        if (!isGameObject)
        {
            if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<Vector3>))
            {
                Debug.LogError(
                    "Parent does not supply a Vector3 (position), please use a blackboard operation to get one!");
                return TaskStatus.Failed;
            }

            nodeAI.NavigatePosition(((BlackboardOperation<Vector3>)blackboard.lastExecutedNode).value);
            return TaskStatus.Success;
        }
        else
        {
            if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<GameObject>))
            {
                Debug.LogError(
                    "Parent does not supply a GameObject (position), please use a blackboard operation to get one!");
                return TaskStatus.Failed;
            }

            nodeAI.NavigatePosition(((BlackboardOperation<GameObject>)blackboard.lastExecutedNode).value.transform.position);
            return TaskStatus.Success;
        }
    }
}
