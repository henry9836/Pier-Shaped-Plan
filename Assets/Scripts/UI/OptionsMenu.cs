using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OptionsMenu : MonoBehaviour
{

    public GameObject[] screens;
    public GameObject titleText;
    public GameObject[] buttons;

    private float titlePos;
    private float[] buttonPos;
    private float[] buttonStartPos;

    private CanvasGroup canvas;

    void Awake()
    {
        canvas = GetComponent<CanvasGroup>();

        titlePos = titleText.transform.localPosition.x;
        titleText.transform.DOLocalMoveX(-600f, 0f);

        buttonPos = new float[buttons.Length];
        buttonStartPos = new float[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonPos[i] = buttons[i].transform.localPosition.x;
            buttonStartPos[i] = -180f - i * 50f;
            buttons[i].transform.DOKill(true);
            buttons[i].transform.DOLocalMoveX(buttonStartPos[i], 0f);
        }
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
        canvas.DOFade(0f, 0.2f).SetEase(Ease.OutQuint);
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        titleText.transform.DOLocalMoveX(-600f, 1.25f).SetEase(Ease.OutQuint);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.DOLocalMoveX(buttonStartPos[i], 1.0f).SetEase(Ease.OutQuint);
        }
    }

    private void BeginAnimation()
    {
        canvas.DOFade(1f, 0.5f).SetEase(Ease.InSine);
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        titleText.transform.DOLocalMoveX(titlePos, 1.25f).SetEase(Ease.OutQuint);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.DOLocalMoveX(buttonPos[i], 1.0f).SetEase(Ease.OutQuint);
        }
    }
}
