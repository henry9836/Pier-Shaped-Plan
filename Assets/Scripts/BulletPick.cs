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
        dispenser = GameObject.Find("dispenser");
    }

    void Update()
    {
        if (this.GetComponent<PlayerController>().amHitman == false)
        {
            Destroy(this);
        }

        for (int i = 0; i < dispenser.transform.childCount; i++)
        {
            if (Vector3.Distance(this.gameObject.transform.position, dispenser.transform.GetChild(i).transform.position) < maxdistance)
            {
                if (dispenser.transform.GetChild(i).GetComponent<bulletDispencer>().currenttimer <= 0.0f)
                {
                    if (Input.GetKeyDown("e"))
                    {
                        UIcurrenttimer += Time.deltaTime;

                        if (UIcurrenttimer >= UIfintimer)
                        {
                            add(i);
                        }
                    }
                    else
                    {
                        UIcurrenttimer = 0.0f; ;
                    }
                }
            }
        }
    }

    void add(int i)
    {
        dispenser.transform.GetChild(i).GetComponent<bulletDispencer>().currenttimer = dispenser.transform.GetChild(i).GetComponent<bulletDispencer>().inittimer;
        this.GetComponent<PewPewGun>().Bullets += 1;
        UIcurrenttimer = 0.0f;
    }
}
