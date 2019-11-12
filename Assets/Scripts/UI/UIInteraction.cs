using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class UIInteraction : NetworkBehaviour
{
    public string[] interactLabels = new string[] 
    { "Eat Fish", "Greet Fisherman", "Use Phone", "Buy Paper", "Tie Boat", "Drop Package" };

    private Decoder decoder;
    private Interaction interact;
    private float nearInteractDistance = 12f;
    private Vector3[] interactionPoints;

    private GameObject playerCanvas;
    public GameObject interactionDotPrefab;
    private GameObject interactionPrompt;
    private Text interactText;
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
            player = GetComponent<PlayerController>();
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
            interactText = interactionPrompt.transform.Find("Text").GetComponent<Text>();
            interactProgressBar = interactionPrompt.transform.Find("Text/HoldCircleProgress").GetComponent<Image>();

            hasInitialized = true;
        }
    }

    private void FindNearestInteractable()
    {
        int curTaskID = (int)interact.theTask;

        // Draw dot on nearby interaction points
        for (int i = 0; i < taskCount; i++)
        {
            float dist = Vector3.Distance(transform.position, TheGrandExchange.taskWorldPositions[taskID[i]]);
            bool isComplete = decoder.DecodeBool(TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE, i);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(TheGrandExchange.taskWorldPositions[taskID[i]]);
            interactionDot[i].transform.position = screenPos;

            if (dist < nearInteractDistance && !isComplete && interactionDot[i].transform.position.z > 0f)
            {
                interactionDot[i].SetActive(true);
            }
            else
            {
                interactionDot[i].SetActive(false);
            }

        }

        // Draw prompt on top of interaction point when close enough and task isn't complete
        float nearestDist = Vector3.Distance(transform.position, TheGrandExchange.taskWorldPositions[curTaskID]);
        bool nearestIsComplete = decoder.DecodeBool(TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE, TaskIDToIndex(curTaskID));
        Vector3 nearestScreenPos = Camera.main.WorldToScreenPoint(TheGrandExchange.taskWorldPositions[curTaskID]);
        interactionPrompt.transform.position = nearestScreenPos;

        // Set prompt text to appropriate string based on the associated task
        interactText.text = interactLabels[curTaskID];

        if (nearestDist < interact.maxDistance && !nearestIsComplete && interactionPrompt.transform.position.z > 0f)
        {
            interactionPrompt.SetActive(true);

            // Disable dot when prompt appears
            interactionDot[TaskIDToIndex(curTaskID)].SetActive(false);
        }
        else
        {
            interactionPrompt.SetActive(false);
        }

        // Disable if hitman or completing task
        if (interactProgressBar.fillAmount == 1.0f || player.amHitman)
        {
            interactionPrompt.SetActive(false);
            for (int i = 0; i < taskCount; i++)
            {
                interactionDot[i].SetActive(false);
            }
        }

        // Update interaction progress ring
        interactProgressBar.fillAmount = interact.INTRtimer / interact.INTRtimerMAX;

    }

    private int TaskIDToIndex(int ID)
    {
        int index = 0;

        for (int i = 0; i < taskCount; i++)
        {
            if (taskID[i] == ID)
            {
                index = i;
            }
        }

        return index;
    }

}
