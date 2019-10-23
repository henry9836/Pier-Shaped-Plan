using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public bool isColliding;
    public bool playerBlocked;

    public float timeUnblocked;

    // Start is called before the first frame update
    void Start()
    {
        
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 9)
        {
        }
            isColliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 9)
        {
        }
            isColliding = false;
    }
}
