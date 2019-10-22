using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour {

    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform playerBody;

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

        if (xAxisClamp > 90f)
        {
            xAxisClamp = 90f;
            mouseY = 0f;
            ClampXaxisRotationToValue(270f);
        }
        else if (xAxisClamp < -90f)
        {
            xAxisClamp = -90f;
            mouseY = 0f;
            ClampXaxisRotationToValue(90f);
        }

        //transform.RotateAround(transform.position, -transform.right, mouseY);
        transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void ClampXaxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
