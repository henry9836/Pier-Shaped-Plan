using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerController : NetworkBehaviour
{

    public float speed = 100.0f;
    public float maxSpeed = 10.0f;
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

    }
}
