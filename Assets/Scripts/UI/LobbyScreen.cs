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

    public GameObject[] screens;
    public Text addressField;
    public Text hostIPText;

    private CanvasGroup canvas;
    public AudioClip clickSound;

    private NetworkManager manager;
    private string hostAddress;

    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        manager = FindObjectOfType<NetworkManager>();
        manager.networkAddress = "localhost";
        hostAddress = IPManager.GetIP(ADDRESSFAM.IPv4);
        hostIPText.text = hostAddress;
        //Debug.Log(hostAddress);
    }

    void Update()
    {
    }

    public void Connect()
    {
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            manager.StartClient();
            manager.networkAddress = addressField.text;

        }
    }

    public void Host()
    {
        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            manager.StartHost();
        }
    }
}
