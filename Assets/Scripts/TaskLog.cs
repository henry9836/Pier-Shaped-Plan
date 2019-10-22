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

    public List<TaskState> TaskStates = new List<TaskState>();
    public List<int> ExcludedTasks = new List<int>();

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
    }

    [Command]
    public void CmdTestPing()
    {
        Debug.Log("Pong");
    }

    void Start()
    {
        if (!isServer)
        {
            return;
        }

        for (int i = 0; i < (System.Enum.GetValues(typeof(TASKS)).Length - 1) - numberoftasks; i++)
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
                }
            }

        }

        for (int i = 0; i < System.Enum.GetValues(typeof(TASKS)).Length - 1; i++)
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

