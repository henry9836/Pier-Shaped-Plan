using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    //This is a server only script

    public GameObject hitmanReference;
    public int lobbyThreshold = 2;

    private bool hitmanSelected = false;

    private void Start()
    {
        if (!isServer)
        {
            Debug.LogError("SERVER SCRIPT [GameManager] RUN ON NON SERVER OBJECT");
            return;
        }

        hitmanSelected = false;

    }

    void SelectHitman()
    {
        hitmanReference = GameObject.FindGameObjectsWithTag("Player")[Random.Range(0, GameObject.FindGameObjectsWithTag("Player").Length)];

        hitmanSelected = true;
    }

    private void FixedUpdate()
    {
        if (!isServer)
        {
            Debug.LogError("SERVER SCRIPT [GameManager] RUN ON NON SERVER OBJECT");
            return;
        }
        
        if (!hitmanSelected && lobbyThreshold <= GameObject.FindGameObjectsWithTag("Player").Length)
        {
            SelectHitman();
        }

    }

}
