using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour
{

    public float speed = 100.0f;
    public float maxSpeed = 10.0f;
    private float tmpSpeed = 0.0f;
    public int health = 1;
    public float fireForce = 500;
    public float panicSpeedMultiplier = 2.0f;

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
    private bool modelLoaded = false;
    [SyncVar]
    public bool gameStarted;
    [SyncVar]
    public bool canEscape = false;
    [SyncVar]
    public bool escaped = false;
    [SyncVar]
    public bool gameOverState = false;
    [SyncVar]
    public bool gunOut = false;
    [SyncVar]
    public int PNESid = 0;

    [Command]
    void CmdModelLoad()
    {
        //offset by one to skip over gun
        int model = 1;
        if (amHitman)
        {
            model = (GetComponent<Decoder>().Decode(TheGrandExchange.NODEID.PLAYERMODELS, PNESid) + 1);
        }
        else
        {
            model = (GetComponent<Decoder>().Decode(TheGrandExchange.NODEID.PLAYERMODELS, PNESid) + 1);
        }
        transform.GetChild(model).gameObject.SetActive(true);
        modelLoaded = true;

        //Assign Animator
        //GetComponent<PNESAnimator>().animator = transform.GetChild(model).transform.GetChild(4).gameObject.GetComponent<Animator>();
        GetComponent<PNESAnimator>().animator = GetComponent<Animator>();
        GetComponent<Animator>().runtimeAnimatorController = transform.GetChild(model).transform.GetChild(4).gameObject.GetComponent<Animator>().runtimeAnimatorController;
        GetComponent<Animator>().avatar = transform.GetChild(model).transform.GetChild(4).gameObject.GetComponent<Animator>().avatar;

        RpcModelLoad(model);
    }

    [ClientRpc]
    void RpcModelLoad(int model)
    {
        transform.GetChild(model).gameObject.SetActive(true);
        modelLoaded = true;

        //Assign Animator
        GetComponent<PNESAnimator>().animator = transform.GetChild(model).transform.GetChild(4).gameObject.GetComponent<Animator>();
        GetComponent<Animator>().runtimeAnimatorController = transform.GetChild(model).transform.GetChild(4).gameObject.GetComponent<Animator>().runtimeAnimatorController;
        GetComponent<Animator>().avatar = transform.GetChild(model).transform.GetChild(4).gameObject.GetComponent<Animator>().avatar;
    }

    [Command]
    void CmdGunOut()
    {
        gunOut = true;
        RpcUpdateGunState();
    }

    [Command]
    void CmdGunHide()
    {
        gunOut = false;
        RpcUpdateGunState();
    }

    [ClientRpc]
    void RpcUpdateGunState()
    {
        //Find the hitman with gun

        GameObject hitmanRefer;

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().amHitman)
            {
                hitmanRefer = GameObject.FindGameObjectsWithTag("Player")[i];
                //Update meshrender depending if is out
                GameObject.FindGameObjectWithTag("Gun").GetComponent<MeshRenderer>().enabled = hitmanRefer.GetComponent<PlayerController>().gunOut;
            }
        }
    }

    [Command]
    public void CmdFireBullet()
    {
        GameObject gunReference = GameObject.FindGameObjectWithTag("Gun");
        GameObject tmpBullet = Instantiate(bullet, gunReference.transform.position + (gunReference.transform.forward), Quaternion.identity);
        tmpBullet.GetComponent<Rigidbody>().AddForce(gunReference.transform.forward * fireForce);
        NetworkServer.Spawn(tmpBullet);
    }

    [Command]
    public void CmdEscape()
    {
        escaped = true;
    }

    [Command]
    public void CmdCompletedTask(TheGrandExchange.TASKIDS taskID)
    {
        if (!isServer)
        {
            return;
        }

        GameObject.Find("GameManager").GetComponent<TaskLog>().CmdCompletedTask(taskID);

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
            playerCanvasReference.transform.Find("Blinder").gameObject.SetActive(true);
        }
        playerCanvasReference.transform.Find("Blinder").transform.GetChild(0).gameObject.GetComponent<Text>().text = "WAITING FOR PLAYERS\n" + playerCount + " CONNECTED";
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

        playerCanvasReference.transform.Find("Blinder").gameObject.SetActive(false);
        gameStarted = true;
    }
  
    private void Start()
    {

        if (!isLocalPlayer)
        {
            return;
        }
        modelLoaded = false;
        gunReference = transform.GetChild(0).transform.GetChild(0).gameObject;
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
            playerCanvasReference.transform.Find("Blinder").gameObject.SetActive(true);
        }

        tmpSpeed = maxSpeed;
    }

    

    //Player was hit by bullet
    public void HitByBullet() 
    {
        CmdHitByBullet();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Spawn gun if hitman
        if (amHitman && gunReference.tag != "Gun")
        {
            gunReference.tag = "Gun";
            //gunReference.GetComponent<MeshRenderer>().enabled = true;
        }

        if (gameStarted)
        {
            //Model loading
            CmdModelLoad();

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

            //Adjust maxspeed
            maxSpeed = tmpSpeed;
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                maxSpeed *= panicSpeedMultiplier;
            }

            //move in direction of input and not dead
            if (moveDir != Vector3.zero && (health > 0))
            {
                transform.LookAt(new Vector3(transform.position.x + moveDir.x, transform.position.y, transform.position.z + moveDir.z));

                if (GetComponent<Rigidbody>().velocity.magnitude < maxSpeed)
                {
                    GetComponent<Rigidbody>().AddForce(transform.forward * speed * Time.deltaTime);
                }

                //Walk animation
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Debug.Log("WE ARE SHIFTING INTO OPVERDRIVE!");
                    GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.PLAYERANIMATORWALK, PNESid, 0);
                    GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.AIANIMATORPANIC, PNESid, 1);
                }
                else
                {
                    GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.AIANIMATORPANIC, PNESid, 0);
                    GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.PLAYERANIMATORWALK, PNESid, 1);
                }
            }
            else
            {
                //Idle animation
                //GetComponent<Animator>().SetBool("Walk", false);
                GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.AIANIMATORPANIC, PNESid, 0);
                GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.PLAYERANIMATORWALK, PNESid, 0);
            }

            //Checking our state
            if (health <= 0)
            {
                //Death animation
                //GetComponent<Animator>().SetTrigger("Death");
                GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.AIANIMATORDEATH, PNESid, 1);
                CmdAmGameOverState();
            }
            else if (escaped)
            {
                CmdAmGameOverState();
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

            //Gun mechanic
            if (amHitman)
            {
                //Holding right click
                if (Input.GetMouseButton(1))
                {
                    gunReference.GetComponent<MeshRenderer>().enabled = true;
                    GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.PLAYERANIMATORGUN, PNESid, 1);
                    CmdGunOut();
                }
                //We are not holding right click
                else
                {
                    gunReference.GetComponent<MeshRenderer>().enabled = false;
                    GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.PLAYERANIMATORGUN, PNESid, 0);
                    CmdGunHide();
                }
            }
        }

        else
        {
            //Check on UI state because we might of done it out of order
            if (!playerCanvasReference.transform.Find("Blinder").gameObject.activeInHierarchy)
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
