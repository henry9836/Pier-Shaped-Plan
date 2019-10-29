using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCamera : MonoBehaviour 
{

    public Transform cameraTarget;
    private GameObject camPivot;
    private GameObject camRoot;
    private Camera mainCam;

    private string mouseXInputName = "Mouse X", mouseYInputName = "Mouse Y";
    [SerializeField] private float mouseSensitivity = 180f;

    [SerializeField] private float lerpSpeed = 12f;
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

    private bool isAiming;
    public float aimFOV = 55f;
    public float aimDistance = 3f;
    private float fovLerp, zOffsetLerp;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        camPivot = transform.GetChild(0).gameObject;
        camRoot = transform.GetChild(0).GetChild(0).gameObject;
        mainCam = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Camera>();
    }

    void Update () 
    {
        if (Input.GetAxis("Fire2") > 0.5f)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }


        // Update camera rotation
        CameraRotation();

        // Maintain line of sight with centre point next to player
        ObstacleCheck();

        // Calculate vertical rotation value (0-1 float) where 0 is pitch up and 1 is pitch down
        Vector3 eRot = camPivot.transform.localRotation.eulerAngles;
        pitchValue = Mathf.DeltaAngle(eRot.x, 270f) / -180f;
        pitchValueAdj = Mathf.DeltaAngle(eRot.x, 360f - maxPitchUp) / -(maxPitchUp + maxPitchDown);
        pitchValue = Mathf.Clamp(pitchValue, 0.0f, 1.0f);

        // Update camera FOV
        float fov = Mathf.Lerp(minFOV, maxFOV, FOVCurve.Evaluate(1 - pitchValueAdj));
        float fovAim = isAiming ? aimFOV : fov;
        fovLerp = Mathf.Lerp(fovLerp, fovAim, Time.smoothDeltaTime * lerpSpeed);
        float fovFinal = isAiming ? fovLerp : fov;
        mainCam.fieldOfView = fov;

        // Update camera distance
        if (camRoot != null)
        {
            zOffset = Mathf.Lerp(2f, maxDistance, distCurve.Evaluate(pitchValueAdj));
            float zOffsetColl = Mathf.Clamp(zOffset, 1f, hitDistance);
            float zOffsetAim = isAiming ? aimDistance : zOffsetColl;
            zOffsetLerp = Mathf.Lerp(zOffsetLerp, zOffsetAim, Time.smoothDeltaTime * lerpSpeed);
            camRoot.transform.localPosition = new Vector3(camRoot.transform.localPosition.x, camRoot.transform.localPosition.y, -zOffsetColl);
        }
    }

    private void FixedUpdate()
    {
        if (cameraTarget != null)
        {
            // Smooth camera follow target
            transform.position = Vector3.Lerp(transform.position, cameraTarget.position + cameraTarget.transform.up, Time.smoothDeltaTime * lerpSpeed);
        }
    }

    private void CameraRotation()
    {
        float mouseX = Input.GetAxisRaw(mouseXInputName) * mouseSensitivity * Time.smoothDeltaTime;
        float mouseY = Input.GetAxisRaw(mouseYInputName) * mouseSensitivity * Time.smoothDeltaTime;

        // Clamp and smooth vertical rotation
        xAxisRot += mouseY;

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
        eulerRotation.x = Mathf.Lerp(eulerRotation.x, value, Time.smoothDeltaTime * lerpSpeed);
        camPivot.transform.eulerAngles = eulerRotation;
    }

    private void ObstacleCheck()
    {
        Vector3 origin = camPivot.transform.position + camPivot.transform.right * 0.0f + camPivot.transform.up * 0.0f;
        Vector3 direction = -camPivot.transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, zOffset, obstacleLayers))
        {
            //Debug.DrawRay(origin, direction * hit.distance, Color.yellow);
            hitDistance = hit.distance;
        }
        else
        {
            hitDistance = zOffset;
        }
    }

    public void Shake()
    {
        Vector3 b = cameraTarget.position;
        transform.DOShakeRotation(3f, 15f, 7, 90f, true);
    }

}