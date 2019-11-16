using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPick : MonoBehaviour
{
    public float UIfintimer = 2.0f;
    public float UIcurrenttimer = 0.0f;
    private float maxdistance = 5.0f;

    GameObject dispenser;

    void Start()
    {
        dispenser = GameObject.Find("DispenserLocations");
    }

    void Update()
    {
        if (this.GetComponent<PlayerController>().gameStarted == true)
        {
            if (this.GetComponent<PlayerController>().amHitman == false)
            {
                Destroy(this);
            }
        }

        //for each despencer
        for (int i = 0; i < dispenser.transform.childCount; i++)
        {
            //if in range
            if (Vector3.Distance(this.gameObject.transform.position, dispenser.transform.GetChild(i).transform.position) < maxdistance)
            {
                //if its not on cool down. inittimer = 30, currenttimer starts at 30 and goes down to 0
                //loading up: ((blah.inittimer - blah.currenttimer) / blah.inittimer)
                // or 
                //loading down: (blah.currenttimer / blah.inittimer)
                if (dispenser.transform.GetChild(i).GetComponent<bulletDispenser>().currenttimer <= 0.0f)
                {

                    if (Input.GetKey("e"))
                    {
                        //hold e for UIcurrenttimer/UIfintimer (2 seconds)
                        UIcurrenttimer += Time.deltaTime;

                        if (UIcurrenttimer >= UIfintimer)
                        {
                            //bullet despenced
                            add(i);
                        }
                    }
                    else
                    {
                        //reset holding timer
                        UIcurrenttimer = 0.0f; 
                    }
                }
            }
        }
    }

    void add(int i)
    {
        //resets timer, adds a bulet, resets holding timer
        dispenser.transform.GetChild(i).GetComponent<bulletDispenser>().currenttimer = dispenser.transform.GetChild(i).GetComponent<bulletDispenser>().inittimer;
        this.GetComponent<PewPewGun>().Bullets += 1;
        UIcurrenttimer = 0.0f;
    }
}
