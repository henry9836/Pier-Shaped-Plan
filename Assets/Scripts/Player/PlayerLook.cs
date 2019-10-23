using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour {

    [SerializeField] private string mouseXInputName, mouseYInputName;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform YRotRef;
    [SerializeField] private GameObject camRoot;

    public AnimationCurve distCurve;
    private float pitchValue, pitchValueAdj;
    public float maxDistance = 6f;
    public float maxPitchDown = 60f;
    public float maxPitchUp = 50f;
    private float xAxisClamp;

    [SerializeField] LayerMask obstacleLayers;
    private bool cameraIsColliding;
    private bool playerBlocked;

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
        pitchValue = Mathf.DeltaAngle(eRot.x, 270f) / -180f;
        pitchValueAdj = Mathf.DeltaAngle(eRot.x, 360f - maxPitchUp) / -(maxPitchUp + maxPitchDown);
        pitchValue = Mathf.Clamp(pitchValue, 0.0f, 1.0f);

        // Update camera distance
        if (camRoot != null)
        {
            //camRoot.transform.localPosition = new Vector3(camRoot.transform.localPosition.x, camRoot.transform.localPosition.y, -maxDistance * Mathf.Pow(verticalRotationValue, 0.5f));
            camRoot.transform.localPosition = new Vector3(camRoot.transform.localPosition.x, camRoot.transform.localPosition.y, -maxDistance * distCurve.Evaluate(pitchValue));

        }

        // Update camera rotation
        CameraRotation();

        // Check camera for obstacles
        ObstacleCheck();
	}

    private void CameraRotation()
    {
        float mouseX = Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // Clamp vertical rotation
        xAxisClamp += mouseY;

        if (xAxisClamp > maxPitchUp)
        {
            xAxisClamp = maxPitchUp;
            mouseY = 0f;
            ClampXaxisRotationToValue(360f - maxPitchUp);
        }
        else if (xAxisClamp < -maxPitchDown)
        {
            xAxisClamp = -maxPitchDown;
            mouseY = 0f;
            ClampXaxisRotationToValue(maxPitchDown);
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

    private void ObstacleCheck()
    {

    }
}
