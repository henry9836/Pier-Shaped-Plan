using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TabScreen : MonoBehaviour
{
    public GameObject content;

    public bool isEnabled;
    public float activateDelay = 0.5f;
    private bool beginRequested;
    private bool endRequested;

    private CanvasGroup canvas;

    void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    void Update()
    {



        if (isEnabled && !beginRequested)
        {
            Begin();

        }

        if (!isEnabled && !endRequested)
        {
            End();
        }
    }

    private void Begin()
    {
        Invoke("BeginAnimation", 0.0f);
        beginRequested = true;
        endRequested = false;
    }

    private void End()
    {
        Deactivate();
        beginRequested = false;
        endRequested = true;

        canvas.DOKill(true);
        canvas.DOFade(0f, 0.1f).SetEase(Ease.OutQuint);
    }

    private void BeginAnimation()
    {
        Invoke("Activate", activateDelay);

        canvas.DOFade(1f, 0.1f).SetEase(Ease.InSine);
    }

    private void Activate()
    {
        if (isEnabled)
        {
            canvas.interactable = true;
            canvas.blocksRaycasts = true;
        }
    }

    private void Deactivate()
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }
}
