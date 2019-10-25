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
        Credits,
        Quit
    }

    // 0 - Nain Menu
    // 1 - Options Menu
    // 2 - Credits Screen
    // 3 - Quit Dialog

    public MenuState screenState;
    private int screenStateCur = -1;

    public GameObject[] screens;
    public GameObject titleText;
    public GameObject[] buttons;

    private float titlePos;
    private float[] buttonPos;
    private float[] buttonStartPos;

    private CanvasGroup canvas;
    public AudioClip clickSound;

    public Text fpsText;
    public float deltaTime;

    void Start()
    {
        screenState = MenuState.Main;
        screenStateCur = (int)MenuState.Main;
        canvas = GetComponent<CanvasGroup>();

        titlePos = titleText.transform.localPosition.x;
        titleText.transform.DOLocalMoveX(-600f, 0f);

        buttonPos = new float[buttons.Length];
        buttonStartPos = new float[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            buttonPos[i] = buttons[i].transform.localPosition.x;
            buttonStartPos[i] = -200f - i * 50f;
            buttons[i].transform.DOKill(true);
            buttons[i].transform.DOLocalMoveX(buttonStartPos[i], 0f);
        }

        Begin();
    }

    void Update()
    {

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = Mathf.Ceil(fps).ToString();

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
                    screens[screenStateCur].GetComponent<OptionsMenu>().End();
                    break;

                case 2:
                    screens[screenStateCur].GetComponent<CreditsScreen>().End();
                    break;

                case 3:
                    screens[screenStateCur].GetComponent<DialogBox>().End();
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

                case MenuState.Quit:
                    screens[3].GetComponent<DialogBox>().Begin();
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
        Invoke("BeginAnimation", 0.2f);
    }

    public void End()
    {
        canvas.DOKill(true);
        canvas.DOFade(0f, 0.35f).SetEase(Ease.OutQuint);
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
