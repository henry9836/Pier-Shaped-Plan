using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class EndScreen : NetworkBehaviour
{
    private PlayerController player;
    private GameObject playerCanvas;
    private bool hasInitialized;

    public bool showEnd;
    private GameObject endScreen;
    private CanvasGroup endScreenCanvas;
    private bool beginRequested;
    private bool endRequested;

    private Text endTitleText;
    private Text endDescText;

    public string hitmanEndTitleSuccess = "MISSION COMPLETE";
    public string hitmanEndDescSuccess = "Target eliminated. Good work, Agent!";
    public string targetEndTitleSuccess = "YOU ESCAPED";
    public string targetEndDescSuccess = "Successfully completed tasks and esacped the area.";

    public string hitmanEndTitleFail = "MISSION FAILED";
    public string hitmanEndDescFail = "Your target managed to escape to area.";
    public string targetEndTitleFail = "YOU ARE DEAD";
    public string targetEndDescFail = "Assassinated by the Hitman."; 


    void Start()
    {
        
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Initialize();

        if (player.amHitman)
        {
            if (player.escaped)
            {
                endTitleText.text = hitmanEndTitleFail;
                endDescText.text = hitmanEndDescFail;
            }
            else
            {
                endTitleText.text = hitmanEndTitleSuccess;
                endDescText.text = hitmanEndDescSuccess;
            }
        }
        else
        {
            if (player.escaped)
            {
                endTitleText.text = targetEndTitleSuccess;
                endDescText.text = targetEndDescSuccess;
            }
            else
            {
                endTitleText.text = targetEndTitleFail;
                endDescText.text = targetEndDescFail;
            }
        }


        if (!hasInitialized)
        {
            return;
        }

        if (player.gameOverState)
        {
            showEnd = true;
        }

        if (showEnd && !beginRequested)
        {
            ShowEndScreen();

        }

        if (!showEnd && !endRequested)
        {
            HideEndScreen();
        }
    }

    private void Initialize()
    {
        if (playerCanvas == null)
        {
            player = GetComponent<PlayerController>();
            playerCanvas = GameObject.Find("PlayerCanvas(Clone)");

            endScreen = playerCanvas.transform.Find("EndScreen").gameObject;
            endScreenCanvas = endScreen.GetComponent<CanvasGroup>();
            endTitleText = endScreen.transform.Find("Title").GetComponent<Text>();
            endDescText = endScreen.transform.Find("Description").GetComponent<Text>();

            endScreen.transform.DOScale(0.0f, 0.0f);
            endScreenCanvas.alpha = 0.0f;

            hasInitialized = true;
        }
    }

    private void ShowEndScreen()
    {
        Cursor.lockState = CursorLockMode.None;

        endScreenCanvas.DOKill(true);
        endScreenCanvas.DOFade(1f, 0.3f);
        endScreen.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void HideEndScreen()
    {
        endScreenCanvas.DOKill(true);
        endScreenCanvas.DOFade(0f, 0.25f);
        endScreen.transform.DOScale(0f, 0.4f).SetEase(Ease.InQuad);
    }
}
