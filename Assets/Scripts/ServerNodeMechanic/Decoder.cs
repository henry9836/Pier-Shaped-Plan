using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoder : MonoBehaviour
{
    public int Decode(GameObject node)
    {
        int result = 0;

        //Get value

        result = (int)node.transform.position.y;

        //uninvert value
        result *= -1;

        //remove padding
        result -= 1;

        return result;
    }

    public int Decode(TheGrandExchange.NODEID id, int element)
    {
        int result = 0;

        //Get node
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

        //Read node
        if (node != null)
        {
            //Get value
            result = (int)node.transform.position.y;

            //uninvert value
            result *= -1;

            //remove padding
            result -= 1;
        }
        else
        {
            Debug.LogWarning("Could not find node to decode | [Req] ID: " + (int)id + " ELEMENT: " + element);
        }

        return result;
    }

    public bool DecodeBool(TheGrandExchange.NODEID id, int element)
    {
        bool result = false;

        int resultInt = Decode(id, element);

        if (resultInt == 1)
        {
            result = true;
        }

        return result;
    }
}
