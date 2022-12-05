using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    public WBlackboard blackboard;
    public TaskStatus currentTreeStatus;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        tree = new EntryNode().SetBlackboard(blackboard).SetExecutor(this)
            .Append(new SequenceNode()
                .Append(new BlackboardOperation<AIPath>("Path",BlackboardOperationType.ReadReference))
                .Append(new AISetPath())
                .Append(new AIIndexToBlackboard("guardIndex"))
                .Append(new AIGoalToBlackboard("guardGoal"))
                .Append(new ConditionalNode(() =>
                    {
                        if (blackboard.TryGetVariable("guardIndex", out int index) &&
                            blackboard.TryGetVariable("guardGoal", out int goal))
                        {
                            return index == goal;
                        }
                        return false;
                    })
                    .Append(new AINextPoint())
                    .Append(new AINavigateToPoint())
            )).Build();
    }

    private void FixedUpdate()
    {
        currentTreeStatus = tree?.RunNode() ?? TaskStatus.Failed;
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
