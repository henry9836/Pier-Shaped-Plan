using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    //public AudioClip winSound;

    private CanvasGroup canvas;

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

    void Start()
    {
        screenState = MenuState.Main;
        screenStateCur = (int)MenuState.Main;
        canvas = GetComponent<CanvasGroup>();
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
                    //GetComponent<AudioSource>().clip = winSound;
                    //GetComponent<AudioSource>().Play();
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
