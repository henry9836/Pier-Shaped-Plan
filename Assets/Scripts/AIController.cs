using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AIController : NetworkBehaviour
{

    public bool panic;
    Vector3 targetPos;


    void Flee()
    {
        //Flee
    }

    void CheckForWeapon()
    {
        //Look if hitman has weapon near us out
    }

    void Start()
    {
        panic = false;
        targetPos = Vector3.zero;
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (!panic)
        {
            //Find a point
        }


    }
}
