using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CreditsScreen : MonoBehaviour
{
    public GameObject[] screens;
    public GameObject titleText;
    public GameObject[] buttons;

    private float titlePos;
    private float[] buttonPos;

    private CanvasGroup canvas;

    void Awake()
    {
        canvas = GetComponent<CanvasGroup>();

        titlePos = titleText.transform.localPosition.x;
        titleText.transform.DOLocalMoveX(-600f, 0f);

        buttonPos = new float[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonPos[i] = buttons[i].transform.localPosition.x;
            buttons[i].transform.DOKill(true);
            buttons[i].transform.DOLocalMoveX(-320f, 0f);
        }
    }

    void Update()
    {

    }

    public void Begin()
    {
        Invoke("BeginAnimation", 0.33f);
    }

    public void End()
    {
        canvas.DOFade(0f, 0.5f);
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        titleText.transform.DOLocalMoveX(-600f, 0.5f).SetEase(Ease.OutQuart);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.DOLocalMoveX(-320f, 0.5f).SetEase(Ease.OutQuart);
        }
    }

    private void BeginAnimation()
    {
        canvas.DOFade(1f, 0.5f);
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        titleText.transform.DOLocalMoveX(titlePos, 0.5f).SetEase(Ease.OutQuart);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.DOLocalMoveX(buttonPos[i], 0.5f).SetEase(Ease.OutQuart);
        }
    }
}
