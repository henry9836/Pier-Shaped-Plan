using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CreditsScreen : MonoBehaviour
{
    private CanvasGroup canvas;

    void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    void Update()
    {

    }

    public void Begin()
    {
        Invoke("BeginAnimation", 0.25f);
    }

    public void End()
    {
        canvas.DOFade(0f, 0.25f);
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    private void BeginAnimation()
    {
        canvas.DOFade(1f, 0.25f);
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }
}
