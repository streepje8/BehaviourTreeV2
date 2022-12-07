using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetLocationNode : ActionNode
{
    private string whereToSave;
    public GetLocationNode(string whereToSave)
    {
        this.whereToSave = whereToSave;
        onRun = Execute;
    }

    public TaskStatus Execute()
    {
        if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<GameObject>))
        {
            Debug.LogError(
                "Parent does not supply a GameObject (position), please use a blackboard operation to get one!");
            return TaskStatus.Failed;
        }
        blackboard.SetVariable(whereToSave,
            ((BlackboardOperation<GameObject>)blackboard.lastExecutedNode).value.transform.position);
        return TaskStatus.Success;
    }
}
