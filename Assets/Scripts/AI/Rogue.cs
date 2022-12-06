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
        tree = new EntryNode().SetBlackboard(blackboard).SetExecutor(this)
            .Append(new SequenceNode()
                .Append(new BlackboardOperation<GameObject>("Player", BlackboardOperationType.ReadReference))
                .Append(new AINavigatePosition(true))
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
