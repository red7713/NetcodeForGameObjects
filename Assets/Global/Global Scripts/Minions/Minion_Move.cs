using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Minion_Move : MonoBehaviour
{
    //public GameObject goal; // this is supposed to be the end goal, but don't know yet how to get there

    public Transform Target;
    public float UpdateRate = 0.1f; //how frequently to recalculate path based on Target transform's position

    private NavMeshAgent Agent;

    private Coroutine FollowCoroutine;

    

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();        
    }

    public void StartChasing()
    {
        if(FollowCoroutine == null)
        {
            StartCoroutine(FollowTarget());
        }
        
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(UpdateRate);

        while (enabled)
        {
            Agent.SetDestination(Target.transform.position);
            yield return Wait;
        }

    }
}
