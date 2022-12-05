using UnityEngine;

public class DebugLogNode : ActionNode
{
    private string message;
    public DebugLogNode(string message)
    {
        this.message = message;
        onRun = () =>
        {
            Debug.Log(message);
            return TaskStatus.Success;
        };
    }
}
