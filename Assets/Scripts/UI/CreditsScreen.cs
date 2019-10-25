using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CreditsScreen : MonoBehaviour
{
    private CanvasGroup canvas;
    public float beginDelay = 0.2f;
    public float beginDelayTimer = 0f;
    private bool beginStarted = false;

    void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (beginStarted)
        {
            beginDelayTimer = Mathf.MoveTowards(beginDelayTimer, beginDelay, Time.deltaTime);
        }
        else
        {
            beginDelayTimer = 0f;
        }

        if (beginDelayTimer >= beginDelay)
        {
            canvas.DOFade(1f, 0.25f);
            canvas.interactable = true;
            canvas.blocksRaycasts = true;

            beginStarted = false;
        }
    }

    public void Begin()
    {
        beginStarted = true;
    }

    public void End()
    {
        canvas.DOFade(0f, 0.25f);
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        beginStarted = false;
    }
}
