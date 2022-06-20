using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AI.AIAgent))]
public class Patrol : MonoBehaviour
{
    private AI.AIAgent agent;   // reference to AI agent
    public Transform patrolPointSet;
    private List<Transform> patrolPoints = new List<Transform>();  // list that stores all the patrol points
    private float arriveRadius = 0.2f;  // when in vincinity, we consider this as arrived at the point
    private int currTargetIndex = 0;    // the index that indicates which patrol point it is currently going after

    void Start()
    {
        agent = GetComponent<AI.AIAgent>();


        foreach (Transform t in patrolPointSet)
        {
            patrolPoints.Add(t);
        }

        if (patrolPoints.Count > 0)
        {
            agent.trackedTarget = patrolPoints[currTargetIndex];
            agent.trackedEnemy = patrolPoints[currTargetIndex].gameObject;
        }



    }

    // Update is called once per frame
    void Update()
    {
        if(patrolPoints.Count == 0)
        {
            return;
        }
        
        // arrive at each patrol points one by one. 
        // if one is reached, update the index to go to the next patrol point
        float distance = Vector3.Distance(transform.position, patrolPoints[currTargetIndex].position);
        if(distance <= arriveRadius)
        {
            updateIndex();
            agent.trackedTarget = patrolPoints[currTargetIndex];
            agent.trackedEnemy = patrolPoints[currTargetIndex].gameObject;

        }

    }

    // update currTargetIndex, if index exceeds list length, go back to 0
    private void updateIndex()
    {
        currTargetIndex++;
        if (currTargetIndex > patrolPoints.Count - 1)
        {
            currTargetIndex = 0;
        }
    }
}
