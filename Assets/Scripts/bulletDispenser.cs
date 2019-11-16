using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletDispenser : MonoBehaviour
{
    public float inittimer = 30.0f;
    public float currenttimer = 0.0f;

    void Update()
    {
        if (currenttimer > 0.0f)
        {
            currenttimer -= Time.deltaTime;
        }
        if (currenttimer < 0.0f)
        {
            currenttimer = 0.0f;
        }

    }
}
