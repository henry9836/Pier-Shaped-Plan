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

    public bool showStart;
    private bool readStart;
    private GameObject endScreen;
    private CanvasGroup endScreenCanvas;
    private bool beginRequested;
    private bool endRequested;

    private Text endTitleText;
    private Text endDescText;
    private GameObject decorCircle;

    public string hitmanEndTitleSuccess = "MISSION COMPLETE";
    public string hitmanEndDescSuccess = "Target eliminated. Good work, Agent!";
    public string targetEndTitleSuccess = "YOU ESCAPED";
    public string targetEndDescSuccess = "Successfully completed tasks and escaped the area.";

    public string hitmanEndTitleFail = "MISSION FAILED";
    public string hitmanEndDescFail = "Your target managed to escape to area.";
    public string targetEndTitleFail = "YOU ARE DEAD";
    public string targetEndDescFail = "Assassinated by the Hitman.";

    private GameObject StartHitman;
    private CanvasGroup StartHitmanCanvas;

    private GameObject StartTarget;
    private CanvasGroup StartTargetCanvas;


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

        // Set text based on whether player is hitman and whether they succeeded
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

        decorCircle.transform.Rotate(new Vector3(0.0f, 0.0f, 30.0f * Time.deltaTime), Space.Self);

        if (!hasInitialized)
        {
            return;
        }
        
        if (player.gameStarted && !readStart)
        {
            showStart = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Tab))
        {
            showStart = false;
            readStart = true;
        }

        if (player.gameOverState)
        {
            ShowEndScreen();
        }

        if (showStart && !beginRequested)
        {
            ShowStartScreen();

        }

        if (!showStart && !endRequested)
        {
            HideStartScreen();
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
            decorCircle = endScreen.transform.Find("DecorCircle").gameObject;

            StartHitman = playerCanvas.transform.Find("StartScreenHitman").gameObject;
            StartHitmanCanvas = StartHitman.GetComponent<CanvasGroup>();

            StartTarget = playerCanvas.transform.Find("StartScreenTarget").gameObject;
            StartTargetCanvas = StartTarget.GetComponent<CanvasGroup>();

            endScreen.transform.DOScale(0.0f, 0.0f);
            endScreenCanvas.alpha = 0.0f;

            StartHitman.transform.DOScale(0.0f, 0.0f);
            StartHitmanCanvas.alpha = 0.0f;

            StartTarget.transform.DOScale(0.0f, 0.0f);
            StartTargetCanvas.alpha = 0.0f;


            hasInitialized = true;
        }
    }

    private void ShowEndScreen()
    {
        if (!showStart)
        {
            Cursor.lockState = CursorLockMode.None;

            endScreenCanvas.DOKill(true);
            endScreenCanvas.DOFade(1f, 0.3f);
            endScreen.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    private void ShowStartScreen()
    {
        beginRequested = true;
        endRequested = false;

        if (player.amHitman)
        {
            StartHitmanCanvas.DOKill(true);
            StartHitmanCanvas.DOFade(1f, 0.3f);
            StartHitman.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            StartTargetCanvas.DOKill(true);
            StartTargetCanvas.DOFade(1f, 0.3f);
            StartTarget.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    private void HideStartScreen()
    {
        beginRequested = false;
        endRequested = true;

        if (player.amHitman)
        {
            StartHitmanCanvas.DOKill(true);
            StartHitmanCanvas.DOFade(0f, 0.25f);
            //StartHitman.transform.DOScale(0f, 0.4f).SetEase(Ease.InQuad);
        }
        else
        {
            StartTargetCanvas.DOKill(true);
            StartTargetCanvas.DOFade(0f, 0.25f);
            //StartTarget.transform.DOScale(0f, 0.4f).SetEase(Ease.InQuad);
        }
    }
}
