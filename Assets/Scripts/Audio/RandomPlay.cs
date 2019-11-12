using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RandomPlay : NetworkBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();
    [SyncVar]
    public bool randomAutoplay = true;
    [SyncVar]
    public float timer = 0.0f;
    public float threshold = 7.0f;
    public Vector2 thresholdRange = new Vector2(4.0f, 20.0f);

    public void PlayRandomAudio()
    {
        CmdPlay(Random.Range(0, audioClips.Count));
    }

    public void PlayAudio(int track)
    {
        CmdPlay(track);
    }

    [Command]
    void CmdPlay(int track)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = audioClips[track];
        GetComponent<AudioSource>().Play();
        RpcPlay(track);
    }

    [ClientRpc]
    void RpcPlay(int track)
    {
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = audioClips[track];
        GetComponent<AudioSource>().Play();
    }

    private void Start()
    {
        threshold = Random.Range(thresholdRange.x, thresholdRange.y);
    }

    void FixedUpdate()
    {
       if (!isServer)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer > threshold)
        {
            PlayRandomAudio();
            threshold = Random.Range(thresholdRange.x, thresholdRange.y);
            timer = 0.0f;
        }

    }

}
