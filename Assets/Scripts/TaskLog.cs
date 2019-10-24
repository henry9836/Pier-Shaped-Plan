using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TaskLog : NetworkBehaviour
{
    public int numberoftasks = 3;

    public enum TASKS
    {
        EATFISH,
        GREETFISHERMAN,
        USEPAYPHONE,
        BUYNEWSPAPER,
        TIEUPBOAT,
        DROPTHEPACKAGE,
    };


    [System.Serializable]
    public struct TaskState
    {
        public TASKS id; //enum
        public bool completed;
        public TaskState(TASKS id, bool completed)
        {
            this.id = id;
            this.completed = completed;
        }
    }
    public class TaskStatesClass : SyncListStruct<TaskState> { }
    TaskStatesClass TaskStates = new TaskStatesClass();


    public List<int> ExcludedTasks = new List<int>();

    //When we complete a task
    [Command]
    public void CmdCompletedTask(TASKS _id)
    {
        for (int i = 0; i < TaskStates.Count; i++)
        {
            if (TaskStates[i].id == _id)
            {
                TaskStates.RemoveAt(i);
                TaskStates.Add(new TaskState(_id, true));
            }
        }
        Debug.Log("Changed a value");
        string log = "SERVER Values: ";
        for (int i = 0; i < TaskStates.Count; i++)
        {
            log += " " + TaskStates[i].id + ":" + TaskStates[i].completed + " | ";
        }
        Debug.LogError(log);

        //Update the gamemanger since we are server here
        GameObject.Find("GameManager").GetComponent<GameManager>().completedTasks++;
        
        RpcEcho();
    }

    [ClientRpc]
    public void RpcEcho()
    {
        string log = "CLIENT Values: ";
        for (int i = 0; i < TaskStates.Count; i++)
        {
            log += " " + TaskStates[i].id + ":" + TaskStates[i].completed + " | ";
        }
        Debug.LogError(log);
    }


    void Start()
    {
        if (!isServer)
        {
            return;
        }

        for (int i = 0; i < System.Enum.GetValues(typeof(TASKS)).Length - numberoftasks; i++)
        {
            bool added = true;
            while (added == true)
            {
                int rand = Random.Range(0, System.Enum.GetValues(typeof(TASKS)).Length - 1);
                for (int j = 0; j < ExcludedTasks.Count; j++)
                {
                    if (rand == ExcludedTasks[j])
                    {
                        added = false;
                    }
                }

                if (added == true)
                {
                    ExcludedTasks.Add(rand);
                    added = false;
                }
                else
                {
                    added = true;
                }

            }

        }

        for (int i = 0; i < System.Enum.GetValues(typeof(TASKS)).Length; i++)
        {
            bool Exculded = false;
            for (int j = 0; j < ExcludedTasks.Count; j++)
            {
                if (ExcludedTasks[j] == i)
                {
                    Exculded = true;
                }
            }
            if (Exculded == false)
            {
                TaskStates.Add(new TaskState((TASKS)i, false));
            }
        }

    }


}

