using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class BlackboardOperation<T> : BTBaseNode
{
    public delegate T LambdaValue();
    public T valueA;

    public T value
    {
        get
        {
            if (valueB != null)
                return valueB.Invoke();
            return valueA;
        }
    }
    public LambdaValue valueB;
    private string name;
    private BlackboardOperationType type;
    
    public BlackboardOperation(string name, BlackboardOperationType type)
    {
        this.name = name;
        valueA = default;
        valueB = null;
        this.type = type;
    }
    
    public BlackboardOperation(string name, BlackboardOperationType type, T value = default)
    {
        this.name = name;
        valueA = value;
        valueB = null;
        this.type = type;
    }
    
    public BlackboardOperation(string name, BlackboardOperationType type, LambdaValue valueB = null)
    {
        this.name = name;
        valueA = default;
        this.valueB = valueB;
        this.type = type;
    }

    public override TaskStatus Run()
    {
        switch (type)
        {
            case BlackboardOperationType.ReadVariable:
                if (blackboard.TryGetVariable(name, out T val))
                {
                    valueA = val;
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
                        valueA = (T)Convert.ChangeType(outputVal, typeof(T)); //This code is illegal but required and in this case safe
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
