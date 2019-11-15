using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPick : MonoBehaviour
{
    public float inittimer = 30.0f;
    public float currenttimer = 0.0f;

    public float UIinittimer = 2.0f;
    public float UIcurrenttimer = 2.0f;


    void Update()
    {
        currenttimer -= Time.deltaTime;
        if (currenttimer <= 0.0f)
        {
            if (this.GetComponent<PlayerController>().amHitman == true)
            {
                if (Input.GetKeyDown("e"))
                {
                    UIcurrenttimer -= Time.deltaTime;
             
                    if (UIcurrenttimer <= 0.0f)
                    {
                        add();
                    }
                }
                else
                {
                    UIcurrenttimer = UIinittimer;
                }
            }
        }
    }

    void add()
    {
        this.GetComponent<PewPewGun>().Bullets += 1;
        currenttimer = inittimer;
        UIcurrenttimer = UIinittimer;
    }
}
