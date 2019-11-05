using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonFlash : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Activate();
        }
    }

    public void Activate()
    {
        transform.DOKill(true);
        transform.DOPunchScale(new Vector3(1f, 1f, 1f), 0.22f, 0, 0f);
    }
}
