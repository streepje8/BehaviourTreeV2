public class EmptyNode : ActionNode
{
    public EmptyNode() => onRun = () => TaskStatus.Success;
}
