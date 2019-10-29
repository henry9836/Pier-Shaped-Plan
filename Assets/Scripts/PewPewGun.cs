using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PewPewGun : NetworkBehaviour
{
    public int Bullets = 1;
    public int maxBullets = 6;
    public GameObject Camera;

    //when picked up bullets
    void AddBullet(int shots)
    {
        Bullets += shots;
        if (Bullets > maxBullets)
        {
            Bullets = maxBullets;
        }
    }

    void pew()
    {
        if (Bullets <= 0)
        {
            Bullets = 0;

            //empty click sfx
        }
        else
        {
            //pew sfx

            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            int hitmanno = 0;
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i].GetComponent<PlayerController>().amHitman == true)
                {
                    hitmanno = i;
                }
            }

            Players[hitmanno].GetComponent<PlayerController>().CmdFireBullet();

            Bullets -= 1;
            //find game manger and say that it hit Shot.collider.name
        }
    }

    [Command]
    void CmddrawLine(Vector3 hitmanLaserPos)
    {
        this.gameObject.GetComponent<LineRenderer>().SetPosition(0, transform.position);
        this.gameObject.GetComponent<LineRenderer>().SetPosition(1, hitmanLaserPos);
        RpcdrawLine(hitmanLaserPos);
    }

    [ClientRpc]
    void RpcdrawLine(Vector3 hitmanLaserPos)
    {
        GameObject hitmanRefer;

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().amHitman)
            {
                hitmanRefer = GameObject.FindGameObjectsWithTag("Player")[i];
                hitmanRefer.GetComponent<LineRenderer>().SetPosition(0, hitmanRefer.transform.position);
                hitmanRefer.GetComponent<LineRenderer>().SetPosition(1, hitmanLaserPos);
            }
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (this.Camera == null)
        {
            Camera = GameObject.Find("Main Camera");
        }
        else
        {
            RaycastHit Shot;
            Physics.Raycast(Camera.transform.position, Camera.transform.forward, out Shot, Mathf.Infinity);
            Debug.DrawLine(Camera.transform.position, Shot.point);

            if (this.gameObject.GetComponent<PlayerController>().amHitman == true)
            {
                Debug.DrawLine(Shot.point, transform.position);
                this.gameObject.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                this.gameObject.GetComponent<LineRenderer>().SetPosition(1, Shot.point);

                CmddrawLine(Shot.point);


                if (Input.GetMouseButtonDown(0))
                {
                    pew();
                }
            }


        }
    }
}
