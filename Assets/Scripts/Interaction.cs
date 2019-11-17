using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class Interaction : NetworkBehaviour
{
    public float timeToComplete = 5.0f;
    public float currentCompletion = 0.0f;
    public float maxDistance = 5.0f;
    public bool clientDone = false;
    public bool once = false;
    public int interactorable = 0;

    public TheGrandExchange.TASKIDS theTask = TheGrandExchange.TASKIDS.BUYNEWSPAPER;




    public bool onceGen = false;
    public float tick = 0.0f;
    public float timer = 0.0f;
    public float skillStartTime = 0.0f;
    public float skillFinTime = 0.0f;
    public float genTimer = 0.0f;
    public float genTimerMAX = 15.0f;

    public int skillcheckCount = 0;
    public int maxchecks = 5;
    public int minchecks = 1;
    public bool doing = false;
    private int result = 0;

    public List<float> valid = new List<float>();
    public List<float> selected = new List<float>();
    private List<float> selected2 = new List<float>();

    public GameObject UI;
    public Sprite UIimageIndicator;
    public Sprite UIimageCircle;

    private CanvasGroup skillCheckCanvas;
    private GameObject skillCircle;
    private CanvasGroup skillCircleCanvas;
    private GameObject skillIndicator;
    private CanvasGroup skillIndicatorCanvas;
    private CanvasGroup spacePrompt;

    public Image ProgressBar;


    public GameObject point;
    public GameObject pointStart;
    public GameObject PointFin;


    public bool INTR = false;
    public float INTRtimer = 0.0f;
    public float INTRtimerMAX = 0.5f;
    public int interactorable2 = 0;


    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (UI == null)
        {
            UI = GameObject.FindGameObjectWithTag("PlayerCanvas");
            // Main skill check canvas
            skillCheckCanvas = UI.transform.Find("SkillCheck").GetComponent<CanvasGroup>();

            skillCircle = UI.transform.Find("SkillCheck/SkillCircle").gameObject;                   // Skill circle object for rotating
            skillCircleCanvas = skillCircle.GetComponent<CanvasGroup>();                            // Skill circle for fade

            skillIndicator = UI.transform.Find("SkillCheck/SkillIndicator").gameObject;             // Skill indicator for rotating
            skillIndicatorCanvas = skillIndicator.GetComponent<CanvasGroup>();                      // Skill indicator for fade
            spacePrompt = UI.transform.Find("SkillCheck/SpacePrompt").GetComponent<CanvasGroup>();

            skillCircleCanvas.alpha = 0.0f;                                                         // Set circle and indicator alpha to 0
            skillIndicatorCanvas.alpha = 0.0f;
            spacePrompt.alpha = 0.0f;

            ProgressBar = UI.transform.Find("SkillCheck/ProgressBar/ProgressFill").GetComponent<Image>();
        }
       

        if (UIimageIndicator == null)
        {
            UIimageIndicator = GameObject.Find("UISkillchecktemp").GetComponent<Image>().sprite;
        }
        if (UIimageCircle == null)
        {
            UIimageCircle = GameObject.Find("UISkillchecktemp2").GetComponent<Image>().sprite;
        }


        //Attempt to find an interactable object
        Vector3 playerpos = this.transform.position;

        interactorable = -9999;
        bool resetOverride = false;
        for (int i = 0; i < System.Enum.GetValues(typeof(TheGrandExchange.TASKIDS)).Length; i++)
        {
            //Debug.Log("DISTANCE: " + Vector3.Distance(playerpos, TheGrandExchange.taskWorldPositions[i]));
            //Check if close enough to a interactable spot
            if (Vector3.Distance(playerpos, TheGrandExchange.taskWorldPositions[i]) < maxDistance)
            {
                //Debug.Log("DISTANCE: " + Vector3.Distance(playerpos, TheGrandExchange.taskWorldPositions[i]));
                //Check that we are allowed to do this one
                bool allowedToComplete = false;

                //Find amount of tasks avaible
                GameObject[] nodes = GameObject.FindGameObjectsWithTag("SERVERINFONODE");
                List<GameObject> TaskNodes = new List<GameObject>();
                for (int j = 0; j < nodes.Length; j++)
                {
                    if ((int)nodes[j].transform.position.x == (int)TheGrandExchange.NODEID.TASKLOG)
                    {
                        TaskNodes.Add(nodes[j]);
                    }
                }

                //for each task, one avaible does it match our task?
                for (int j = 0; j < TaskNodes.Count; j++)
                {
                    //if the node we are looking at that is close to us is valid
                    // TheGrandExchange.TASKIDS
                    int compareY = ((int)TaskNodes[j].transform.position.y * -1) - 1;
                    int compareNode = (int)(TheGrandExchange.TASKIDS)TheGrandExchange.TASKIDS.ToObject(typeof(TheGrandExchange.TASKIDS), i);
                    if (compareY == compareNode)
                    {
                       // Debug.Log("ALLOWED: " + compareY + ":" + compareNode + ":" + i);
                        allowedToComplete = true;
                    }
                    else
                    {
                        //Debug.Log("NOT ALLOWED: " + compareY + ":" + compareNode + ":" + i);
                    }
                }

                //If allowed to complete flag is true
                if (allowedToComplete)
                {
                    //Do not allow reset to timer
                    resetOverride = true;
                    if (this.transform.gameObject.GetComponent<PlayerController>().amHitman == false)
                    {
                        theTask = (TheGrandExchange.TASKIDS)i;
                        if (this.transform.gameObject.GetComponent<PlayerController>().tryingToInteract == true)
                        {
                            INTRtimer += Time.deltaTime;
                            Debug.Log(INTRtimer + " " + INTRtimerMAX);

                            if (INTRtimer >= INTRtimerMAX)
                            {
                                INTR = true;
                                interactorable = i;
                                interactorable2 = i;
                            }
                        }
                        else if (!INTR)
                        {
                            INTRtimer = 0.0f;
                        }
                    }
                }
            }
            //If we are not close enough to a interactable spot
            else
            {
                //INTR = false;
                //INTRtimer = 0.0f;
                //Endskill();
            }
        }

        if (!resetOverride) {
            INTR = false;
            INTRtimer = 0.0f;
        }

        if (INTR == true)
        {
            interactorable = interactorable2;
        }


        //If we found an interable object

        if (interactorable != -9999)
        {

            bool doing = gen();
            if (doing == true)
            {
                GetComponent<PlayerController>().CmdCompletedTask(theTask);
            }
        }
        else
        {
            onceGen = false;
            End();
        }
    }



    //call in update while holding button down
    public bool gen()
    {
        //initlize
        if (onceGen == false)
        {
            Begin();

            onceGen = true;
            genTimer = 0.0f;
            skillcheckCount = Random.Range(minchecks, (maxchecks));
            selected2.Clear();
            selected.Clear();
            valid.Clear();

            //gets skill check times to do 
            for (int i = 1; i < maxchecks + 1; i++)
            {
                valid.Add((genTimerMAX / maxchecks) * i);
            }

            valid.RemoveAt(valid.Count - 1);

            for (int i = 0; i < skillcheckCount; i++)
            {
                int temp = Random.Range(0, valid.Count);
                selected.Add(valid[temp]);
                valid.RemoveAt(temp);
            }


            //sorting function
            while (selected.Count > 1)
            {
                int lowestsel = 0;
                for (int i = 1; i < selected.Count; i++)
                {
                    if (selected[lowestsel] > selected[i])
                    {
                        lowestsel = i;
                    }
                }
                selected2.Add(selected[lowestsel]);
                selected.RemoveAt(lowestsel);
            }
            selected2.Add(selected[0]);
            selected.RemoveAt(0);

            selected = new List<float>(selected2);
            selected2.Clear();
        }

        genTimer += Time.deltaTime;

        ProgressBar.fillAmount = genTimer / genTimerMAX;

        if (selected.Count > 0)
        {
            //if its skillcheck time
            Debug.Log(genTimer + " " + selected[0] + "timer selected ");
            if (genTimer >= selected[0])
            {
                StartCoroutine(SkillCheck());
                selected.RemoveAt(0);
            }
        }


        //resuilts of a skil check 
        if (result == 0) // doing
        {
        }
        else if (result == 1) //sucessful skillcheck
        {
            Debug.Log("pass");
            doing = false;
            Endskill();
        }
        else if (result == 2) // unsucessful skillcheck
        {
            Debug.Log("fail");
            result = 0;
            onceGen = false;
            doing = false;
            this.transform.gameObject.GetComponent<PlayerController>().tryingToInteract = false;
            Endskill();
        }

        if (genTimer > genTimerMAX) //completed the gen
        {
            End();


            return (true);
        }
        else
        {
            return (false);
        }
    }

    //um oh a wjild skill check appeared
    IEnumerator SkillCheck()
    {

        Debug.Log("---Skillcheck--- ");

        //sets of stuff includeing start and fin a skill check times
        timer = 0.0f;
        tick = 0.0f;
        skillStartTime = Random.Range(36.666f, 85.0f );
        skillFinTime = skillStartTime + 14.0f;

        //UI
        

        Beginskill();
        skillCircle.transform.eulerAngles = new Vector3(0, 0, (skillStartTime + 14.0f) * (360.0f / 100.0f));


        //the time the skill check is valid
        for (timer = 0.0f; timer < 2.0f; timer += Time.deltaTime)
        {
            tick = (timer / 2.0f) * 100.0f;

            skillIndicator.transform.eulerAngles = new Vector3(0, 0, tick * (360.0f / 100.0f));

            //if they thry to hit it 
            if (Input.GetKeyDown("space")) 
            {
                if (tick < skillFinTime && tick > skillStartTime) //win
                {
                    //End();

                    Debug.Log("win skill chick");
                    result = 1;
                    Destroy(point);
                    Destroy(pointStart);
                    Destroy(PointFin);

                    yield break;

                }
                else // else
                {
                   // End();

                    onceGen = false;
                    Debug.Log("missed skill check");
                    result = 2;
                    Destroy(point);
                    Destroy(pointStart);
                    Destroy(PointFin);
                    yield break;

                }
            }
            
            result = 0; //doing
            yield return null;
        }
       // End();

        Debug.Log("skill check ended"); // they missed the time and didnt hit anything return fail
        result = 2;
        Destroy(point);
        Destroy(pointStart);
        Destroy(PointFin);

    }


    private void Begin()
    {
        skillCheckCanvas.alpha = 1.0f;
    }
    private void End()
    {
        skillCheckCanvas.alpha = 0.0f;
    }
    
    private void Beginskill()
    {
        skillCircleCanvas.alpha = 1.0f;
        skillIndicatorCanvas.alpha = 1.0f;
        spacePrompt.alpha = 1.0f;
    }
    private void Endskill()
    {
        skillCircleCanvas.alpha = 0.0f;
        skillIndicatorCanvas.alpha = 0.0f;
        spacePrompt.alpha = 0.0f;
    }
}

//research of how skill checks fucntion
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
