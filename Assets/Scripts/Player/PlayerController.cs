using System.Collections;
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

    [SyncVar]
    public bool amHitman = false;

    public GameObject camPrefab;
    public GameObject gunPrefab;
    public GameObject playerCanvas;
    public GameObject bullet;

    private Transform moveReference;
    private GameObject playerCanvasReference;
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
        GameObject tmpBullet = Instantiate(bullet, transform.position + (transform.forward * 2), Quaternion.identity);
        tmpBullet.GetComponent<Rigidbody>().AddForce(transform.forward * fireForce);
        NetworkServer.Spawn(tmpBullet);
    }

    [Command]
    void CmdGunSpawn()
    {
        //Sanity Check
        if (GameObject.Find("Gun(Clone)") == null)
        {
            GameObject gunRefer = Instantiate(gunPrefab, Vector3.zero, Quaternion.identity);
            gunRefer.transform.parent = transform.GetChild(1).transform;
            gunRefer.transform.localPosition = Vector3.zero;
            NetworkServer.Spawn(gunRefer);
        }
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
        if (isLocalPlayer)
        {
            CmdHitByBullet();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Spawn gun if hitman
        if (amHitman && GameObject.Find("Gun(Clone)") == null)
        {
            CmdGunSpawn();
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
            }

            // Draw line in player look direction
            Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
            Debug.DrawLine(transform.position, transform.position + transform.forward * 1.5f, Color.white, Time.deltaTime);

            //Shooting
            if (Input.GetMouseButtonDown(0))
            {
                CmdFireBullet();
            }

            //Checking our state
            if (health <= 0)
            {
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
