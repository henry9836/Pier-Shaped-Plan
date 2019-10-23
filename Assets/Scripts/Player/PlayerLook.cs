using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour {

    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform YRotRef;
    [SerializeField] private GameObject camRoot;

    public float verticalRotationValue;
    public float maxDistance = 6f;
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
        // Calculate vertical rotation value (0-1 float) where 0 is looking fully up and 1 is looking fully down
        Vector3 eRot = transform.localRotation.eulerAngles;
        verticalRotationValue = Mathf.DeltaAngle(eRot.x, 360f - minVerticalRotation) / -(minVerticalRotation + maxVerticalRotation);
        verticalRotationValue = Mathf.Clamp(verticalRotationValue, 0.1f, 1.0f);

        // Update camera distance
        if (camRoot != null)
        {
            camRoot.transform.localPosition = new Vector3(camRoot.transform.localPosition.x, camRoot.transform.localPosition.y, -maxDistance * Mathf.Pow(verticalRotationValue, 0.5f));

        }

        // Update camera rotation
        CameraRotation();
	}

    private void CameraRotation()
    {
        float mouseX = Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // Clamp vertical rotation
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
