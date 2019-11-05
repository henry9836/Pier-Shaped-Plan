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
            }

            interactor.Clear();
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
