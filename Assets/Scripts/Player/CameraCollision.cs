using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public bool isColliding;
    public bool playerBlocked;

    private float maxDistance;
    public float distOffset;
    private int curWhileIter = 0;

    public float timeUnblocked;

    // Start is called before the first frame update
    void Start()
    {
        maxDistance = -transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (isColliding || playerBlocked)
        {
            timeUnblocked = 0f;
        }
        else
        {
            timeUnblocked += Time.deltaTime;
        }

    }

    private void FixedUpdate()
    {
        //ObstacleCheck();
        
    }

    private void ObstacleCheck()
    {
        float os = distOffset;

        if (playerBlocked || isColliding)
        {
            distOffset += Time.deltaTime;
            distOffset = Mathf.Clamp(distOffset, 0f, maxDistance - 1f);
        }
        else
        {
            distOffset = Mathf.Lerp(distOffset, 0f, Time.deltaTime);
            distOffset = Mathf.Clamp(distOffset, 0f, maxDistance - 1f);

            if (curWhileIter > 0)
            {
                Debug.Log(curWhileIter + " Loops");
                curWhileIter = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9)
        {
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9)
        {
            isColliding = false;
        }
    }
}
