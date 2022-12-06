using System;
using System.Linq.Expressions;
using UnityEngine;

public class DecrementBlackboardValue<T> : ActionNode
{
    private string name;
    private T amount;
    private SpecialOperations specialOperation;
    
    //Code from https://stackoverflow.com/questions/8523061/how-to-verify-whether-a-type-overloads-supports-a-certain-operator
    private static bool HasSubtract<Y>() {
        var c = Expression.Constant(default(Y), typeof(Y));
        try {
            _ = Expression.Subtract(c, c); // Throws an exception if - is not defined
            return true;
        } catch {
            return false;
        }
    }
    
    public DecrementBlackboardValue(string name,T amount = default,SpecialOperations specialOperation = SpecialOperations.Normal)
    {
        this.amount = amount;
        this.specialOperation = specialOperation;
        this.name = name;
        onRun = Execute;
    }
    
    public TaskStatus Execute()
    {
        if (!HasSubtract<T>()) return TaskStatus.Failed;
        if (blackboard.TryGetVariable(name, out T current))
        {
            Expression expression;
            switch (specialOperation)
            {
                case SpecialOperations.DeltaTime:
                    expression = Expression.Subtract(Expression.Constant(current, typeof(T)),
                        Expression.Constant(Convert.ChangeType((float)Convert.ChangeType(amount,typeof(float)) * Time.deltaTime,typeof(T)), typeof(T)));
                    break;
                case SpecialOperations.FixedDeltaTime:
                    expression = Expression.Subtract(Expression.Constant(current, typeof(T)),
                        Expression.Constant(Convert.ChangeType((float)Convert.ChangeType(amount,typeof(float)) * Time.fixedDeltaTime,typeof(T)), typeof(T)));
                    break;
                case SpecialOperations.Time:
                    expression = Expression.Subtract(Expression.Constant(current, typeof(T)),
                        Expression.Constant(Convert.ChangeType((float)Convert.ChangeType(amount,typeof(float)) * Time.time,typeof(T)), typeof(T)));
                    break;
                default:
                    expression = Expression.Subtract(Expression.Constant(current, typeof(T)),
                        Expression.Constant(amount, typeof(T)));
                    break;
            }
            blackboard.SetVariable(name, Expression.Lambda<Func<T>>(expression).Compile().Invoke());
            return TaskStatus.Success;
        }
        return TaskStatus.Failed;
    }
}
