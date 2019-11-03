using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Encoder : NetworkBehaviour
{

    public GameObject nodePrefab;

    public void Encode(TheGrandExchange.NODEID id, int element, int value)
    {

        if (!isServer)
        {
            return;
        }

        Vector3 encodedInfomation = Vector3.zero;

        //Assign ID
        encodedInfomation.x = (int)id;

        //Assign Element

        encodedInfomation.z = element;

        //Assign Value

        encodedInfomation.y = (value + 1) * -1; //+1 to avoid 0s and inverted to not go above level

        GameObject newNode = Instantiate(nodePrefab, encodedInfomation, Quaternion.identity);
        NetworkServer.Spawn(newNode);

    }

    public void Modify(TheGrandExchange.NODEID id, int element, int value)
    {
        Vector3 enocodedInfomation = Vector3.zero;

        //Find node

        GameObject node = null;
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("SERVERINFONODE");

        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].transform.position.x == (int)id)
            {
                if (nodes[i].transform.position.z == element)
                {
                    node = nodes[i];
                }
            }
        }

        //Assign Value

        if (node != null)
        {
            value = (value + 1) * -1; //+1 to avoid 0s and inverted to not go above level
            node.transform.position = new Vector3(node.transform.position.x, value, node.transform.position.z);
        }
        else
        {
            Debug.LogWarning("Could not find node to modify | [Req] ID: " + (int)id + " ELEMENT: " + (int)element);
        }

    }

    public void Modify(TheGrandExchange.NODEID id, TheGrandExchange.TASKIDS element, bool value)
    {
        if (value)
        {
            Modify(id, (int)element, 1);
        }
        else
        {
            Modify(id, (int)element, 0);
        }
    }
}
