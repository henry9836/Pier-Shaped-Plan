using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class UIInteraction : NetworkBehaviour
{
    private Decoder decoder;
    private Interaction interact;
    private float nearInteractDistance = 12f;
    private Vector3[] interactionPoints;

    private GameObject playerCanvas;
    public GameObject interactionDotPrefab;
    private GameObject interactionPrompt;
    private Image interactProgressBar;

    private bool hasInitialized;
    private PlayerController player;

    private int taskCount;
    private int[] taskID;
    private bool[] taskComplete;
    private GameObject[] interactionDot;

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

        if (playerCanvas == null)
        {
            playerCanvas = GameObject.Find("PlayerCanvas(Clone)");
            decoder = GetComponent<Decoder>();
            interact = GetComponent<Interaction>();
            nearInteractDistance = interact.maxDistance * 3.0f;

            // Find cubes that are encoded with task log data
            GameObject[] nodes = GameObject.FindGameObjectsWithTag("SERVERINFONODE");
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].transform.position.x == (int)TheGrandExchange.NODEID.TASKLOG)
                {
                    taskCount++;
                }
            }

            taskID = new int[taskCount];
            taskComplete = new bool[taskCount];
            interactionDot = new GameObject[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                // Find chosen tasks and set them in taskID[]
                taskID[i] = decoder.Decode(TheGrandExchange.NODEID.TASKLOG, i);

                // Create UI dot for each interaction point
                interactionDot[i] = Instantiate(interactionDotPrefab);
                interactionDot[i].transform.SetParent(playerCanvas.transform);
                interactionDot[i].transform.localPosition = new Vector3(0.0f, 64f - (i * 64f), 0);
                interactionDot[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            }

            interactionPrompt = playerCanvas.transform.Find("InteractionPrompt").gameObject;
            interactProgressBar = interactionPrompt.transform.Find("Text/HoldCircleProgress").GetComponent<Image>();

            hasInitialized = true;
        }
    }

    private void FindNearestInteractable()
    {
        // Draw dot on nearby interaction points
        for (int i = 0; i < taskCount; i++)
        {
            if (Vector3.Distance(transform.position, TheGrandExchange.taskWorldPositions[taskID[i]]) < nearInteractDistance)
            {
                interactionDot[i].SetActive(true);

                Vector3 screenPos = Camera.main.WorldToScreenPoint(TheGrandExchange.taskWorldPositions[taskID[i]]);
                interactionDot[i].transform.position = screenPos;
            }
            else
            {
                interactionDot[i].SetActive(false);
            }

        }

        // Show interaction prompt when close enough
        if (Vector3.Distance(transform.position, TheGrandExchange.taskWorldPositions[(int)interact.theTask]) < interact.maxDistance)
        {
            // Disable dot when prompt appears
            for (int i = 0; i < taskCount; i++)
            {
                if (taskID[i] == (int)interact.theTask)
                {
                    interactionDot[i].SetActive(false);
                }
            }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(TheGrandExchange.taskWorldPositions[(int)interact.theTask]);
            interactionPrompt.SetActive(true);
            interactionPrompt.transform.position = screenPos;
        }
        else
        {
            interactionPrompt.SetActive(false);
        }

        // Update interaction progress ring
        interactProgressBar.fillAmount = interact.currentCompletion / interact.timeToComplete;

    }

}
