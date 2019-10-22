using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TaskLog : NetworkBehaviour
{
    
    void Start()
    {
        if (!isServer)
        {
            return;
        }

    }
}
