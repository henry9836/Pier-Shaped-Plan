using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogBox : MonoBehaviour
{

    private CanvasGroup canvas;
    public GameObject titleText;

    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        transform.DOScale(0f, 0f);
    }

    void Update()
    {
        
    }

    public void Begin()
    {
        Invoke("BeginAnimation", 0.2f);
    }

    public void End()
    {
        canvas.DOKill(true);
        canvas.DOFade(0f, 0.3f);
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        transform.DOScale(0f, 0.5f).SetEase(Ease.InQuad);

    }

    private void BeginAnimation()
    {
        canvas.DOFade(1f, 0.3f);
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

}
