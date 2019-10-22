using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour
{

    public float speed = 100.0f;
    public float maxSpeed = 10.0f;

    public GameObject camPrefab;
    public GameObject bullet;
    public Transform camRoot;

    [Command]
    public void CmdPing()
    {
        Debug.Log("SERVER PING");
        RpcPing();
    }

    //[Command]
    //public void CmdFireBullet()
    //{
    //    GameObject tmpBullet = Instantiate(bullet, transform.position, Quaternion.identity);
    //    tmpBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 100);
    //    NetworkServer.Spawn(tmpBullet);
    //}

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
        camRefer.transform.parent = camRoot;
        camRefer.transform.localPosition = Vector3.zero;
        camRefer.transform.localRotation = Quaternion.identity;
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

        if (Input.GetAxis("Horizontal") != 0)
        {
            moveDir += (transform.right * Input.GetAxis("Horizontal"));
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            moveDir += (transform.forward * Input.GetAxis("Vertical"));
        }

        if (GetComponent<Rigidbody>().velocity.magnitude < maxSpeed)
        {
            GetComponent<Rigidbody>().AddForce(moveDir.normalized * speed);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            CmdPing();
        }

        if (Input.GetMouseButtonDown(0)) {
            //CmdFireBullet();
        }


    }



}
