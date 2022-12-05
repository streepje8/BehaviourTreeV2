using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGoalToBlackboard : ActionNode
{
    private NodeAIComponent nodeAI;
    private string varname;
    
    public AIGoalToBlackboard(string varname)
    {
        this.varname = varname;
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
        blackboard.SetVariable(varname, nodeAI.goalIndex);
        return TaskStatus.Success;
    }
}
