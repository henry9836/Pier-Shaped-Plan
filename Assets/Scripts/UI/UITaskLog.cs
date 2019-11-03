using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class UITaskLog : NetworkBehaviour
{
    public bool isEnabled;
    private bool beginRequested;
    private bool endRequested;

    public GameObject playerCanvas;
    private GameObject taskLog;
    private CanvasGroup canvas;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (playerCanvas == null)
        {
            playerCanvas = GameObject.Find("PlayerCanvas(Clone)");

            taskLog = playerCanvas.transform.Find("TaskLog").gameObject;
            canvas = taskLog.GetComponent<CanvasGroup>();

            taskLog.transform.DOScale(0f, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isEnabled = !isEnabled;
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

    private void Begin()
    {
        beginRequested = true;
        endRequested = false;

        canvas.DOKill(true);
        canvas.DOFade(1f, 0.3f);
        taskLog.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    public void End()
    {
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        beginRequested = false;
        endRequested = true;

        canvas.DOKill(true);
        canvas.DOFade(0f, 0.25f);
        taskLog.transform.DOScale(0f, 0.4f).SetEase(Ease.InQuad);
    }

}
