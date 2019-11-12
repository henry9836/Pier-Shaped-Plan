using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private string sceneCur;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        sceneCur = SceneManager.GetActiveScene().name;
        audioSource = GetComponent<AudioSource>();

        DontDestroyOnLoad(this.gameObject);

        if (sceneCur == "MainMenu")
        {

        }

        if (sceneCur == "Game")
        {
            //Destroy(this.gameObject);
            audioSource.Stop();
            Debug.Log("this should be working");
        }

    }

    // Update is called once per frame
    void Update()
    {
        sceneCur = SceneManager.GetActiveScene().name;

        if (sceneCur == "Game")
        {
            audioSource.Stop();
            Debug.Log("this should be working");
        }

        Debug.Log(sceneCur);
    }
}
