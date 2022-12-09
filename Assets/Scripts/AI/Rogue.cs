using TMPro;
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
    public float distanceFromWall = 1.5f;
    public TMP_Text infoText;
    private BTBaseNode tree;

    private void Start()
    {
        blackboard.SetVariable("rogueState",RogueState.FollowPlayer);
        blackboard.SetVariable("cooldown", 0f);
        tree = new EntryNode().SetBlackboard(blackboard).SetExecutor(this)
            .Append(new SequenceNode()
                .Append(new ConditionalNode(() => blackboard.TryGetVariable("rogueState", out RogueState state) && state == RogueState.FollowPlayer, "state == FollowPlayer")
                    .Append(new SequenceNode()
                        .Append(new ConditionalNode(() => blackboard.TryGetReference("Player", out GameObject player) && Vector3.Distance(player.transform.position,transform.position) > 5f, "Distance To Player > 5f")
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
                            )
                        )
                        .Append(new BlackboardOperation<float>("rogueMoving",BlackboardOperationType.ReadVariable))
                        .Append(new SetAnimatorParameter("moving",AnimatorParameterType.Float))
                        .Append(new ConditionalNode(() => blackboard.TryGetReference("Guard", out Guard guard) && guard.state is GuardState.Chase or GuardState.Shoot,"Enemy Spotted Player?")
                            .Append(new BlackboardOperation<RogueState>("rogueState",BlackboardOperationType.WriteVariable,RogueState.CoverSearch))
                            .Append(new EmptyNode())
                        )
                    )
                    .Append(new ConditionalNode(() => blackboard.TryGetVariable("rogueState", out RogueState state) && state == RogueState.CoverSearch, "state == CoverSearch")
                        .Append(new SequenceNode()
                            .Append(new GetCoverPointNode("coverPOS", "Covers", "GuardObj",distanceFromWall))
                            .Append(new BlackboardOperation<Vector3>("coverPOS", BlackboardOperationType.ReadVariable))
                            .Append(new AINavigatePosition())
                            .Append(new AIAtDestinationNode("destinationReached"))
                            .Append(new BlackboardOperation<float>("rogueMoving",BlackboardOperationType.WriteVariable,() => (blackboard.TryGetVariable("destinationReached",out bool reached) && reached) ? 0 : 1))
                            .Append(new BlackboardOperation<float>("rogueMoving",BlackboardOperationType.ReadVariable))
                            .Append(new SetAnimatorParameter("moving",AnimatorParameterType.Float))
                            .Append(new ConditionalNode(() => blackboard.TryGetReference("Guard", out Guard guard) && guard.state is GuardState.Patrol, "state == Patrol")
                                .Append(new BlackboardOperation<RogueState>("rogueState",BlackboardOperationType.WriteVariable,RogueState.FollowPlayer))
                                .Append(new EmptyNode())
                            )
                            .Append(new AIAtDestinationNode("destinationReached"))
                            .Append(new ConditionalNode(() => blackboard.TryGetVariable("destinationReached", out bool reached) && reached ,"Destination Reached?")
                                .Append(new SequenceNode()
                                    .Append(new BlackboardOperation<RogueState>("rogueState",BlackboardOperationType.WriteVariable,RogueState.Hiding))
                                )
                                .Append(new EmptyNode())
                            )
                        )
                        .Append(new ConditionalNode(() => blackboard.TryGetVariable("rogueState", out RogueState state) && state == RogueState.Hiding, "state == Hiding")
                            .Append(new SequenceNode()
                                .Append(new ConditionalNode(() => blackboard.TryGetVariable("cooldown", out float cooldown) && cooldown <= 0,"cooldown <= 0")
                                    .Append(new ConditionalNode(() => blackboard.TryGetReference("Guard", out Guard guard) && guard.state is GuardState.Confused, "guardState == Confused")
                                        .Append(new BlackboardOperation<RogueState>("rogueState",BlackboardOperationType.WriteVariable,RogueState.FollowPlayer))
                                        .Append(new SequenceNode()
                                            .Append(new DebugLogNode("Throwing Smoke bomb"))
                                            .Append(new InstantiateObjectNode("lastSmokeBomb",blackboard.TryGetReference("SmokeBomb", out GameObject value) ? value : null))
                                            .Append(new ExecuteLambdaNode(() =>
                                            {
                                                if (blackboard.TryGetVariable("lastSmokeBomb", out GameObject lastBomb)
                                                    && blackboard.TryGetReference("Guard", out Guard value))
                                                {
                                                    lastBomb.transform.position = transform.position;
                                                    lastBomb.GetComponent<SmokeBomb>().target = value.transform;
                                                    return TaskStatus.Success;
                                                }
                                                return TaskStatus.Failed;
                                            },"Set Position, Arc Target"))
                                            .Append(new BlackboardOperation<float>("cooldown",BlackboardOperationType.WriteVariable,2f))
                                        )
                                    )
                                    .Append(new DecrementBlackboardValue<float>("cooldown",1f,SpecialOperations.DeltaTime))
                                )
                                .Append(new ConditionalNode(() => blackboard.TryGetReference("Guard", out Guard guard) && guard.state is GuardState.Patrol, "guardState == Patrol")
                                    .Append(new BlackboardOperation<RogueState>("rogueState",BlackboardOperationType.WriteVariable,RogueState.FollowPlayer))
                                    .Append(new EmptyNode())
                                )
                            )
                            .Append(new DebugLogNode("Invalid State"))
                        )
                    )
                )
                .Append(new BlackboardOperation<int>("rogueState",BlackboardOperationType.ReadVariable))
                .Append(new SetAnimatorParameter("rogueState",AnimatorParameterType.Int))
            ).Build();
        Debug.Log(tree.ToPlantUML());
    }

    private void FixedUpdate()
    {
        tree?.RunNode();
        if (blackboard.TryGetVariable("rogueState", out RogueState state))
        {
            infoText.text = "State: " + state;
        }
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
