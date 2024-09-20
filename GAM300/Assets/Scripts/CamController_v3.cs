using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static System.TimeZoneInfo;

public class CamController_v3 : MonoBehaviour
{
    public enum CamState { THIRDPERSON, OVERSHOULDER };
    public CamState currentState = CamState.THIRDPERSON;

    [Header("Cam Functions Variables")]
    [SerializeField] Transform followTarget;
    [SerializeField] float mouseSensitivity = 10;
    [SerializeField] float distFromTarget = 2;
    [SerializeField] Vector2 pitchMinMax = new Vector2(10, 45);
    float yaw;
    float pitch;
    public LayerMask collisionMask;
    private bool pitchLock = false;
    Vector3 currentRotation;
    [SerializeField] float rotationSmoothTime;

    [Header("Change Cursor visibility")]
    int PressedV;

    [Header("Change Camera View")]
    int PressedB;
    Vector3 camStartPos;
    Vector3 camEndPos;
    bool transitionTrue;
    [SerializeField] Transform target; //Lerp towards this position before switch state
    [SerializeField] float transitionDuration;
    [SerializeField] GameObject RotateAxis;
    [SerializeField] GameObject Player;

    [Header("CollisionCheck")]
    public float collideSpeed = 5;
    public float returnSpeed = 9;
    public float wallPush = 0.7f;
    public float closestDistanceToPlayer = 2;
    public float evenCloserDistanceToPlayer = 1;

    private void Update()
    {
        ChangeCursorVisibility();
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
                OSCam();
                //this cam function, can look around but without the changing the transform?
                camEndPos = transform.position;
                //if press B key again, change state to Third person
                if (Input.GetKeyDown(KeyCode.B))
                {
                    PressedB = 1;
                    //RotateAxis.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    //RotateAxis.transform.rotation = Quaternion.Euler(0, 0, 0);
                    StartCoroutine(Transition(camEndPos, camStartPos, PressedB));
                }
                break;
        }

    }
    public void ChangeState(CamState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }
    //Lerp between Cams
    IEnumerator Transition(Vector3 startPos, Vector3 endPos, int PositionNo)
    {
        float t = 0.0f;
        //if transitioning, nothing will happen
        if (transitionTrue) yield return 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime * (Time.deltaTime / transitionDuration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transitionTrue = true;
            yield return 0;
        }
        switch (PositionNo)
        {
            case 0:
                ChangeState(CamState.OVERSHOULDER);
                break;
            case 1:
                ChangeState(CamState.THIRDPERSON);
                break;
        }
        transitionTrue = false;
    }
    //3rd Person Cam Stuff
    void CamFunctions()
    {
        RotateAxis.GetComponent<CamController_v2>().enabled = false;
        GetComponent<Camera>().enabled = true;
        target.GetComponent<Camera>().enabled = false;
        followTarget.GetComponent<Movement>().cam = this.gameObject.transform;
        Player.transform.localEulerAngles = new Vector3(0, 180, 0);
        RotateAxis.transform.localEulerAngles = new Vector3(0, 0, 0);
        if (followTarget != null && !transitionTrue)
        {
            //Parent(followTarget, this.gameObject, 1);
            CollisionCheck(followTarget.position - transform.forward * distFromTarget);
            if (!pitchLock)
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
                currentRotation = Vector3.Lerp(currentRotation, new Vector3(pitch, yaw), rotationSmoothTime * Time.deltaTime);
            }
            else
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch = pitchMinMax.y;
                currentRotation = Vector3.Lerp(currentRotation, new Vector3(pitch, yaw), rotationSmoothTime * Time.deltaTime);
            }
            transform.eulerAngles = currentRotation;
        }
    }
    void OSCam()
    {
        target.GetComponent<Camera>().enabled = true;
        RotateAxis.GetComponent<CamController_v2>().enabled = true;
        GetComponent<Camera>().enabled = false;
        followTarget.GetComponent<Movement>().cam = target;
        Player.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
    private void CollisionCheck(Vector3 retPoint)
    {
        RaycastHit hit;
        if (Physics.Linecast(followTarget.position, retPoint, out hit, collisionMask))
        {
            Vector3 norm = hit.normal * wallPush;
            Vector3 p = hit.point + norm;
            if (Vector3.Distance(Vector3.Lerp(transform.position, p, collideSpeed * Time.deltaTime), followTarget.position) <= evenCloserDistanceToPlayer)
            {

            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, p, collideSpeed * Time.deltaTime);
            }
            return;
        }
        transform.position = Vector3.Lerp(transform.position, retPoint, returnSpeed * Time.deltaTime);
        pitchLock = false;
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

}
