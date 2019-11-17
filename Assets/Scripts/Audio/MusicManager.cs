using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private string sceneCur;
    private AudioSource audioSource;

    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private bool menuMusicPlaying;
    private bool gameMusicPlaying;

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

        if (sceneCur == "MainMenu")
        {
            if (!menuMusicPlaying)
            {
                audioSource.clip = menuMusic;
                audioSource.Play();
                menuMusicPlaying = true;
            }

            if (gameMusicPlaying)
            {
                audioSource.Stop();
                gameMusicPlaying = false;
            }
        }

        if (sceneCur == "Game")
        {
            if (menuMusicPlaying)
            {
                audioSource.Stop();
                menuMusicPlaying = false;
            }

            if (!gameMusicPlaying)
            {
                audioSource.clip = gameMusic;
                audioSource.Play();
                gameMusicPlaying = true;
            }
        }

        Debug.Log(sceneCur);
    }
}
