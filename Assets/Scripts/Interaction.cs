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
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

        interactorable = -9999;

        for (int i = 0; i < interactables.Length; i++)
        {
            if (Vector3.Distance(playerpos, interactables[i].transform.position) < maxDistance)
            {
                if (this.transform.gameObject.GetComponent<PlayerController>().tryingToInteract == true)
                {
                    if (this.transform.gameObject.GetComponent<PlayerController>().amHitman == false)
                    {
                        interactorable = i;
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
