using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EscapeController : NetworkBehaviour
{

    public float escapeDistance = 5.0f;

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (GetComponent<PlayerController>().amHitman)
        {
            return;
        }

        //For each escape postiion
        GameObject[] escapes = GameObject.FindGameObjectsWithTag("Escape");
        for (int i = 0; i < escapes.Length; i++)
        {
            //If we are close enough to escape we escape
            if (Vector3.Distance(transform.position, escapes[i].transform.position) < escapeDistance)
            {
                GetComponent<PlayerController>().CmdEscape();
            }
        }


    }
}
