using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class AIController : NetworkBehaviour
{
    public float arriveRange = 5;
    public bool panic;
    public bool arrived;
    public bool waitLock;
    private Vector3 targetPos;
    private List<Vector3> wanderPoints = new List<Vector3>();

    private bool warped;
    private NavMeshAgent agent;

    void Flee()
    {
        //Flee
    }

    void Wander()
    {
        //Wander
        targetPos = wanderPoints[Random.Range(0, wanderPoints.Count - 1)];
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
        waitLock = false;
        arrived = false;
        warped = true;
        targetPos = Vector3.zero;

        agent = GetComponent<NavMeshAgent>();

        GameObject[] pts = GameObject.FindGameObjectsWithTag("AINAVNODE");
        for (int i = 0; i < pts.Length; i++)
        {
            wanderPoints.Add(pts[i].transform.position);
        }

        //Go to a point
        Wander();
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }

        //Fix AI
        if (!warped)
        {
            Debug.Log("Warp was called");
            warped = agent.Warp(transform.position);
        }

        //Check if there is a weapon nearby
        CheckForWeapon();

        //If we are panicing
        if (panic)
        {
            Flee();
        }
        //If there is no weapon
        else
        {
            //If we have arrived at our destination
            if (Vector3.Distance(transform.position, targetPos) < arriveRange)
            {
                //Start the timer
                if (!arrived && !waitLock)
                {
                    Debug.Log("Told to wait and Lock is " + waitLock + " arrive: " + arrived);
                    //stop agent
                    targetPos = transform.position;
                    warped = agent.SetDestination(targetPos);
                    //wait at point
                    StartCoroutine(WaitAtPoint());
                }
                
                //if our wait is over go somewhere else
                if (!waitLock && arrived)
                {
                    Debug.Log("Told to wander");
                    arrived = false;
                    Wander();
                }
            }
        }

        //Go to target

        warped = agent.SetDestination(targetPos);

    }

    IEnumerator WaitAtPoint()
    {
        arrived = true;
        waitLock = true;
        yield return new WaitForSeconds(Random.Range(3,6));
        waitLock = false;
    }

}
