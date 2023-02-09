using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobAI : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;

    void Start()
    {

    }

    void Update()
    {
        agent.SetDestination(target.position);
    }
}