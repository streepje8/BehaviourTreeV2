using UnityEditor;
using UnityEngine;

public enum GuardState
{
    Patrol,
    WeaponSeach,
    Chase,
    Shoot,
    Confused
}

public class Guard : MonoBehaviour
{
    public WBlackboard blackboard;
    public float chaseRange = 5f;
    public float shootRange = 2f;
    public float guardFOV = 70f;
    public float smokedTime = 0f;
    private BTBaseNode tree;
    
    //Debug
    public TaskStatus currentTreeStatus;
    public GuardState state = GuardState.Patrol;

    private void Start()
    {
        blackboard.SetVariable("guardState", GuardState.Patrol);
        blackboard.SetVariable("hasGun", false);
        blackboard.SetVariable("cooldown", 0f);
        tree = new EntryNode().SetBlackboard(blackboard).SetExecutor(this)
            .Append(new SequenceNode()
                .Append(new BlackboardOperation<int>("guardState",BlackboardOperationType.ReadVariable))
                .Append(new SetAnimatorParameter("guardState",AnimatorParameterType.Int))
                .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state is GuardState.Patrol, "guardState == Patrol")
                    .Append(new SequenceNode()
                        .Append(new BlackboardOperation<AIPath>("Path",BlackboardOperationType.ReadReference))
                        .Append(new AISetPath())
                        .Append(new AIIndexToBlackboard("guardIndex"))
                        .Append(new AIGoalToBlackboard("guardGoal"))
                        .Append(new ConditionalNode(() => blackboard.TryGetReference("View", out Transform view) && blackboard.TryGetReference("Player", out GameObject player) && 
                                Vector3.Angle( view.forward, player.transform.position - view.position) < guardFOV && Vector3.Distance(view.position,player.transform.position) < chaseRange - 0.1f &&
                                Physics.Raycast(view.position,(player.transform.position - view.position).normalized,out RaycastHit hit) && hit.collider.CompareTag("Player")
                                , "Guard can see player")
                            .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.WeaponSeach))
                            .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardIndex", out int index) && blackboard.TryGetVariable("guardGoal", out int goal) && index == goal, "Current AI Index == Goal")
                                .Append(new AINextPoint())
                                .Append(new SequenceNode()
                                    .Append(new AISetSpeed(1.9f))
                                    .Append(new AINavigateToPoint())
                                )
                            )
                        )
                    )
                    .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state == GuardState.WeaponSeach, "guardState == WeaponSearch")
                        .Append(new ConditionalNode(() => blackboard.TryGetVariable("hasGun", out bool hasGun) && !hasGun, "Player has no gun?")
                            .Append(new ConditionalNode(() => blackboard.TryGetReference("Gun", out GameObject gun) && Vector3.Distance(transform.position,gun.transform.position) < 0.5f, "Player next to gun?")
                                .Append(new SequenceNode()
                                    .Append(new DebugLogNode("GunPickup"))
                                    .Append(new BlackboardOperation<bool>("hasGun",BlackboardOperationType.WriteVariable,true))
                                    .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Chase))
                                )
                                .Append(new SequenceNode()
                                    .Append(new AISetSpeed(4f))
                                    .Append(new BlackboardOperation<GameObject>("Gun", BlackboardOperationType.ReadReference))
                                    .Append(new AINavigatePosition(true))
                                )
                            )
                            .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Chase))
                        )
                        .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state == GuardState.Chase, "guardState == Chase")
                            .Append(new ConditionalNode(() => blackboard.TryGetReference("Player", out GameObject player) && Vector3.Distance(transform.position,player.transform.position) > chaseRange,"Player outside of chasing range?")
                                .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Patrol))
                                .Append(new ConditionalNode(() => blackboard.TryGetReference("Player", out GameObject player) && Vector3.Distance(transform.position,player.transform.position) < shootRange, "Player inside of shooting range?")
                                    .Append(new SequenceNode()
                                        .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Shoot))
                                        .Append(new AIStopNode())
                                        .Append(new DebugLogNode("Start Shooting!"))
                                    )
                                    .Append(new SequenceNode() //Chase him
                                        .Append(new AISetSpeed(1.9f))
                                        .Append(new BlackboardOperation<GameObject>("Player", BlackboardOperationType.ReadReference))
                                        .Append(new AINavigatePosition(true)))
                                )
                            )
                            .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state == GuardState.Shoot, "guardState == Shoot")
                                .Append(new ConditionalNode(() => blackboard.TryGetReference("Player", out GameObject player) && Vector3.Distance(transform.position,player.transform.position) > shootRange + 0.5f, "Player outside of shooting range?")
                                    .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Chase))
                                    .Append(new ConditionalNode(() => blackboard.TryGetVariable("cooldown", out float cooldown) && cooldown <= 0, "cooldown <= 0")
                                        .Append(new SequenceNode()
                                            .Append(new DebugLogNode("Shoot sound!"))
                                            .Append(new BlackboardOperation<float>("cooldown",BlackboardOperationType.WriteVariable,2f))
                                        )
                                        .Append(new SequenceNode()
                                            .Append(new DecrementBlackboardValue<float>("cooldown",1f,SpecialOperations.FixedDeltaTime))
                                            .Append(new ConditionalNode(() => smokedTime > 0f)
                                                .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Confused))
                                                .Append(new EmptyNode())
                                            )
                                        )
                                    )
                                )
                                .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state is GuardState.Confused, "guardState == Confused")
                                    .Append(new SequenceNode()
                                        .Append(new ConditionalNode(() => smokedTime <= 0f)
                                            .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Chase))
                                            .Append(new EmptyNode())
                                        )
                                    )
                                    .Append(new DebugLogNode("Invalid State!"))
                                )
                            )
                        )
                    )
                )
            ).Build();
        //Debug.Log(tree.ToPlantUML());
    }

    private void FixedUpdate()
    {
        tree?.RunNode();
        blackboard.TryGetVariable("guardState", out GuardState stat);
        state = stat;
        if (smokedTime > 0)
            smokedTime -= Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.yellow;
        Handles.color = Color.yellow;
        if (blackboard.TryGetReference("View", out Transform viewTransform))
        {
            Vector3 endPointLeft = viewTransform.position +
                                   (Quaternion.Euler(0, -guardFOV, 0) * viewTransform.transform.forward).normalized *
                                   chaseRange;
            Vector3 endPointRight = viewTransform.position +
                                    (Quaternion.Euler(0, guardFOV, 0) * viewTransform.transform.forward).normalized *
                                    chaseRange;
            Handles.DrawWireArc(viewTransform.position, Vector3.up,
                Quaternion.Euler(0, -guardFOV, 0) * viewTransform.transform.forward, guardFOV * 2, chaseRange);
            Gizmos.DrawLine(viewTransform.position, endPointLeft);
            Gizmos.DrawLine(viewTransform.position, endPointRight);
            Gizmos.color = Color.white;
            Handles.color = Color.white;
        }
    }
}
