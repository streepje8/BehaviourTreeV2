using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

public enum GuardState
{
    Patrol,
    WeaponSeach,
    Chase,
    Shoot
}

public class Guard : MonoBehaviour
{
    public WBlackboard blackboard;
    public float chaseRange = 5f;
    public float shootRange = 2f;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;
    
    //Debug
    public TaskStatus currentTreeStatus;
    public GuardState state = GuardState.Patrol;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        blackboard.SetVariable("guardState", GuardState.Patrol);
        tree = new EntryNode().SetBlackboard(blackboard).SetExecutor(this)
            .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state == GuardState.Patrol)
                .Append(new SequenceNode()
                    .Append(new BlackboardOperation<AIPath>("Path",BlackboardOperationType.ReadReference))
                    .Append(new AISetPath())
                    .Append(new AIIndexToBlackboard("guardIndex"))
                    .Append(new AIGoalToBlackboard("guardGoal"))
                    .Append(new ConditionalNode(() => blackboard.TryGetReference("View", out Transform view) && Physics.SphereCast(view.position,1f,view.forward,out RaycastHit hit,chaseRange - 1.1f) 
                            && hit.collider.CompareTag("Player"))
                        .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.WeaponSeach))
                        .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardIndex", out int index) && blackboard.TryGetVariable("guardGoal", out int goal) && index == goal)
                            .Append(new AINextPoint())
                            .Append(new AINavigateToPoint())
                        )
                    )
                )
                .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state == GuardState.WeaponSeach)
                    .Append(new SequenceNode()
                        .Append(new ConditionalNode(() => blackboard.TryGetReference("Gun", out GameObject gun) && Vector3.Distance(transform.position,gun.transform.position) < 0.5f)
                            .Append(new SequenceNode()
                                .Append(new DebugLogNode("GunPickup"))
                                .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Chase))
                            )
                            .Append(new SequenceNode()
                                .Append(new BlackboardOperation<GameObject>("Gun", BlackboardOperationType.ReadReference))
                                .Append(new AINavigatePosition(true))
                            )
                        )
                    )
                    .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state == GuardState.Chase)
                        .Append(new ConditionalNode(() => blackboard.TryGetReference("Player", out GameObject player) && Vector3.Distance(transform.position,player.transform.position) > chaseRange)
                            .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Patrol))
                            .Append(new ConditionalNode(() => blackboard.TryGetReference("Player", out GameObject player) && Vector3.Distance(transform.position,player.transform.position) < shootRange)
                                .Append(new SequenceNode()
                                    .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Shoot))
                                    .Append(new AIStopNode())
                                    .Append(new DebugLogNode("Start Shooting!"))
                                )
                                .Append(new SequenceNode() //Chase him
                                    .Append(new BlackboardOperation<GameObject>("Player", BlackboardOperationType.ReadReference))
                                    .Append(new AINavigatePosition(true)))
                            )
                        )
                        .Append(new ConditionalNode(() => blackboard.TryGetVariable("guardState", out GuardState state) && state == GuardState.Shoot)
                            .Append(new ConditionalNode(() => blackboard.TryGetReference("Player", out GameObject player) && Vector3.Distance(transform.position,player.transform.position) > shootRange + 0.5f)
                                .Append(new BlackboardOperation<GuardState>("guardState", BlackboardOperationType.WriteVariable, GuardState.Chase))
                                .Append(new DebugLogNode("SHOOTING CODE"))
                            )
                            .Append(new DebugLogNode("Invalid State!"))
                            )
                    )
                )
            ).Build();
        Debug.Log(tree.toPlantUML());
    }

    private void FixedUpdate()
    {
        currentTreeStatus = tree?.RunNode() ?? TaskStatus.Failed;
        blackboard.TryGetVariable("guardState", out GuardState stat);
        state = stat;   
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        Gizmos.color = Color.white;
    }
}
