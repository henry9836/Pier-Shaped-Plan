using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class AIController : NetworkBehaviour
{

    public bool panic;
    public bool arrived;
    public bool interactLock;
    private Vector3 targetPos;
    private List<Vector3> wanderPoints = new List<Vector3>();

    private bool warped;
    private NavMeshAgent agent;

    void Flee()
    {
        //Flee
    }

    void CheckForWeapon()
    {
        //Look if hitman has weapon near us out
    }

    void Start()
    {
        if (!isServer)
        {
            return;
        }
        panic = false;
        interactLock = false;
        arrived = false;
        warped = true;
        targetPos = Vector3.zero;

        agent = GetComponent<NavMeshAgent>();

        GameObject[] pts = GameObject.FindGameObjectsWithTag("AINAVNODE");
        for (int i = 0; i < pts.Length; i++)
        {
            wanderPoints.Add(pts[i].transform.position);
        }

        //Find a point
        Debug.Log("WE SHOULD GO TO: " + wanderPoints[wanderPoints.Count - 1]);
        targetPos = wanderPoints[wanderPoints.Count-1];

    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (!warped)
        {
            Debug.Log("Warp was called");
            warped = agent.Warp(transform.position);
        }

        //Go to target

        warped = agent.SetDestination(targetPos);

    }
}
