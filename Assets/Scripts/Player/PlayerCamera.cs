using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour {

    public Transform cameraTarget;
    public Vector3 positionOffset;
    [SerializeField] private float lerpSpeed = 12f;

    void Start () 
    {

    }
	
	void Update () 
    {
        //Vector3 a = cameraPivot.position;
	}

    private void FixedUpdate()
    {
        if (cameraTarget != null)
        {
            Vector3 b = cameraTarget.position;

            transform.position = Vector3.Lerp(transform.position, b, lerpSpeed * Time.smoothDeltaTime);
            //transform.DOLookAt(b, 1f, AxisConstraint.None, Vector3.up);
        }
        
    }

    public void Shake()
    {
        Vector3 b = cameraTarget.position;
        //transform.DOShakeRotation(3f, 15f, 7, 90f, true);
    }
}
