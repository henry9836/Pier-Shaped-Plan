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


    public bool onceSkill = false;
    public bool onceGen = false;
    public float tick = 0.0f;
    public float timer = 0.0f;
    public float skillStartTime = 0.0f;
    public float skillFinTime = 0.0f;
    public float genTimer = 20.0f;
    public int skillcheckCount = 0;
    public int maxchecks = 5;
    public int minchecks = 1;
    public bool doing = false;


    //call in update while holding button down
    public void gen()
    {
        if (onceGen == false)
        {
            onceGen = true;
            genTimer = 20.0f;
            skillcheckCount = Random.Range(minchecks, (maxchecks + 1));
        }

        if (doing == false)
        {
            doing = true;
            int result = Skillcheck();

            if (result == 0)
            {

            }
            else if (result == 1)
            {

            }
            else if (result == 2)
            {

            }
        }
    }


    public int Skillcheck()
    {
        if (onceSkill == false)
        {
            timer = 0.0f;
            tick = 0.0f;
            skillStartTime = Random.Range(36.666f, 95.0f);
            skillFinTime = skillStartTime + 5.0f;
            onceSkill = true;
        }
        timer += Time.deltaTime;
        tick = timer / 2.0f;

        if (Input.GetKeyDown("Space"))
        {
            if (tick < skillFinTime && tick > skillStartTime)
            {
                onceSkill = false;
                return (1);
            }
            else
            {
                onceSkill = false;
                return (2);
            }
        }


        return (0);
    }


}

//OBS - https://youtu.be/_Wb8hBPDPNo?t=31

//starts - 7.36		0.0	0.0		
//1/4 - 8.155		0.795	0.3975
//3/8 - 8.33		0.97	0.485

//startskill - 8.17	0.81	0.405
//finish skill 8.43 	1.07	0.5358
	

//starts - 3.48		0.0	
//1/4 - 4.075		0.595
//3/8 - 4.16		0.68	

//startskill - 4.11	0.63	
//finish skill 4.21	0.73

//total timer = 2 seconds
//skill check timer = 0.1 seconds

//-> /100
//========================

//trainer - http://mistersyms.com/tools/gitgud/index

//Math.floor((210 * random.range(0, 1)) - 30) + 180
//max =  360 / 3.6 = 100
//min =  150 / 3.6 = 41.666

//==========================

//total timer = 100
//skill check timer = 5

//max =  360 / 3.6 = 95 <-> 100
//min =  150 / 3.6 = 36.666 <-> 41.666

//============
