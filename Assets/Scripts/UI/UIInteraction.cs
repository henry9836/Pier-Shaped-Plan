using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class UIInteraction : NetworkBehaviour
{
    public float interactionDistance = 8f;

    private GameObject[] interactables;
    private GameObject interactionPrompt;

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
            interactables = GameObject.FindGameObjectsWithTag("Interactable");
            Debug.Log(interactables.Length + " interactable objects found");

            player = GetComponent<PlayerController>();
            playerCanvas = GameObject.Find("PlayerCanvas(Clone)");

            interactionPrompt = playerCanvas.transform.Find("InteractionPrompt").gameObject;

            hasInitialized = true;
        }
    }

    private void FindNearestInteractable()
    {
        GameObject nearestObject = null;
        float closestDistance = interactionDistance + 1.0f;

        for (int i = 0; i < interactables.Length; i++)
        {
            float dist = Vector3.Distance(interactables[i].transform.position, transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearestObject = interactables[i];
            }
        }

        // Show interaction prompt on interactable when close enough
        if (closestDistance < interactionDistance)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(nearestObject.transform.position);
            interactionPrompt.SetActive(true);
            interactionPrompt.transform.position = screenPos;
        }
        else
        {
            interactionPrompt.SetActive(false);
        }

    }

}
