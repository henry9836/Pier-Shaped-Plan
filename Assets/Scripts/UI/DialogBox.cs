using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogBox : MonoBehaviour
{

    public bool isEnabled;
    public float activateDelay = 0.5f;
    private bool beginRequested;
    private bool endRequested;

    private CanvasGroup canvas;
    public GameObject titleText;

    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        transform.DOScale(0f, 0f);
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
        Invoke("BeginAnimation", 0.2f);
        beginRequested = true;
        endRequested = false;
    }

    public void End()
    {
        Deactivate();
        beginRequested = false;
        endRequested = true;

        canvas.DOKill(true);
        canvas.DOFade(0f, 0.3f);
        transform.DOScale(0f, 0.5f).SetEase(Ease.InQuad);

    }

    private void BeginAnimation()
    {
        Invoke("Activate", activateDelay);

        canvas.DOFade(1f, 0.3f);
        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
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
