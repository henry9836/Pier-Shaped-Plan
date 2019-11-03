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

    public GameObject gameManager;
    private TaskLog taskManager;
    private GameObject playerCanvas;
    private GameObject taskLog;
    private CanvasGroup canvas;

    public GameObject taskItemPrefab;
    public Sprite checkboxOn;
    public Sprite checkboxOff;
    public int taskCount = 5;
    public string[] tasks;

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isEnabled = !isEnabled;
        }

        // Play open or close animation based on state of isEnabled
        if (isEnabled && !beginRequested)
        {
            Begin();

        }

        if (!isEnabled && !endRequested)
        {
            End();
        }
    }

    private void Initialize()
    {
        // Initialize player canvas UI

        if (playerCanvas == null)
        {
            playerCanvas = GameObject.Find("PlayerCanvas(Clone)");

            taskLog = playerCanvas.transform.Find("TaskLog").gameObject;
            canvas = taskLog.GetComponent<CanvasGroup>();
            taskLog.transform.DOScale(0f, 0f);

            CmdGetLog();

            // Instantiate task list items
            //taskCount = taskManager.numberoftasks;
            taskItem = new GameObject[taskCount];
            taskItemCheckbox = new Image[taskCount];
            taskItemText = new Text[taskCount];

            for (int i = 0; i < taskCount; i++)
            {
                taskItem[i] = Instantiate(taskItemPrefab);
                taskItemCheckbox[i] = taskItem[i].transform.GetChild(0).GetComponent<Image>();
                taskItemText[i] = taskItem[i].transform.GetChild(1).GetComponent<Text>();

                taskItem[i].transform.parent = taskLog.transform;
                taskItem[i].transform.localPosition = new Vector3(0.0f, 170f - (i * 64f), 0);
                taskItem[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                // Initialize task strings
                // here
            }
        }
    }

    private void UpdateTaskLog()
    {
        nodes = GameObject.FindGameObjectsWithTag("SERVERINFONODE");

        // Update checkboxes
        for (int i = 0; i < taskCount; i++)
        {
            bool isComplete = decoder.DecodeBool(TheGrandExchange.NODEID.TASKLOGCOMPLETESTATE, (int)nodes[i].transform.position.z);

            if (isComplete)
            {
                taskItemCheckbox[i].sprite = checkboxOn;
            }
            else
            {
                taskItemCheckbox[i].sprite = checkboxOff;
            }
        }
    }

    [Command]
    void CmdGetLog()
    {
        if (!isServer)
        {
            return;
        }

        gameManager = GameObject.Find("GameManager");
        taskManager = gameManager.GetComponent<TaskLog>();

    }

    private void Begin()
    {
        // Play open animation
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
        // Play close animation
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        beginRequested = false;
        endRequested = true;

        canvas.DOKill(true);
        canvas.DOFade(0f, 0.25f);
        taskLog.transform.DOScale(0f, 0.4f).SetEase(Ease.InQuad);
    }

}
