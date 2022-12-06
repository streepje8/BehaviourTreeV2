using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class BlackboardOperation<T> : BTBaseNode
{
    public T value = default;
    private string name;
    private BlackboardOperationType type;
    
    public BlackboardOperation(string name, BlackboardOperationType type, T value = default)
    {
        this.name = name;
        this.value = value;
        this.type = type;
    }

    public override TaskStatus Run()
    {
        switch (type)
        {
            case BlackboardOperationType.ReadVariable:
                if (blackboard.TryGetVariable(name, out T val))
                {
                    value = val;
                    return TaskStatus.Success;
                }
                break;
            case BlackboardOperationType.WriteVariable:
                if (blackboard.SetVariable(name, value))
                    return TaskStatus.Success;
                break;
            case BlackboardOperationType.ReadReference:
                if (blackboard.TryGetReference(name, out Object outputVal))
                {
                    if (typeof(T) == outputVal.GetType())
                    {
                        value = (T)Convert.ChangeType(outputVal, typeof(T)); //This code is illegal but required and in this case safe
                        return TaskStatus.Success;
                    }
                }
                break;
            case BlackboardOperationType.WriteReference:
                if(value.GetType().IsSubclassOf(typeof(Object)))
                    if (blackboard.SetReference(name, (Object)Convert.ChangeType(value, typeof(Object)))) //This code is illegal but required and in this case safe
                        return TaskStatus.Success;
                break;
        }
        return TaskStatus.Failed;
    }
}
