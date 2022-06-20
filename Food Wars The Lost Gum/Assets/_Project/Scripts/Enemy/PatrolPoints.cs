using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{
    public Transform patrolPointSet;
    public int currTargetIndex = 0;
    public List<Transform> patrolPoints = new List<Transform>();


    private void Start()
    {
        foreach(Transform t in patrolPointSet)
        {
            patrolPoints.Add(t);
        }
    }


    // update currTargetIndex, if index exceeds list length, go back to 0
    public void updateIndex()
    {
        currTargetIndex++;
        if (currTargetIndex > patrolPoints.Count - 1)
        {
            currTargetIndex = 0;
        }
    }
}
