using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class PlayerCamera : MonoBehaviour {

    public Transform cameraTarget;
    public Transform cameraPivot;
    public GameObject player;
    [SerializeField] private float lerpSpeed = 12f;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 a = cameraPivot.position;
        Vector3 b = cameraTarget.position;
        Vector3 c = player.transform.position;

        cameraPivot.transform.position = Vector3.Lerp(a, b, lerpSpeed * Time.smoothDeltaTime);
        //transform.DOLookAt(b, 1f, AxisConstraint.None, Vector3.up);
	}

    public void Shake()
    {
        Vector3 b = cameraTarget.position;
        //transform.DOShakeRotation(3f, 15f, 7, 90f, true);
    }
}
