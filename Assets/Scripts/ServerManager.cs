using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : NetworkBehaviour
{

    public GameObject GameManager;

    // Start is called before the first frame update
    void Start()
    {
        if (!isServer)
        {
            return;
        }

        GameObject gameRefer = Instantiate(GameManager, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(gameRefer);

    }
}
