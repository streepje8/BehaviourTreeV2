using UnityEngine;

public enum RogueState
{
    FollowPlayer,
    CoverSearch,
    Hiding
}

public class Rogue : MonoBehaviour
{
    public WBlackboard blackboard;
    private BTBaseNode tree;

    private void Start()
    {
        blackboard.SetVariable("rogueState",RogueState.FollowPlayer);
        tree = new EntryNode().SetBlackboard(blackboard).SetExecutor(this)
            .Append(new SequenceNode()
                .Append(new ConditionalNode(() => blackboard.TryGetVariable("rogueState", out RogueState state) && state == RogueState.FollowPlayer)
                    .Append(new SequenceNode()
                        .Append(new ConditionalNode(() => blackboard.TryGetReference("Player", out GameObject player) && Vector3.Distance(player.transform.position,transform.position) > 5f)
                            .Append(new SequenceNode()
                                .Append(new BlackboardOperation<GameObject>("Player",BlackboardOperationType.ReadReference))
                                .Append(new GetLocationNode("PlayerPOS"))
                                .Append(new AddRandomVectorOffset("PlayerPOS",new Vector3(2,0,2)))
                                .Append(new AIAtDestinationNode("destinationReached"))
                                .Append(new BlackboardOperation<float>("rogueMoving",BlackboardOperationType.WriteVariable,() => (blackboard.TryGetVariable("destinationReached",out bool reached) && reached) ? 0 : 1))
                                .Append(new BlackboardOperation<Vector3>("PlayerPOS",BlackboardOperationType.ReadVariable))
                                .Append(new AINavigatePosition())
                            )
                            .Append(new SequenceNode()
                                .Append(new AIAtDestinationNode("destinationReached"))
                                .Append(new BlackboardOperation<float>("rogueMoving",BlackboardOperationType.WriteVariable,() => (blackboard.TryGetVariable("destinationReached",out bool reachedd) && reachedd) ? 0 : 1))
                                .Append(new DebugLogNode("Set animation to vibing"))
                            )
                        )
                        .Append(new BlackboardOperation<float>("rogueMoving",BlackboardOperationType.ReadVariable))
                        .Append(new SetAnimatorParameter("moving",AnimatorParameterType.Float))
                    )
                    .Append(new ConditionalNode(() => blackboard.TryGetVariable("rogueState", out RogueState state) && state == RogueState.CoverSearch)
                        .Append(new DebugLogNode("Fuck i need cover!!"))
                        .Append(new ConditionalNode(() => blackboard.TryGetVariable("rogueState", out RogueState state) && state == RogueState.Hiding)
                            .Append(new DebugLogNode("Heheh1e he can't see me here!"))
                            .Append(new DebugLogNode("Invalid State"))
                        )
                    )
                )
                .Append(new BlackboardOperation<int>("rogueState",BlackboardOperationType.ReadVariable))
                .Append(new SetAnimatorParameter("rogueState",AnimatorParameterType.Int))
            ).Build();
    }

    private void FixedUpdate()
    {
        tree?.RunNode();
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Handles.color = Color.yellow;
    //    Vector3 endPointLeft = viewTransform.position + (Quaternion.Euler(0, -ViewAngleInDegrees.Value, 0) * viewTransform.transform.forward).normalized * SightRange.Value;
    //    Vector3 endPointRight = viewTransform.position + (Quaternion.Euler(0, ViewAngleInDegrees.Value, 0) * viewTransform.transform.forward).normalized * SightRange.Value;

    //    Handles.DrawWireArc(viewTransform.position, Vector3.up, Quaternion.Euler(0, -ViewAngleInDegrees.Value, 0) * viewTransform.transform.forward, ViewAngleInDegrees.Value * 2, SightRange.Value);
    //    Gizmos.DrawLine(viewTransform.position, endPointLeft);
    //    Gizmos.DrawLine(viewTransform.position, endPointRight);

    //}
}
