using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public float timeToComplete = 5.0f;
    public float currentCompletion = 0.0f;
    public bool isDone = false;
    public int ammountOfSkillchecks = 1;
    public float maxDistance = 10.0f;

    public List<int> interactor = new List<int>();
    public List<int> skillchecks = new List<int>();

    public bool once = false;

    void Update()
    {
        if (isDone == false)
        {
            Vector3 thisPos = this.transform.position;
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

            for (int i = 0; i < Players.Length; i++)
            {
                if (Vector3.Distance(thisPos, Players[i].transform.position) < maxDistance)
                {
                    if (Players[i].GetComponent<PlayerController>().tryingToInteract == true)
                    {
                        if (Players[i].GetComponent<PlayerController>().amHitman == false)
                        {
                            interactor.Add(i);
                        }
                    }
                }
            }

            if (interactor.Count == 0)
            {
                currentCompletion = 0.0f;
                once = false;
            }
            else
            {
                if (once == false)
                {
                    once = true;
                    //set up skillcheck times
                }
                currentCompletion += Time.deltaTime * interactor.Count;

                //if (skillcheck timer appears do skillcheck)
            }

            if (currentCompletion >= timeToComplete)
            {
                isDone = true;
                Players[0].GetComponent<PlayerController>().CmdCompletedTask(TheGrandExchange.TASKIDS.BUYNEWSPAPER);
            }

            interactor.Clear();
        }

    }
}
