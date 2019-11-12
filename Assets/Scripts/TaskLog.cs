using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TaskLog : NetworkBehaviour
{
    public int numberoftasks = 3;

    private List<TheGrandExchange.TASKIDS> assignedTasks = new List<TheGrandExchange.TASKIDS>();

    //When we complete a task
    [Command]
    public void CmdCompletedTask(TheGrandExchange.TASKIDS id)
    {
        //get the element for the other ID
        int element = 0;
        for (int i = 0; i < assignedTasks.Count; i++)
        {
            //If we found the node with the same task
            if (assignedTasks[i] == id)
            {
                element = i;
            }
        }
        GetComponent<Encoder>().Modify(TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE, (TheGrandExchange.TASKIDS)element, true);
    }

    private void FixedUpdate()
    {
        //When game starts
    }

    void Start()
    {
        if (!isServer)
        {
            return;
        }

        //New Code
        
        //For the number of tasks
        while (assignedTasks.Count < numberoftasks)
        {
            //Pick a task
            int choice = Random.Range(0, System.Enum.GetValues(typeof(TheGrandExchange.TASKIDS)).Length);

            //Have we already assigned this task?
            bool notAssigned = true;

            for (int i = 0; i < assignedTasks.Count; i++)
            {
                if ((int)assignedTasks[i] == choice)
                {
                    notAssigned = false;
                }
            }

            if (notAssigned)
            {
                assignedTasks.Add((TheGrandExchange.TASKIDS)choice);
            }
        }

        //Generate list from assigned Tasks
        for (int i = 0; i < assignedTasks.Count; i++)
        {
            GetComponent<Encoder>().Encode(TheGrandExchange.NODEID.TASKLOG, i, (int)assignedTasks[i]); //set value of task to element
            GetComponent<Encoder>().Encode(TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE, i, 0); //set completed to false
            GameObject Visual = Instantiate(GameObject.Find("GameManager").GetComponent<GameManager>().InteractObject, TheGrandExchange.taskWorldPositions[(int)assignedTasks[i]],Quaternion.identity);
            NetworkServer.Spawn(Visual);
        }

    }


}

