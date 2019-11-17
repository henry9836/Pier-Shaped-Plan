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
        //If gun is out
        if (GetComponent<PlayerController>().gunOut)
        {
            //if we ran out of bullets
            if (Bullets <= 0)
            {
                Bullets = 0;

                //empty click sfx
            }
            //we have a bullet to shoot
            else
            {
                //pew sfx
                GetComponent<PNESAnimator>().CmdUpdateAnimation(TheGrandExchange.NODEID.PLAYERANIMATORSHOOT, GetComponent<PlayerController>().PNESid, 1);
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
    }

    [Command]
    void CmddrawLine(Vector3 hitmanLaserPos, Vector3 gunPos, Vector3 gunHitPos)
    {

        //Disable render if we don't have gun out
        if (GetComponent<PlayerController>().gunOut)
        {
            GetComponent<LineRenderer>().enabled = true;
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }

        this.gameObject.GetComponent<LineRenderer>().SetPosition(0, gunPos);
        this.gameObject.GetComponent<LineRenderer>().SetPosition(1, gunHitPos);

        //Make gun look at its target
        GameObject.FindGameObjectWithTag("Gun").transform.LookAt(hitmanLaserPos);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (!players[i].GetComponent<PlayerController>().amHitman)
            {
                TargetdrawLine(players[i].GetComponent<NetworkIdentity>().connectionToClient, gunHitPos, gunPos);
            }
        }
        
        
    }

    [TargetRpc]
    void TargetdrawLine(NetworkConnection connection, Vector3 hitmanLaserPos, Vector3 gunPos)
    {
        GameObject hitmanRefer;

        GameObject.FindGameObjectWithTag("Gun").transform.LookAt(hitmanLaserPos);

        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<PlayerController>().amHitman)
            {
                hitmanRefer = GameObject.FindGameObjectsWithTag("Player")[i];
                hitmanRefer.GetComponent<LineRenderer>().SetPosition(0, gunPos);
                hitmanRefer.GetComponent<LineRenderer>().SetPosition(1, hitmanLaserPos);
                //Disable render if we don't have gun out
                if (hitmanRefer.GetComponent<PlayerController>().gunOut)
                {
                    hitmanRefer.GetComponent<LineRenderer>().enabled = true;
                }
                else
                {
                    hitmanRefer.GetComponent<LineRenderer>().enabled = false;
                }

            }
        }
       
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //Disable render if we don't have gun out
        if (GetComponent<PlayerController>().gunOut) {
            GetComponent<LineRenderer>().enabled = true;
        }
        else
        {
            GetComponent<LineRenderer>().enabled = false;
        }


        if (this.Camera == null)
        {
            Camera = GameObject.Find("Main Camera");
        }
        else if (this.Gun == null)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetComponent<PlayerController>().amHitman)
                {
                    players[i].transform.GetChild(0).transform.GetChild(0).tag = "Gun";
                    players[i].transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
            }
            Gun = GameObject.FindGameObjectWithTag("Gun");
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

                //Make gun look at its target
                GameObject.FindGameObjectWithTag("Gun").transform.LookAt(Shot.point);

                CmddrawLine(Shot.point, Gun.transform.position, gunShot.point);

                //Shoot
                if (Input.GetMouseButtonDown(0))
                {
                    pew();
                }
            }


        }
    }
}
