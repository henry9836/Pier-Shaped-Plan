using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PewPewGun : NetworkBehaviour
{
    public int Bullets = 1;
    public int maxBullets = 6;
    public GameObject Camera;
    public GameObject Gun;

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
    void CmddrawLine(Vector3 hitmanLaserPos, Vector3 gunPos, Vector3 gunHitPos)
    {
        this.gameObject.GetComponent<LineRenderer>().SetPosition(0, gunPos);
        this.gameObject.GetComponent<LineRenderer>().SetPosition(1, gunHitPos);

        //Make gun look at its target
        GameObject.Find("Gun(Clone)").transform.LookAt(hitmanLaserPos);

        RpcdrawLine(gunHitPos, gunPos);
    }

    [ClientRpc]
    void RpcdrawLine(Vector3 hitmanLaserPos, Vector3 gunPos)
    {
        GameObject hitmanRefer;

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().amHitman)
            {
                hitmanRefer = GameObject.FindGameObjectsWithTag("Player")[i];
                hitmanRefer.GetComponent<LineRenderer>().SetPosition(0, gunPos);
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
        else if (this.Gun == null)
        {
            Gun = GameObject.Find("Gun(Clone)");
        }
        else
        {
            RaycastHit Shot;
            RaycastHit gunShot;
            Physics.Raycast(Camera.transform.position, Camera.transform.forward, out Shot, Mathf.Infinity);
            Physics.Raycast(Gun.transform.position, Gun.transform.forward, out gunShot, Mathf.Infinity);

            if (this.gameObject.GetComponent<PlayerController>().amHitman == true)
            {
                this.gameObject.GetComponent<LineRenderer>().SetPosition(0, Gun.transform.position);
                this.gameObject.GetComponent<LineRenderer>().SetPosition(1, gunShot.point);

                CmddrawLine(Shot.point, Gun.transform.position, gunShot.point);


                if (Input.GetMouseButtonDown(0))
                {
                    pew();
                }
            }


        }
    }
}
