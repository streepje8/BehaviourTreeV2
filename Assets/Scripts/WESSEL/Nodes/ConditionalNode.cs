public class ConditionalNode : BTBaseNode
{
    public override int maxChildCount => 2;
    public delegate bool Condition();
    private Condition conditionEvaluator;
    private string comment;

    public ConditionalNode(Condition c, string comment = "Unknown Condition")
    {
        conditionEvaluator = c;
        this.comment = comment;
    }
    
    public override TaskStatus Run()
    {
        if (conditionEvaluator.Invoke())
            return children[0]?.RunNode() ?? TaskStatus.Failed;
        return children[1]?.RunNode() ?? TaskStatus.Failed;
    }
}
