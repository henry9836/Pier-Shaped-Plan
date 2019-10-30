using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyScreen : MonoBehaviour
{
    public enum TabState
    {
        Join,
        Host
    }
    public TabState screenState;
    private int screenStateCur = -1;

    public GameObject[] screens;
    public InputField inputField;
    public Text addressField;
    public Text hostIPText;

    private CanvasGroup canvas;
    public AudioClip clickSound;

    private NetworkManager manager;
    private string hostAddress;

    void Start()
    {
        screenState = TabState.Join;
        screenStateCur = (int)TabState.Join;
        canvas = GetComponent<CanvasGroup>();
        manager = FindObjectOfType<NetworkManager>();
        manager.networkAddress = "localhost";
        inputField.text = manager.networkAddress;
        hostAddress = IPManager.GetIP(ADDRESSFAM.IPv4);
        hostIPText.text = hostAddress;
        //Debug.Log(hostAddress);
    }

    void Update()
    {
        // Game screens
        if ((int)screenState != screenStateCur)
        {
            // Deactivate old screen
            switch (screenStateCur)
            {
                case 0:
                    screens[screenStateCur].GetComponent<TabScreen>().isEnabled = false;
                    break;

                case 1:
                    screens[screenStateCur].GetComponent<TabScreen>().isEnabled = false;
                    break;

            }

            // Activate new screen
            switch (screenState)
            {
                case TabState.Join:
                    screens[(int)screenState].GetComponent<TabScreen>().isEnabled = true;
                    break;

                case TabState.Host:
                    screens[(int)screenState].GetComponent<TabScreen>().isEnabled = true;
                    break;
            }

            screenStateCur = (int)screenState;
        }

    }

    public void Connect()
    {
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            manager.networkAddress = addressField.text;
            manager.StartClient();
            Debug.Log(manager.networkAddress);
        }
    }

    public void Host()
    {
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            manager.StartHost();
        }
    }

    public void GoToScreen(int screen)
    {
        screenState = (TabState)screen;
        //GetComponent<AudioSource>().clip = clickSound;
        //GetComponent<AudioSource>().Play();
    }
}
