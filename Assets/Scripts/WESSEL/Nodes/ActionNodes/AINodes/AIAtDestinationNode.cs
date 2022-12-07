using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAtDestinationNode : ActionNode
{
    private NodeAIComponent nodeAI;
    private string outputVariable;
    
    public AIAtDestinationNode(string outputVariable)
    {
        this.outputVariable = outputVariable;
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
        blackboard.SetVariable(outputVariable, nodeAI.agent.remainingDistance < 0.1f);
        return TaskStatus.Success;
    }
}
