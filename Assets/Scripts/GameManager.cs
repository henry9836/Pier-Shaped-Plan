using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    //This is a server only script

    public GameObject hitmanReference;
    public int lobbyThreshold = 2;
    public bool gameover = false;

    //private these
    public int amountOfTasks = 5;
    public int completedTasks = 0;
    public bool hitmanSelected = false;
    public bool gameStarted = false;
    public bool canEscape = false;

    private void Start()
    {
        if (!isServer)
        {
            Debug.LogError("SERVER SCRIPT [GameManager] RUN ON NON SERVER OBJECT");
            return;
        }
        InitGame();
    }

    void InitGame()
    {
        hitmanSelected = false;
        gameStarted = false;
        gameover = false;
        canEscape = false;
    }

    void SelectHitman()
    {
        //Get a random player
        hitmanReference = GameObject.FindGameObjectsWithTag("Player")[Random.Range(0, GameObject.FindGameObjectsWithTag("Player").Length)];
        //Set random as hitman
        hitmanReference.GetComponent<PlayerController>().RpcSetHitman();
        hitmanSelected = true;

        //Unblind players
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().RpcUnblind();
        }

        gameStarted = true;
    }

    private void FixedUpdate()
    {
        if (!isServer)
        {
            Debug.LogError("SERVER SCRIPT [GameManager] RUN ON NON SERVER OBJECT");
            return;
        }
        
        //If we have not selected our hitman and the game has enough players then select the hitman
        if (!hitmanSelected && lobbyThreshold <= GameObject.FindGameObjectsWithTag("Player").Length)
        {
            SelectHitman();
        }

        //Game is running
        else if (gameStarted)
        {

            Debug.Log((completedTasks / amountOfTasks) * 100);

            //if we have enough tasks to escape
            if (((completedTasks/amountOfTasks) * 100) >= 60)
            {
                canEscape = true;
                //Update Player Escape States
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
                {
                    GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().RpcCanEscape();
                }
            }
        }

        //Game is not ready and has not started
        else if (!gameStarted)
        {
            //Update Player Count
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
            {
                GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().RpcUpdatePlayerNumUI(GameObject.FindGameObjectsWithTag("Player").Length);
            }
        }
    }

}
