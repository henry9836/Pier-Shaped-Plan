using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour {

    public Transform cameraTarget;
    private GameObject camPivot;
    private GameObject camRoot;
    private Camera mainCam;

    private string mouseXInputName = "Mouse X", mouseYInputName = "Mouse Y";
    [SerializeField] private float mouseSensitivity = 180f;

    [SerializeField] private float lerpSpeed = 12f;
    private float zPosLerp;
    public AnimationCurve distCurve;
    public AnimationCurve FOVCurve;
    private float pitchValue, pitchValueAdj, zOffset, hitDistance;
    public float maxDistance = 5f;
    public float maxPitchDown = 60f;
    public float maxPitchUp = 50f;
    public float minFOV = 60f;
    public float maxFOV = 75f;
    private float xAxisRot;

    [SerializeField] LayerMask obstacleLayers;
    private CameraCollision camCollision;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        xAxisRot = 0;
        camPivot = transform.GetChild(0).gameObject;
        camRoot = transform.GetChild(0).GetChild(0).gameObject;
        mainCam = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Camera>();
        camCollision = camRoot.GetComponent<CameraCollision>();
    }

    void Update () 
    {
        // Calculate vertical rotation value (0-1 float) where 0 is pitch up and 1 is pitch down
        Vector3 eRot = camPivot.transform.localRotation.eulerAngles;
        pitchValue = Mathf.DeltaAngle(eRot.x, 270f) / -180f;
        pitchValueAdj = Mathf.DeltaAngle(eRot.x, 360f - maxPitchUp) / -(maxPitchUp + maxPitchDown);
        pitchValue = Mathf.Clamp(pitchValue, 0.0f, 1.0f);

        // Update camera distance
        if (camRoot != null)
        {
            zOffset = Mathf.Lerp(1f, maxDistance, distCurve.Evaluate(pitchValueAdj)) + camCollision.distOffset;
            float zPos = Mathf.Clamp(zOffset, 1f, hitDistance);
            zPosLerp = Mathf.Lerp(zPosLerp, zPos, Time.deltaTime * lerpSpeed * 1.75f);
            camRoot.transform.localPosition = new Vector3(camRoot.transform.localPosition.x, camRoot.transform.localPosition.y, -zPosLerp);
        }

        // Update camera FOV
        mainCam.fieldOfView = Mathf.Lerp(minFOV, maxFOV, FOVCurve.Evaluate(1-pitchValueAdj));

        // Update camera rotation
        CameraRotation();

        ObstacleCheck();


    }

    private void FixedUpdate()
    {
        if (cameraTarget != null)
        {
            Vector3 b = cameraTarget.position;
            // Smooth camera follow target
            transform.position = Vector3.Lerp(transform.position, b, Time.smoothDeltaTime * lerpSpeed);
        }
        
    }

    private void CameraRotation()
    {
        float mouseX = Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // Clamp and smooth vertical rotation
        xAxisRot += mouseY;
        xAxisRot = Mathf.LerpAngle(xAxisRot, xAxisRot + mouseY, Time.deltaTime * lerpSpeed);

        if (xAxisRot > maxPitchUp)
        {
            xAxisRot = maxPitchUp;
            mouseY = 0f;
            ClampXaxisRotationToValue(360f - maxPitchUp);
        }
        else if (xAxisRot < -maxPitchDown)
        {
            xAxisRot = -maxPitchDown;
            mouseY = 0f;
            ClampXaxisRotationToValue(maxPitchDown);
        }
        
        camPivot.transform.Rotate(Vector3.left * mouseY);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void ClampXaxisRotationToValue(float value)
    {
        Vector3 eulerRotation = camPivot.transform.eulerAngles;
        eulerRotation.x = Mathf.Lerp(eulerRotation.x, value, Time.deltaTime * lerpSpeed);
        camPivot.transform.eulerAngles = eulerRotation;
    }

    private void ObstacleCheck()
    {
        Vector3 origin = camPivot.transform.position + camPivot.transform.right * 0.8f + camPivot.transform.up * 1f;
        Vector3 direction = -camRoot.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, zOffset, obstacleLayers))
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
            hitDistance = hit.distance;
        }
        else
        {
            hitDistance = zOffset;
        }

        Debug.Log(hitDistance);
    }

    public void Shake()
    {
        Vector3 b = cameraTarget.position;
        transform.DOShakeRotation(3f, 15f, 7, 90f, true);
    }

}
