using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    //This is a server only script

    public GameObject hitmanReference;
    public GameObject AIPrefab;
    public GameObject InteractObject;
    public int lobbyThreshold = 2;
    public int aiToSpawn = 10;
    public bool gameover = false;
    public bool hitmanWin = false;
    public bool survivorWin = false;

    //private these
    public int amountOfTasks = 5;
    public int completedTasks = 0;
    public bool hitmanSelected = false;
    public bool gameStarted = false;
    public bool canEscape = false;

    private bool ending = false;
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
        hitmanSelected = false;
        gameStarted = false;
        canEscape = false;
        ending = false;

        //Debug
        GameObject interactRef = Instantiate(InteractObject, transform.position, Quaternion.identity);
        interactRef.tag = "Respawn";

        NetworkServer.Spawn(interactRef);

    }

    void SelectHitman()
    {
        //Spawn AI
        for (int i = 0; i < aiToSpawn; i++)
        {
            GameObject aiRefer = Instantiate(AIPrefab, new Vector3(20.0f, 2.0f, 5.0f), Quaternion.identity);
            NetworkServer.Spawn(aiRefer);
        }

        //Get a random player
        hitmanReference = GameObject.FindGameObjectsWithTag("Player")[Random.Range(0, GameObject.FindGameObjectsWithTag("Player").Length)];
        //Set random as hitman
        hitmanReference.GetComponent<PlayerController>().amHitman = true; //SyncVar
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
            Debug.LogError("SERVER SCRIPT [GameManager] RUN ON NON SERVER OBJECT!");
            return;
        }
        
        //If we have not selected our hitman and the game has enough players then select the hitman
        if (!hitmanSelected && lobbyThreshold <= GameObject.FindGameObjectsWithTag("Player").Length)
        {
            SelectHitman();
        }

        //Game is running and not gameover
        else if (gameStarted && !gameover)
        {

            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            //Get Completed Tasks
            completedTasks = 0;
            int j = 0;
            GameObject[] nodes = GameObject.FindGameObjectsWithTag("SERVERINFONODE");
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].transform.position.x == (int)TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE)
                {
                    if (GetComponent<Decoder>().DecodeBool(TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE, j))
                    {
                        completedTasks++;
                    }
                    j++;
                }
            }

            //if we have enough tasks to escape
            if ((((float)completedTasks / (float)amountOfTasks) * 100.0f) >= 60.0f)
            {
                canEscape = true;
                //Update Player Escape States
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].GetComponent<PlayerController>().canEscape = true; //SyncVar
                }
            }


            //Hitman Win
            //Check player health

            hitmanWin = true;
            gameover = true;
            for (int i = 0; i < players.Length; i++)
            {
                //are there are non hitman players alive then game is not over
                if (players[i].GetComponent<PlayerController>().health > 0 && !players[i].GetComponent<PlayerController>().amHitman)
                {
                    //Debug.Log("N HITMAN1");
                    hitmanWin = false;
                    gameover = false;
                }
            }

            if (!gameover)
            {

                //Survivor Win
                //Check for escapes
                survivorWin = true;
                gameover = true;
                for (int i = 0; i < players.Length; i++)
                {
                    //has a player not escaped
                    if (!players[i].GetComponent<PlayerController>().escaped && !players[i].GetComponent<PlayerController>().amHitman)
                    {
                        //Debug.Log("N SURVIVOR");
                        survivorWin = false;
                        gameover = false;
                    }
                }
            }

            if (!gameover)
            {
                //Hitman Win 2
                //Check for gameoverStates
                hitmanWin = true;
                gameover = true;
                for (int i = 0; i < players.Length; i++)
                {
                    //has a player that is not in gameoverState
                    if (!players[i].GetComponent<PlayerController>().gameOverState && !players[i].GetComponent<PlayerController>().amHitman)
                    {
                        //Debug.Log("N HITMAN2");
                        hitmanWin = false;
                        gameover = false;
                    }
                }
            }
        }

        else if (gameover)
        {
            if (hitmanWin)
            {
                Debug.Log("Hitman Win!");
            }
            else
            {
                Debug.Log("Survivor Win!");
            }
            if (!ending)
            {
                StartCoroutine(EndGame());
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

    IEnumerator EndGame()
    {
        ending = true;
        yield return new WaitForSeconds(5);
        NetworkServer.DisconnectAll();
        NetworkServer.Shutdown();
        NetworkServer.Reset();
        SceneManager.LoadScene("Menu");
    }

}
