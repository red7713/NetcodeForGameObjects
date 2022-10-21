using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Player_Move : MonoBehaviour
{
    [SerializeField] private Camera Camera;
    private NavMeshAgent Agent;

    [SerializeField] KeyCode KeyCode;

    private RaycastHit[] Hits = new RaycastHit[1];

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();   
    }

   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode))
        {
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            if(Physics.RaycastNonAlloc(ray, Hits) > 0)
            {
                Agent.SetDestination(Hits[0].point);
            }
        }
    }
}
