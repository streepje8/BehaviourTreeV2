using UnityEngine;

public class AISetPath : ActionNode
{
    private NodeAIComponent nodeAI;
    
    public AISetPath()
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
        if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<AIPath>))
        {
            Debug.LogError("Parent does not supply an AI Path, please use a blackboard operation to get one!");
            return TaskStatus.Failed;
        }
        nodeAI.currentPath = ((BlackboardOperation<AIPath>)blackboard.lastExecutedNode).value;
        return TaskStatus.Success;
    }
}
