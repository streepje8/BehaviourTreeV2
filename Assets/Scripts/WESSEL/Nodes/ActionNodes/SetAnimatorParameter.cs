using UnityEngine;

public enum AnimatorParameterType
{
    Float,
    Int,
    Bool
}

public class SetAnimatorParameter : ActionNode
{
    private AnimatorParameterType type;
    private Animator animator;
    private string varname;
    
    public SetAnimatorParameter(string varname, AnimatorParameterType type)
    {
        this.varname = varname;
        this.type = type;
        onRun = Execute;
    }

    public override void Initialize()
    {
        animator = executor.GetComponentInChildren<Animator>();
        if (!animator) animator = executor.gameObject.AddComponent<Animator>();
        base.Initialize();
    }

    public TaskStatus Execute()
    {
        switch (type)
        {
            case AnimatorParameterType.Float:
                if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<float>))
                {
                    Debug.LogError(
                        "Parent does not supply a float, please use a blackboard operation to get one!");
                    return TaskStatus.Failed;
                }
                animator.SetFloat(varname,((BlackboardOperation<float>)blackboard.lastExecutedNode).value);
                break;
            case AnimatorParameterType.Int:
                if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<int>))
                {
                    Debug.LogError(
                        "Parent does not supply an int, please use a blackboard operation to get one!");
                    return TaskStatus.Failed;
                }
                animator.SetInteger(varname,((BlackboardOperation<int>)blackboard.lastExecutedNode).value);
                break;
            case AnimatorParameterType.Bool:
                if (blackboard.lastExecutedNode.GetType() != typeof(BlackboardOperation<bool>))
                {
                    Debug.LogError(
                        "Parent does not supply a boolean, please use a blackboard operation to get one!");
                    return TaskStatus.Failed;
                }
                animator.SetBool(varname,((BlackboardOperation<bool>)blackboard.lastExecutedNode).value);
                break;
        }
        return TaskStatus.Success;
    }
}
