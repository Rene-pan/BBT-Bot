using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] Camera OverShoulderCam;
    [SerializeField] float cameraZDistance;//bigger = viewport will be bigger, smaller = viewport will be smaller
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float minVerticalAngle = 10;
    [SerializeField] float maxVerticalAngle = 45;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    bool camEnabled = true;
    float rotationX;
    float rotationY;
    float invertXVal;
    float invertYVal;
    int PressedV = 0;
    int PressedB = 0;
    [SerializeField] Vector2 framingOffset;
    private void Update()
    {
        ChangeCursorVisibility();
        CamFunctions();
        CamViewToggle();
    }

    //V Key to toggle Cursor visibility
    void ChangeCursorVisibility()
    {
        if (Input.GetKeyDown(KeyCode.V) && PressedV == 0)
        {
            PressedV = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetKeyDown(KeyCode.V) && PressedV == 1)
        {
            PressedV = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    //3rd Person Cam Stuff
    void CamFunctions()
    {

        if (followTarget != null && camEnabled)
        {
            invertXVal = (invertX) ? -1 : 1;
            invertYVal = (invertY) ? -1 : 1;
            rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
            rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;
            var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
            var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);
            //make sure camera is always behind player and how far it is looking at, Rotation By cameraAngle 
            transform.position = focusPosition - Quaternion.Euler(rotationX, rotationY, 0) * new Vector3(0, 0, cameraZDistance);
            transform.rotation = targetRotation;
        }
    }
    //B key to toggle Cam View Perspective
    void CamViewToggle()
    {
        if (Input.GetKeyDown(KeyCode.B) && PressedB == 0)
        {
            OverShoulderCam.enabled = false;
            GetComponent<Camera>().enabled = true;
            PressedB = 1;
        }
        else if (Input.GetKeyDown(KeyCode.B) && PressedB == 1)
        {
            OverShoulderCam.enabled = true;
            GetComponent<Camera>().enabled = false;
            PressedB = 0;
        }
    }
}
