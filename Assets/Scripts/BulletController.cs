using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletController : NetworkBehaviour
{

    //Destory the object
    [Command]
    void CmdDestroyMe()
    {
        Debug.Log("Bullet hit a thing bye bye");
        NetworkServer.Destroy(gameObject);
    }

    //When we hit something
    private void OnTriggerEnter(Collider other)
    {
        //If it is a player
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().HitByBullet();
            CmdDestroyMe();
        }
        //If it is a other
        else
        {
            CmdDestroyMe();
        }
    }
}
