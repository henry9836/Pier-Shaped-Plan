using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour
{

    public float speed = 100.0f;
    public float maxSpeed = 10.0f;

    [SyncVar]
    public bool amHitman = false;

    public GameObject camPrefab;
    public GameObject playerCanvas;
    public GameObject bullet;

    private Transform moveReference;
    private GameObject playerCanvasReference;
    [SyncVar]
    private bool gameStarted;
    [SyncVar]
    private bool canEscape;
    //public Transform camRoot;

    [Command]
    public void CmdFireBullet()
    {
        GameObject tmpBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        tmpBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 100);
        NetworkServer.Spawn(tmpBullet);
    }

    [ClientRpc]
    public void RpcCanEscape()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        canEscape = true;
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

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Debug.Log("LOG: " + isLocalPlayer + ":" + isServer + ":" + localPlayerAuthority);

        if (gameStarted)
        {
            //Movement

            Vector3 moveDir = Vector3.zero;
            Vector3 moveCamRelative = moveReference.transform.rotation.eulerAngles;

            if (Input.GetAxis("Horizontal") != 0)
            {
                moveDir += (moveReference.transform.right * Input.GetAxis("Horizontal"));
            }

            if (Input.GetAxis("Vertical") != 0)
            {
                moveDir += (moveReference.transform.forward * Input.GetAxis("Vertical"));
            }

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

            //Debugging Keys
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GetComponent<TaskLog>().CmdCompletedTask(TaskLog.TASKS.BUYNEWSPAPER);
            }

            //Shooting
            if (Input.GetMouseButtonDown(0))
            {
                CmdFireBullet();
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
