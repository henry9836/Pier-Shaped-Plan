using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private string sceneCur;

    // Start is called before the first frame update
    void Start()
    {
        sceneCur = SceneManager.GetActiveScene().name;

        DontDestroyOnLoad(this.gameObject);

        if (sceneCur == "MainMenu")
        {

        }

        if (sceneCur == "Game")
        {
            //Destroy(this.gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        sceneCur = SceneManager.GetActiveScene().name;

        if (sceneCur == "Game")
        {
            //Destroy(this.gameObject);
            //Debug.Log("jldlsad");
        }
    }
}
