﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour
{

    public float speed = 100.0f;
    public float maxSpeed = 10.0f;
    public int health = 1;
    public float fireForce = 500;

    public bool tryingToInteract = false;

    [SyncVar]
    public bool amHitman = false;

    public GameObject camPrefab;
    public GameObject gunPrefab;
    public GameObject playerCanvas;
    public GameObject bullet;

    private Transform moveReference;
    private GameObject playerCanvasReference;
    private GameObject gunReference;
    [SyncVar]
    private bool gameStarted;
    [SyncVar]
    public bool canEscape = false;
    [SyncVar]
    public bool escaped = false;
    [SyncVar]
    public bool gameOverState = false;
    [Command]
    public void CmdFireBullet()
    {
        GameObject gunReference = GameObject.FindGameObjectWithTag("Gun");
        GameObject tmpBullet = Instantiate(bullet, gunReference.transform.position + (gunReference.transform.forward), Quaternion.identity);
        tmpBullet.GetComponent<Rigidbody>().AddForce(gunReference.transform.forward * fireForce);
        NetworkServer.Spawn(tmpBullet);
    }

    [Command]
    void CmdHitByBullet()
    {
        health -= 1; //SyncVar
    }

    [Command]
    void CmdAmGameOverState()
    {
        gameOverState = true; //SyncVar
    }

    [ClientRpc]
    public void RpcUpdatePlayerNumUI(int playerCount)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (playerCanvasReference == null)
        {
            playerCanvasReference = Instantiate(playerCanvas, Vector3.zero, Quaternion.identity);
            playerCanvasReference.transform.GetChild(0).gameObject.SetActive(true);
        }
        playerCanvasReference.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Text>().text = "WAITING FOR PLAYERS\n" + playerCount + " CONNECTED";
    }

    [ClientRpc]
    public void RpcUnblind()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (playerCanvasReference == null)
        {
            playerCanvasReference = Instantiate(playerCanvas, Vector3.zero, Quaternion.identity);
        }

        playerCanvasReference.transform.GetChild(0).gameObject.SetActive(false);
        gameStarted = true;
    }
  
    private void Start()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        gunReference = transform.GetChild(1).transform.GetChild(0).gameObject;
        gunReference.GetComponent<MeshRenderer>().enabled = false;

        gameStarted = false;
        canEscape = false;
        //Spawn camera
        GameObject camRefer = Instantiate(camPrefab, transform.position, Quaternion.identity);
        camRefer.GetComponent<PlayerCamera>().cameraTarget = this.transform;
        camRefer.transform.localPosition = Vector3.zero;
        camRefer.transform.localRotation = Quaternion.identity;
        moveReference = camRefer.transform;

        //Spawn Canvas
        if (playerCanvasReference == null)
        {
            playerCanvasReference = Instantiate(playerCanvas, Vector3.zero, Quaternion.identity);
            //Blind User until game starts
            playerCanvasReference.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    

    //Player was hit by bullet
    public void HitByBullet() 
    {
        //if (isLocalPlayer)
        //{
        //    CmdHitByBullet();
        //}
        CmdHitByBullet();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Debug.LogError("GOT VAL: " + GetComponent<Decoder>().Decode(TheGrandExchange.NODEID.TASKLOG, 0));
        Debug.LogError("GOT VAL: " + GetComponent<Decoder>().Decode(TheGrandExchange.NODEID.TASKLOG, 1));
        Debug.LogError("GOT VAL: " + GetComponent<Decoder>().Decode(TheGrandExchange.NODEID.TASKLOG, 2));
        //Spawn gun if hitman
        if (amHitman && gunReference.tag != "Gun")
        {
            gunReference.tag = "Gun";
            gunReference.GetComponent<MeshRenderer>().enabled = true;
        }

        if (gameStarted)
        {
            //Movement

            Vector3 moveDir = Vector3.zero;
            Vector3 moveCamRelative = moveReference.transform.rotation.eulerAngles;
            
            //Grab input
            if (Input.GetAxis("Horizontal") != 0)
            {
                moveDir += (moveReference.transform.right * Input.GetAxis("Horizontal"));
            }

            if (Input.GetAxis("Vertical") != 0)
            {
                moveDir += (moveReference.transform.forward * Input.GetAxis("Vertical"));
            }

            //move in direction of input
            if (moveDir != Vector3.zero)
            {
                transform.LookAt(new Vector3(transform.position.x + moveDir.x, transform.position.y, transform.position.z + moveDir.z));

                if (GetComponent<Rigidbody>().velocity.magnitude < maxSpeed)
                {
                    GetComponent<Rigidbody>().AddForce(transform.forward * speed * Time.deltaTime);
                }

                //Walk animation
                GetComponent<Animator>().SetBool("Walk", true);
            }
            else
            {
                //Idle animation
                GetComponent<Animator>().SetBool("Walk", false);
            }

            // Draw line in player look direction
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            Debug.DrawLine(transform.position, transform.position + transform.forward * 1.5f, Color.white, Time.deltaTime);

            //Shooting
            //if (Input.GetMouseButtonDown(0))
            //{
            //    CmdFireBullet();
            //}

            //Checking our state
            if (health <= 0)
            {
                //Death animation
                GetComponent<Animator>().SetTrigger("Death");
                CmdAmGameOverState();
            }
            else if (escaped)
            {
                CmdAmGameOverState();
            }

            //Debugging Keys
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<TaskLog>().CmdCompletedTask(TaskLog.TASKS.BUYNEWSPAPER);
            }

            //interacting 
            if (Input.GetKey("e"))
            {
                tryingToInteract = true;
            }
            else
            {
                tryingToInteract = false;
            }

  
        }

        else
        {
            //Check on UI state because we might of done it out of order
            if (!playerCanvasReference.transform.GetChild(0).gameObject.activeInHierarchy)
            {
                //we did go out of order
                gameStarted = true;
            }
            else
            {
                gameStarted = false;
            }
        }


    }



}
