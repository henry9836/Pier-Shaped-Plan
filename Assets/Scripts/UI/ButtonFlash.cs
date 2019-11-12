using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ButtonFlash : MonoBehaviour
{
    public void Activate()
    {
        transform.DOKill(true);
        transform.DOPunchScale(new Vector3(1f, 1f, 1f), 0.22f, 0, 0f);
    }
}
