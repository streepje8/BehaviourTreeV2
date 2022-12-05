public class ConditionalNode : BTBaseNode
{
    public override int maxChildCount => 2;
    public delegate bool Condition();
    private Condition condition;

    public ConditionalNode(Condition c)
    {
        condition = c;
    }
    
    public override TaskStatus Run()
    {
        if (condition.Invoke())
            return children[0]?.RunNode() ?? TaskStatus.Failed;
        return children[1]?.RunNode() ?? TaskStatus.Failed;
    }
}
