using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using static System.TimeZoneInfo;

public class CamController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
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
    //Transition values
    [SerializeField] float transitionDuration = 2.5f;
    [SerializeField] Transform target;
    [SerializeField] bool transitionTrue = false;
    Vector3 camStartPos;
    Vector3 camEndPos;

    public enum CamState {THIRDPERSON, OVERSHOULDER};
    [SerializeField] CamState currentState = CamState.THIRDPERSON;
    private void Update()
    {
        ChangeCursorVisibility();
        //CamViewToggle();
        switch (currentState) 
        {
            case CamState.THIRDPERSON:
                CamFunctions();
                //if press B key, change state to over shoulder
                if (Input.GetKeyDown(KeyCode.B))
                {
                    PressedB = 0;
                    camStartPos = transform.position;
                    StartCoroutine(Transition(camStartPos, target.position, PressedB));
                }
                break;
            case CamState.OVERSHOULDER:
                //this cam function, can look around but without the changing the transform?
                camEndPos = transform.position;
                CamFunctions_OS();
                //if press B key again, change state to Third person
                if (Input.GetKeyDown(KeyCode.B))
                {
                    PressedB = 1;
                    StartCoroutine(Transition(camEndPos, camStartPos, PressedB));
                }
                break;
        }

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
        if (followTarget != null && camEnabled && !transitionTrue)
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
    void CamFunctions_OS()
    {
        if (followTarget != null && camEnabled && !transitionTrue)
        {
            invertXVal = (invertX) ? -1 : 1;
            invertYVal = (invertY) ? -1 : 1;
            rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
            rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;
            var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
            //var focusPosition = new Vector3(framingOffset.x, framingOffset.y);
            //make sure camera is always behind player and how far it is looking at, Rotation By cameraAngle 
            //transform.position = focusPosition - Quaternion.Euler(rotationX, rotationY, 0) * new Vector3(0, 0, cameraZDistance);
            transform.rotation = targetRotation;
        }
    }
    void CamViewToggle()
    {
        if (Input.GetKeyDown(KeyCode.B) && PressedB == 0)
        {

            //StartCoroutine(Transition(camStartPos, target.position));
            PressedB = 1;

        }
        else if (Input.GetKeyDown(KeyCode.B) && PressedB == 1)
        {

            //StartCoroutine(Transition(target.position, camStartPos));
            PressedB = 0;

        }
    }
    IEnumerator Transition(Vector3 startPos, Vector3 endPos, int PositionNo)
    {
        print("Help1");
        float t = 0.0f;
        //if transitioning, nothing will happen
        if (transitionTrue) yield return 0;
        switch (PositionNo)
        {
            case 0:
                ChangeState(CamState.OVERSHOULDER);
                break;
            case 1:
                ChangeState(CamState.THIRDPERSON);
                break;
        }
        while (t < 1.0f)
        {
            print("Help3");
            transitionTrue = true;
            t += Time.deltaTime* (Time.deltaTime/transitionDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return 0;
        }
        transitionTrue = false;
        print("Help2");
    }
    public void ChangeState(CamState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }
}
