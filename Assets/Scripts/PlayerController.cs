using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour
{

    public float speed = 100.0f;
    public float maxSpeed = 10.0f;

    private Transform moveReference;

    public GameObject camPrefab;
    public GameObject bullet;
    //public Transform camRoot;

    [Command]
    public void CmdPing()
    {
        Debug.Log("SERVER PING");
        RpcPing();
    }

    [Command]
    public void CmdFireBullet()
    {
        GameObject tmpBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        tmpBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 100);
        NetworkServer.Spawn(tmpBullet);
    }

    [ClientRpc]
    public void RpcPing()
    {
        Debug.Log("CLIENT PING");
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Spawn camera
        GameObject camRefer = Instantiate(camPrefab, transform.position, Quaternion.identity);
        //camRefer.transform.parent = camRoot;
        camRefer.GetComponent<PlayerCamera>().cameraTarget = this.transform;
        camRefer.transform.localPosition = Vector3.zero;
        camRefer.transform.localRotation = Quaternion.identity;

        moveReference = camRefer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Debug.Log("LOG: " + isLocalPlayer + ":" + isServer + ":" + localPlayerAuthority);

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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdPing();
        }

        if (Input.GetMouseButtonDown(0)) {
            CmdFireBullet();
        }


    }



}
