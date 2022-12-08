public class ExecuteLambdaNode : ActionNode
{
    private string comment;
    public ExecuteLambdaNode(NodeAction toRun, string comment)
    {
        this.comment = comment;
        onRun = toRun;
    }
}
