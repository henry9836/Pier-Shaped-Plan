using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class AIController : NetworkBehaviour
{
    public float arriveRange = 5;
    public float gunDiscoverRange = 20;
    public bool panic;
    public bool arrived;
    public bool waitLock;
    public Vector2 fleeDistance = new Vector2(25, 50);
    private Vector3 targetPos;
    private List<Vector3> wanderPoints = new List<Vector3>();

    private bool warped;
    private NavMeshAgent agent;

    void Flee(GameObject hitmanPos)
    {
        //Go away from hitman
        Vector3 fleeDir = transform.position - hitmanPos.transform.position;

        fleeDir = Vector3.Normalize(fleeDir);

        //Go to oppsosite Direction of hitman
        targetPos = transform.position + (fleeDir * Random.Range(fleeDistance.x, fleeDistance.y));
    }

    void Wander()
    {
        //Wander
        targetPos = wanderPoints[Random.Range(0, wanderPoints.Count - 1)];
    }

    void CheckForWeapon()
    {
        //Look if hitman has weapon near us out
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++)
        {
            //if the hitman has the gun out
            if (players[i].GetComponent<PlayerController>().amHitman && players[i].GetComponent<PlayerController>().gunOut)
            {
                //if we are within range of hitman
                if (Vector3.Distance(transform.position, players[i].transform.position) < gunDiscoverRange)
                {
                    //Flee
                    Flee(players[i]);
                }
            }
        }
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

        //If there is no weapon
        if (!panic)
        {
            //If we have arrived at our destination
            if (Vector3.Distance(transform.position, targetPos) < arriveRange)
            {
                //Start the timer
                if (!arrived && !waitLock)
                {
                    //stop agent
                    targetPos = transform.position;
                    warped = agent.SetDestination(targetPos);

                    //Start interact animation

                    StartCoroutine(WaitAtPoint());
                }
                
                //if our wait is over go somewhere else
                if (!waitLock && arrived)
                {
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
