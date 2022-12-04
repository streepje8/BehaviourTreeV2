using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WriteToBlackboard<T> : BTBaseNode
{
    private T value;
    private string name;
    
    public WriteToBlackboard(string name, T value)
    {
        this.name = name;
        this.value = value;
    }

    public override TaskStatus Run()
    {
        return TaskStatus.Success;
    }
}
