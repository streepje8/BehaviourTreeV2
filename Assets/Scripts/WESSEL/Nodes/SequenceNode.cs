public class SequenceNode : BTBaseNode
{
    public override int maxChildCount => 9999;

    public override TaskStatus Run()
    {
        for (var i = 0; i < children.Length; i++)
        {
            TaskStatus stat = children[i]?.RunNode() ?? TaskStatus.Failed;
            if (stat == TaskStatus.Failed)
                return TaskStatus.Failed;
        }
        return TaskStatus.Success;
    }
}
