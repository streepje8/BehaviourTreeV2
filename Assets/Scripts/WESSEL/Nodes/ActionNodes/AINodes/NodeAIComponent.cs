using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NodeAIComponent : MonoBehaviour
{
    public NavMeshAgent agent;
    public AIPath currentPath;
    public int goalIndex = 0;
    public int currentIndex = -1;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Navigate()
    {
        if (currentPath == null) Debug.LogError("Trying to navigate to a null variable");
        if (currentIndex != goalIndex)
        {
            if (Vector3.Distance(transform.position,currentPath.waypoints[goalIndex].position) < 0.5f)
            {
                currentIndex = goalIndex;
            }
            agent.SetDestination(currentPath.waypoints[goalIndex].position);
        }
    }

    public void NavigatePosition(Vector3 value)
    {
        agent.SetDestination(value);
    }
}
