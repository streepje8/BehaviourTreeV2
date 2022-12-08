using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Cover : MonoBehaviour
{
    private BoxCollider col;
    private void Start()
    {
        col = GetComponent<BoxCollider>();
    }

    public Vector3 GetCoverPointFor(Vector3 watcherPos, float distanceFromWall)
    {
        Vector3 wallPos = transform.position;
        Vector3 direction = (wallPos - watcherPos).normalized;
        float angle = Vector3.Angle(-direction, transform.forward);
        return wallPos + direction * (col.bounds.size.x * (angle/90f) + distanceFromWall);
    }
}
