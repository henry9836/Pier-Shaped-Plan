using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneSwitcher : MonoBehaviour {

    public GameObject fadePanel;
    public float fadeTime;
    private float fadeInDelay = 0.25f;
    private Image fadeImage;
    private string targetScene;
    public string curScene;
    private bool isFading;
    private bool isSwitching;
    private float fadeTimeCur;

    public AudioClip clickSound;

    void Awake()
    {
        //fadePanel = GameObject.Find("fadePanel");

        curScene = SceneManager.GetActiveScene().name;
        Debug.Log(curScene);
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
        //ExitFade();
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
            else
            {
                SceneManager.LoadScene(targetScene);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (curScene == "MainMenu")
            {
                Application.Quit();
            }
            else
            {
                SceneSwitch("MainMenu");
            }
        }

    }

    public void SceneSwitch(string scene)
    {
        if (!isSwitching && !isFading)
        {
            Vector4 initialColor = fadeImage.color;
            fadeImage.DOFade(1, fadeTime / 1.0f).SetEase(Ease.InOutSine);
            fadeTimeCur = fadeTime;
            targetScene = scene;
            isSwitching = true;

            GetComponent<AudioSource>().clip = clickSound;
            GetComponent<AudioSource>().Play();
        }
    }


    public void StartFade()
    {
        if (!isSwitching)
        {
            Vector4 initialColor = fadeImage.color;
            fadeImage.DOFade(1, fadeTime / 1.0f).SetEase(Ease.InOutSine);
        }
        isSwitching = true;
        fadeTimeCur = fadeTime;
    }

    public void ExitFade()
    {
        isFading = true;
        isSwitching = false;
        fadeTimeCur = fadeTime;
        Vector4 initialColor = fadeImage.color;
        fadeImage.DOFade(0, fadeTime).SetEase(Ease.InOutSine);
    }

    public void EnterMainMenu()
    {
        StartFade();
        targetScene = "MainMenu";
    }
}
