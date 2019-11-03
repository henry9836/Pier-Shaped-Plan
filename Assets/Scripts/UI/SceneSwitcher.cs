using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using DG.Tweening;

public class SceneSwitcher : MonoBehaviour {

    public GameObject fadePanel;
    public float fadeTime;
    private float fadeInDelay = 0.25f;
    private Image fadeImage;
    public string targetScene;
    public string curScene;
    private bool isFading;
    private bool isSwitching;
    private float fadeTimeCur;

    private NetworkManager netman;

    public AudioClip clickSound;

    void Awake()
    {
        //fadePanel = GameObject.Find("FadePanel");

        curScene = SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: " + curScene);

        netman = FindObjectOfType<NetworkManager>();
    }

	// Use this for initialization
	void Start ()
    {
        if (curScene == "MainMenu")
        {
            //GlobalData.LastScene = curScene;
        }

        if (fadePanel == null)
        {
            fadePanel = GameObject.Find("SceneFader");
        }
        fadePanel.SetActive(true);
        fadeImage = fadePanel.GetComponent<Image>();

        Invoke("ExitFade", fadeInDelay);
    }
	
	// Update is called once per frame
	void Update ()
    {
        fadeTimeCur = Mathf.MoveTowards(fadeTimeCur, 0f, Time.deltaTime);
        if (fadeTimeCur != 0)
        {
            isFading = true;
        }
        else
        {
            isFading = false;
        }

        if (isSwitching && !isFading)
        {
            if (targetScene == "")
            {
                ExitFade();
            }

            if (targetScene == "Quit")
            {
                Application.Quit();
            }
            else if (targetScene == "HostGame")
            {
                netman.StartHost();
            }
            else 
            {
                SceneManager.LoadScene(targetScene);
                Debug.Log("Switched from " + curScene + " to " + targetScene);
            }

            isSwitching = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (curScene == "TitleScreen")
            {
                QuitGame();
            }
            else
            {
                SceneSwitch("TitleScreen");
            }
        }

    }

    public void SceneSwitch(string scene)
    {
        StartFade();
        targetScene = scene;
    }


    public void StartFade()
    {
        if (!isSwitching && !isFading)
        {
            Vector4 initialColor = fadeImage.color;
            fadeImage.DOFade(1, fadeTime / 1.0f).SetEase(Ease.InOutSine);

            isSwitching = true;
            fadeTimeCur = fadeTime;

            GetComponent<AudioSource>().clip = clickSound;
            GetComponent<AudioSource>().Play();
        }
    }

    public void ExitFade()
    {
        isFading = true;
        isSwitching = false;
        fadeTimeCur = fadeTime;
        Vector4 initialColor = fadeImage.color;
        fadeImage.DOFade(0, fadeTime).SetEase(Ease.InOutSine);
    }

    public void HostGame()
    {
        if (!netman.IsClientConnected() && !NetworkServer.active && netman.matchMaker == null)
        {
            StartFade();
            targetScene = "HostGame";
            Debug.Log("Hosting Game...");
        }
    }

    public void ConnectClient()
    {
        if (!netman.IsClientConnected() && !NetworkServer.active && netman.matchMaker == null)
        {
            StartFade();
            targetScene = "Connect";
            Debug.Log("Attempting to connect to host...");
        }
    }

    public void QuitGame()
    {
        StartFade();
        targetScene = "Quit";
        Debug.Log("Silly human. You know you can't quit the game from the editor!");
    }
}
