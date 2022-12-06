public class EntryNode : BTBaseNode
{
    public override int maxChildCount => 1;
    
    public override TaskStatus Run()
    {
        return children[0]?.RunNode() ?? TaskStatus.Success;
    }
}
