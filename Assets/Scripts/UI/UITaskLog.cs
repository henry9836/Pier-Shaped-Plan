using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class UITaskLog : NetworkBehaviour
{
    public bool showTaskLog;
    private bool beginRequested;
    private bool endRequested;

    public float popupShowTime = 5f;
    private float popupTimer;
    private bool showingPopup;

    private PlayerController player;
    private GameObject playerCanvas;
    private GameObject taskLog;
    private CanvasGroup taskLogCanvas;
    private Text taskDescription;
    private Text tasksCompletedCounter;

    private GameObject taskPopup;
    private CanvasGroup taskPopupCanvas;
    private Image taskPopupCheckbox;
    private Text taskPopupDescription;

    public GameObject taskItemPrefab;
    public Sprite checkboxOn;
    public Sprite checkboxCross;
    public Sprite checkboxOff;
    public string[] tasks;
    public string hitmanDescription;
    public string victimDescription;

    private int taskCount = 5;
    private bool[] taskComplete;
    private int[] taskID;
    private GameObject[] nodes;
    private GameObject[] taskItem;
    private Image[] taskItemCheckbox;
    private Text[] taskItemText;

    private Decoder decoder;

    void Start()
    {
        decoder = GetComponent<Decoder>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Initialize();

        UpdateTaskLog();

        // Toggle task log display with TAB
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showTaskLog = !showTaskLog;
        }

        // Play open or close animation based on state of isEnabled
        if (showTaskLog && !beginRequested)
        {
            Begin();

        }

        if (!showTaskLog && !endRequested)
        {
            End();
        }

        if (showingPopup)
        {
            popupTimer = Mathf.MoveTowards(popupTimer, 0f, Time.deltaTime);
            if (popupTimer <= 0f)
            {
                hidePopup();
                showingPopup = false;
            }
        }
        else
        {
            popupTimer = popupShowTime;
        }
    }

    private void Initialize()
    {
        // Initialize player canvas UI

        if (playerCanvas == null)
        {
            player = GetComponent<PlayerController>();
            playerCanvas = GameObject.Find("PlayerCanvas(Clone)");

            taskLog = playerCanvas.transform.Find("TaskLog").gameObject;
            taskLog.transform.DOScale(0f, 0f);
            taskLogCanvas = taskLog.GetComponent<CanvasGroup>();
            taskDescription = taskLog.transform.Find("Description").GetComponent<Text>();
            tasksCompletedCounter = taskLog.transform.Find("TasksCompleted").GetComponent<Text>();

            taskPopup = playerCanvas.transform.Find("TaskPopup").gameObject;
            taskPopup.transform.DOScale(0f, 0f);
            taskPopupCanvas = taskPopup.GetComponent<CanvasGroup>();
            taskPopupCheckbox = taskPopup.transform.Find("Text/Check").GetComponent<Image>();
            taskPopupDescription = taskPopup.transform.Find("Text").GetComponent<Text>();

            // Instantiate task list items
            InitializeTasks();
        }
    }

    private void InitializeTasks()
    {
        // Find cubes that are encoded with task log data
        nodes = GameObject.FindGameObjectsWithTag("SERVERINFONODE");
        taskCount = nodes.Length / 2;
        Debug.Log("Number of tasks: " + taskCount);

        // Initialize arrays
        taskID = new int[taskCount];                // Task type assigned to each task number
        taskItem = new GameObject[taskCount];       // Task item prefab with checkboxes and text
        taskItemCheckbox = new Image[taskCount];    // Checkbox image for the task number
        taskItemText = new Text[taskCount];         // Text for the task number
        taskComplete = new bool[taskCount];         // Stores whether the task has been completed

        for (int i = 0; i < taskCount; i++)
        {
            // Find chosen tasks and set them in taskID[]
            taskID[i] = decoder.Decode(TheGrandExchange.NODEID.TASKLOG, i);

            // Create task item UI object for each task
            taskItem[i] = Instantiate(taskItemPrefab);
            taskItemCheckbox[i] = taskItem[i].transform.GetChild(0).GetComponent<Image>();
            taskItemText[i] = taskItem[i].transform.GetChild(1).GetComponent<Text>();

            taskItem[i].transform.parent = taskLog.transform;
            taskItem[i].transform.localPosition = new Vector3(0.0f, 64f - (i * 64f), 0);
            taskItem[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

            // Initialize task strings
            taskItemText[i].text = tasks[taskID[i]];
            Debug.Log("Task " + i + " wtih ID " + taskID[i] + " is " + tasks[taskID[i]]);
        }
    }

    private void UpdateTaskLog()
    {
        int tasksCompleted = 0;

        // Update task complete state
        for (int i = 0; i < taskCount; i++)
        {
            bool isComplete = decoder.DecodeBool(TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE, i);
            int lastTaskCompleted;

            if (taskComplete[i] != isComplete)
            {
                // Show popup when a task is completed
                lastTaskCompleted = i;
                taskComplete[i] = isComplete;
                taskPopupDescription.text = tasks[taskID[i]];

                if (!showingPopup)
                {
                    showPopup();
                    showingPopup = true;
                }

                Debug.Log("Completed task: " + tasks[taskID[i]]);
            }

            // Update checkboxes
            if (taskComplete[i])
            {
                taskItemCheckbox[i].sprite = player.amHitman ? checkboxCross : checkboxOn;
                tasksCompleted++;
            }
            else
            {
                taskItemCheckbox[i].sprite = checkboxOff;
            }
        }

        // Update tasks completed counter
        tasksCompletedCounter.text = tasksCompleted + "/" + taskCount + " tasks completed";

        // Change description of task log depending on whether the player is a hitman
        taskDescription.text = player.amHitman ? hitmanDescription : victimDescription;
        taskPopupCheckbox.sprite = player.amHitman ? checkboxCross : checkboxOn;
    }

    private void Begin()
    {
        // Play open animation
        beginRequested = true;
        endRequested = false;

        taskLogCanvas.DOKill(true);
        taskLogCanvas.DOFade(1f, 0.3f);
        taskLog.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);

        // Move popup out of the way
        taskPopup.transform.DOLocalMoveX(360f, 0.3f);
    }

    public void End()
    {
        // Play close animation
        beginRequested = false;
        endRequested = true;

        taskLogCanvas.DOKill(true);
        taskLogCanvas.DOFade(0f, 0.25f);
        taskLog.transform.DOScale(0f, 0.4f).SetEase(Ease.InQuad);

        // Move popup back
        taskPopup.transform.DOLocalMoveX(0f, 0.4f);
    }


    private void showPopup()
    {
        taskPopupCanvas.DOKill(true);
        taskPopupCanvas.DOFade(1f, 0.3f);
        taskPopup.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void hidePopup()
    {
        taskPopupCanvas.DOKill(true);
        taskPopupCanvas.DOFade(0f, 0.25f);
        taskPopup.transform.DOScale(0f, 0.4f).SetEase(Ease.InQuad);
    }

}
