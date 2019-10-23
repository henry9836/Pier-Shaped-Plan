using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour {

    public Transform cameraTarget;
    [SerializeField] private float lerpSpeed = 12f;

    [SerializeField] private string mouseXInputName = "Mouse X", mouseYInputName = "Mouse Y";
    [SerializeField] private float mouseSensitivity = 180f;
    [SerializeField] private GameObject camPivot;
    [SerializeField] private GameObject camRoot;

    public AnimationCurve distCurve;
    public AnimationCurve pitchCurve;
    private float pitchValue, pitchValueAdj;
    private Vector3 pitchLerpTarget;
    public float maxDistance = 6f;
    public float maxPitchDown = 60f;
    public float maxPitchUp = 50f;
    private float xAxisRot;

    [SerializeField] LayerMask obstacleLayers;
    private CameraCollision camCollision;


    private void Awake()
    {
        LockCursor();
        xAxisRot = 0;
        camCollision = camRoot.GetComponent<CameraCollision>();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update () 
    {
        // Calculate vertical rotation value (0-1 float) where 0 is looking fully up and 1 is looking fully down
        Vector3 eRot = camPivot.transform.localRotation.eulerAngles;
        pitchValue = Mathf.DeltaAngle(eRot.x, 270f) / -180f;
        pitchValueAdj = Mathf.DeltaAngle(eRot.x, 360f - maxPitchUp) / -(maxPitchUp + maxPitchDown);
        pitchValue = Mathf.Clamp(pitchValue, 0.0f, 1.0f);

        // Update camera distance
        if (camRoot != null)
        {
            float zOffset = -maxDistance * distCurve.Evaluate(pitchValue) + camCollision.distOffset;
            //zOffset = Mathf.Clamp(zOffset, 0.5f, maxDistance);
            camRoot.transform.localPosition = new Vector3(camRoot.transform.localPosition.x, camRoot.transform.localPosition.y, zOffset);
        }

        // Update camera rotation
        CameraRotation();
    }

    private void FixedUpdate()
    {
        if (cameraTarget != null)
        {
            Vector3 b = cameraTarget.position;

            transform.position = Vector3.Lerp(transform.position, b, lerpSpeed * Time.smoothDeltaTime);
        }
        
    }

    private void CameraRotation()
    {
        float mouseX = Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // Clamp vertical rotation
        xAxisRot += mouseY;
        //xAxisRot = Mathf.LerpAngle(xAxisRot, xAxisRot + mouseY, Time.deltaTime);

        if (xAxisRot > maxPitchUp)
        {
            xAxisRot = Mathf.LerpAngle(xAxisRot, maxPitchUp, Time.deltaTime * 12f);
            mouseY = 0f;
            ClampXaxisRotationToValue(360f - maxPitchUp);
        }
        else if (xAxisRot < -maxPitchDown)
        {
            xAxisRot = Mathf.LerpAngle(xAxisRot, -maxPitchDown, Time.deltaTime * 12f);
            mouseY = 0f;
            ClampXaxisRotationToValue(maxPitchDown);
        }
        
        camPivot.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ClampXaxisRotationToValue(float value)
    {
        Vector3 eulerRotation = camPivot.transform.eulerAngles;
        eulerRotation.x = Mathf.Lerp(eulerRotation.x, value, Time.deltaTime * 12f);
        camPivot.transform.eulerAngles = eulerRotation;
    }


    public void Shake()
    {
        Vector3 b = cameraTarget.position;
        //transform.DOShakeRotation(3f, 15f, 7, 90f, true);
    }

}
