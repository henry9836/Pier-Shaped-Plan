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

    public bool isEnabled;
    public float activateDelay = 0.5f;
    private bool beginRequested;
    private bool endRequested;

    public GameObject[] screens;
    public GameObject titleText;
    public GameObject[] buttons;

    private float titlePos;
    private float[] buttonPos;
    private float[] buttonStartPos;

    private CanvasGroup canvas;
    public AudioClip clickSound;

    public Text fpsText;
    private float deltaTime;

    void Start()
    {
        // Network manager, get out!
        GameObject netMan = GameObject.Find("NetworkManager");
        if (netMan != null)
        {
            //GameObject.Destroy(netMan);
        }

        Cursor.lockState = CursorLockMode.None;
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

        isEnabled = true;
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
                    isEnabled = false;
                    break;

                case 1:
                    screens[screenStateCur].GetComponent<OptionsMenu>().isEnabled = false;
                    break;

                case 2:
                    screens[screenStateCur].GetComponent<CreditsScreen>().isEnabled = false;
                    break;

                case 3:
                    screens[screenStateCur].GetComponent<DialogBox>().isEnabled = false;
                    break;

            }

            // Activate new screen
            switch (screenState)
            {
                case MenuState.Main:
                    isEnabled = true;
                    break;

                case MenuState.Options:
                    screens[(int)screenState].GetComponent<OptionsMenu>().isEnabled = true;

                    break;

                case MenuState.Credits:
                    screens[(int)screenState].GetComponent<CreditsScreen>().isEnabled = true;
                    break;

                case MenuState.Quit:
                    screens[(int)screenState].GetComponent<DialogBox>().isEnabled = true;
                    break;
            }

            screenStateCur = (int)screenState;
        }

        if (isEnabled && !beginRequested)
        {
            Begin();

        }

        if (!isEnabled && !endRequested)
        {
            End();
        }

    }

    public void GoToScreen(int screen)
    {
        screenState = (MenuState)screen;
        GetComponent<AudioSource>().clip = clickSound;
        GetComponent<AudioSource>().Play();
    }

    private void Begin()
    {
        Invoke("BeginAnimation", 0.2f);
        beginRequested = true;
        endRequested = false;
    }

    private void End()
    {
        Deactivate();
        beginRequested = false;
        endRequested = true;

        canvas.DOKill(true);
        canvas.DOFade(0f, 0.2f).SetEase(Ease.OutQuint);
        titleText.transform.DOLocalMoveX(-600f, 1.25f).SetEase(Ease.OutQuint);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.DOLocalMoveX(buttonStartPos[i], 1.0f).SetEase(Ease.OutQuint);
        }
    }

    private void BeginAnimation()
    {
        Invoke("Activate", activateDelay);

        canvas.DOFade(1f, 0.5f).SetEase(Ease.InSine);
        titleText.transform.DOLocalMoveX(titlePos, 1.25f).SetEase(Ease.OutQuint);

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.DOLocalMoveX(buttonPos[i], 1.0f).SetEase(Ease.OutQuint);
        }
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
