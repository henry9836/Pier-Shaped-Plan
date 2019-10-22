using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour {

    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform YRotRef;

    public float maxVerticalRotation = 60f;
    public float minVerticalRotation = 50f;
    private float xAxisClamp;

    private void Awake()
    {
        LockCursor();
        xAxisClamp = 0;
    }

	void Start ()
    {
		
	}
	
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

	void Update ()
    {
        CameraRotation();
	}

    private void CameraRotation()
    {
        float mouseX = Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        xAxisClamp += mouseY;

        if (xAxisClamp > minVerticalRotation)
        {
            xAxisClamp = minVerticalRotation;
            mouseY = 0f;
            ClampXaxisRotationToValue(360f - minVerticalRotation);
        }
        else if (xAxisClamp < -maxVerticalRotation)
        {
            xAxisClamp = -maxVerticalRotation;
            mouseY = 0f;
            ClampXaxisRotationToValue(maxVerticalRotation);
        }

        //transform.RotateAround(transform.position, -transform.right, mouseY);
        transform.Rotate(Vector3.left * mouseY);
        YRotRef.Rotate(Vector3.up * mouseX);
    }

    private void ClampXaxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
