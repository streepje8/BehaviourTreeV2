using System.Collections.Generic;
using UnityEngine;

public class CoverCollection : MonoBehaviour
{
    public List<Cover> covers = new List<Cover>();

    public Cover GetClosestCover(GameObject target)
    {
        Cover closest = null;
        float distance = Mathf.Infinity;
        covers.ForEach(x =>
        {
            float dst = Vector3.Distance(target.transform.position, x.transform.position);
            if (dst < distance)
            {
                closest = x;
                distance = dst;
            }
        });
        return closest;
    }
}
