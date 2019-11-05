using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Interaction : NetworkBehaviour
{
    public float timeToComplete = 5.0f;
    public float currentCompletion = 0.0f;

    public bool clientDone = false;

    //public int ammountOfSkillchecks = 1;
    public float maxDistance = 5.0f;

    public int interactorable = 0;
    //public List<int> skillchecks = new List<int>();

    public bool once = false;




    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Vector3 playerpos = this.transform.position;

        interactorable = -9999;

        for (int i = 0; i < System.Enum.GetValues(typeof(TheGrandExchange.TASKIDS)).Length; i++)
        {
            //Check if close enough to a interactable spot
            if (Vector3.Distance(playerpos, TheGrandExchange.taskWorldPositions[i]) < maxDistance)
            {
                //Check that we are allowed to do this one
                bool allowedToComplete = false;

                //Find amount of tasks avaible
                int tasksInWorld = 0;
                GameObject[] nodes = GameObject.FindGameObjectsWithTag("SERVERINFONODE"); 
                for (int j = 0; j < nodes.Length; j++)
                {
                    if ((int)nodes[i].transform.position.x == (int)TheGrandExchange.NODEID.TASKLOG)
                    {
                        tasksInWorld++;
                    }
                }

                //if each task avaible does it match our task?
                for (int j = 0; j < tasksInWorld; j++)
                {
                    //do stuff
                }

                if (allowedToComplete) {
                    if (this.transform.gameObject.GetComponent<PlayerController>().tryingToInteract == true)
                    {
                        if (this.transform.gameObject.GetComponent<PlayerController>().amHitman == false)
                        {
                            interactorable = i;
                        }
                    }
                }
            }
        }




        if (interactorable != -9999)
        {
            if (once == false)
            {
                once = true;
                //set up skillcheck times
            }

            currentCompletion += Time.deltaTime;

            if (currentCompletion >= timeToComplete)
            {
                currentCompletion = 5.0f;
            }
            //if (skillcheck timer appears do skillcheck)
        }
        else
        {
            currentCompletion = 0.0f;
            once = false;
        }

        if (currentCompletion >= timeToComplete)
        {
            //true communications
            TheGrandExchange.TASKIDS theTask = TheGrandExchange.TASKIDS.BUYNEWSPAPER;

            for (int i = 0; i < System.Enum.GetValues(typeof(TheGrandExchange.TASKIDS)).Length; i++)
            {
                if (interactables[interactorable].gameObject.layer == LayerMask.NameToLayer(((TheGrandExchange.TASKIDS)i).ToString()))
                {
                    theTask = (TheGrandExchange.TASKIDS)i;
                }
            }

            GetComponent<PlayerController>().CmdCompletedTask(theTask);
        }
    }
}
