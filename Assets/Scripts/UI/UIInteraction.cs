using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class UIInteraction : NetworkBehaviour
{
    private Interaction interact;

    private float nearInteractDistance;
    private GameObject[] interactables;
    private GameObject interactionPrompt;
    private Image interactProgressBar;

    private bool hasInitialized;
    private PlayerController player;
    private GameObject playerCanvas;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Initialize();

        if(!hasInitialized)
        {
            return;
        }

        FindNearestInteractable();
    }

    private void Initialize()
    {
        // Initialize player canvas UI

        if (playerCanvas == null)
        {
            interact = GetComponent<Interaction>();

            interactables = GameObject.FindGameObjectsWithTag("Interactable");
            Debug.Log(interactables.Length + " interactable objects found");

            player = GetComponent<PlayerController>();
            playerCanvas = GameObject.Find("PlayerCanvas(Clone)");

            interactionPrompt = playerCanvas.transform.Find("InteractionPrompt").gameObject;
            interactProgressBar = interactionPrompt.transform.Find("Text/HoldCircleProgress").GetComponent<Image>();

            hasInitialized = true;
        }
    }

    private void FindNearestInteractable()
    {
        if (Vector3.Distance(transform.position, TheGrandExchange.taskWorldPositions[(int)interact.theTask]) < interact.maxDistance)
        {
            Vector3 pos = TheGrandExchange.taskWorldPositions[(int)interact.theTask];

            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);
            interactionPrompt.SetActive(true);
            interactionPrompt.transform.position = screenPos;
        }
        else
        {
            interactionPrompt.SetActive(false);
        }

        interactProgressBar.fillAmount = interact.currentCompletion / interact.timeToComplete;

    }

}
