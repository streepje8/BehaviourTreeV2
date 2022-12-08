using UnityEngine;

public class GetCoverPointNode : ActionNode
{
    private string output;
    private string covers;
    private string watcher;
    private float distanceFromWall;
    
    public GetCoverPointNode(string outputVariableName, string coversVariableName, string watcherVariableName, float distanceFromWall)
    {
        output = outputVariableName;
        covers = coversVariableName;
        watcher = watcherVariableName;
        this.distanceFromWall = distanceFromWall;
        onRun = Execute;
    }

    public TaskStatus Execute()
    {
        if (blackboard.TryGetReference(covers, out CoverCollection covcol) &&
            blackboard.TryGetReference(watcher, out GameObject watcherobj))
        {
            blackboard.SetVariable(output, covcol.GetClosestCover(executor.gameObject).GetCoverPointFor(watcherobj.transform.position, distanceFromWall));
        }
        return TaskStatus.Success;
    }
}
