using System;

public class ActionNode : BTBaseNode
{
    public delegate TaskStatus NodeAction();
    protected NodeAction onRun;

    public ActionNode(NodeAction onRun = null)
    {
        this.onRun = onRun;
    }
    
    public override TaskStatus Run()
    {
        return onRun?.Invoke() ?? TaskStatus.Failed;
    }
}
