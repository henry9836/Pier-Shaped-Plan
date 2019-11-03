using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class NetworkTest : NetworkBehaviour
{
    [Command]
    public void CmdPing()
    {
        Debug.Log("Client Pinged the server");
    }

    [ClientRpc]
    public void RpcPing()
    {
        Debug.Log("Server Ping the client");
    }
}
