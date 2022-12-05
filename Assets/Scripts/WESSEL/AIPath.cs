using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPath : MonoBehaviour
{
    public bool drawGizmo = false;
    [HideInInspector] public List<Transform> waypoints = new List<Transform>();
    void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
            waypoints.Add(transform.GetChild(i));
    }

    private void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            if (!Application.isPlaying)
            {
                for (int i = 1; i < transform.childCount; i++)
                {
                    Debug.DrawLine(transform.GetChild(i-1).position, transform.GetChild(i).position, Color.green);
                }
                Debug.DrawLine(transform.GetChild(0).position, transform.GetChild(transform.childCount - 1).position, Color.green);
            }
            else
            {
                for (var i = 1; i < waypoints.Count; i++)
                {
                    Debug.DrawLine(waypoints[i - 1].position, waypoints[i].position, Color.green);
                }
                Debug.DrawLine(waypoints[0].position, waypoints[waypoints.Count - 1].position, Color.green);
            }
        }
    }
}
