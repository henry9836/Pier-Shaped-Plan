using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{

    public enum MenuState
    {
        Main,
        Options,
        Credits
    }

    // 0 - Nain Menu
    // 1 - Options Menu
    // 2 - Credits Screen

    public MenuState screenState;
    private int screenStateCur = -1;

    public GameObject[] screens;
    public GameObject titleText;
    public GameObject[] buttons;

    private float titlePos;
    private float[] buttonPos;

    private CanvasGroup canvas;
    public AudioClip clickSound;

    void Start()
    {
        screenState = MenuState.Main;
        screenStateCur = (int)MenuState.Main;
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

        Begin();
    }

    void Update()
    {

    // Game screens
        if ((int)screenState != screenStateCur)
        {
            // Deactivate old screen
            switch (screenStateCur)
            {
                case 0:
                    End();
                    break;

                case 1:
                    screens[1].GetComponent<OptionsMenu>().End();
                    break;

                case 2:
                    screens[2].GetComponent<CreditsScreen>().End();
                    break;

            }

            // Activate new screen
            switch (screenState)
            {
                case MenuState.Main:
                    Begin();
                    break;

                case MenuState.Options:
                    screens[1].GetComponent<OptionsMenu>().Begin();

                    break;

                case MenuState.Credits:
                    screens[2].GetComponent<CreditsScreen>().Begin();
                    break;
            }

            screenStateCur = (int)screenState;
        }
        
    }

    public void GoToScreen(int screen)
    {
        screenState = (MenuState)screen;
        GetComponent<AudioSource>().clip = clickSound;
        GetComponent<AudioSource>().Play();
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
