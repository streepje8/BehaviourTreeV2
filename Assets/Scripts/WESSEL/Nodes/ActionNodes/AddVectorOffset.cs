using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddRandomVectorOffset : ActionNode
{
    private string variable;
    private Vector3 amount;
    public AddRandomVectorOffset(string variable, Vector3 amount)
    {
        this.variable = variable;
        this.amount = amount;
        onRun = Execute;
    }

    public TaskStatus Execute()
    {
        if (blackboard.TryGetVariable(variable, out Vector3 current))
        {
            Vector3 offset = current + new Vector3(Random.Range(-amount.x, amount.x), Random.Range(-amount.y, amount.y),
                Random.Range(-amount.z, amount.z));
            blackboard.SetVariable(variable, offset);
            return TaskStatus.Success;
        }
        return TaskStatus.Failed;
    }
}
