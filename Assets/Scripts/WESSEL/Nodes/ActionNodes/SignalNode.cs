using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalNode : ActionNode
{
    private string name;
    
    public SignalNode(string name)
    {
        onRun = Execute;
        this.name = name;
    }

    public TaskStatus Execute()
    {
        for (var i = SignalReciever.recievers.Count - 1; i >= 0; i--)
        {
            SignalReciever.recievers[i].Recieve(executor, name);
        }
        return TaskStatus.Success;
    }
}
