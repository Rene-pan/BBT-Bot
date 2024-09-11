using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using static System.TimeZoneInfo;

public class CamController_v1 : MonoBehaviour
{
    public enum CamState { THIRDPERSON, OVERSHOULDER };
    [SerializeField] CamState currentState = CamState.THIRDPERSON;
    [Header("Cam Vars")]
    [SerializeField] Transform followTarget;
    [SerializeField] float mouseSensitivity = 10;
    [SerializeField] float distFromTarget = 2;
    [SerializeField] Vector2 pitchMinMax = new Vector2(10, 45);

    [SerializeField] float rotationSmoothTime = 8f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;
    float yaw;
    float pitch;
    int PressedV = 0;
    int PressedB = 0;
    [SerializeField] float transitionDuration = 2.5f;
    [SerializeField] Transform target;
    [SerializeField] bool transitionTrue = false;
    Vector3 camStartPos;
    Vector3 camEndPos;

    [Header("Collision Vars")]

    [Header("Transparency")]
    public bool changeTransparency = true;
    public MeshRenderer targetRenderer;

    [Header("Speeds")]
    public float collideSpeed = 5;
    public float returnSpeed = 9;
    public float wallPush = 0.7f;

    [Header("Distances")]
    public float closestDistanceToPlayer = 2;
    public float evenCloserDistanceToPlayer = 1;

    [Header("Mask")]
    public LayerMask collisionMask;
    private bool pitchLock = false;

    [Header("Aiming Var")]
    public Vector3 playerOffset;
    public float distanceFromOffset;
    public Transform rotator;
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
        if (followTarget != null && !transitionTrue)
        {
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
    //B key to toggle Cam View Perspective
    void CamFunctions_OS()
    {
        if (transitionTrue) return;
        if (Input.GetMouseButtonDown(1))
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.y);
        }
        else if (Input.GetMouseButton(1))
        {
            print("oh so liddat");
        }
        else
        {
            rotator.eulerAngles = new Vector3(0, rotator.eulerAngles.y, rotator.eulerAngles.z);
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
            t += Time.deltaTime * (Time.deltaTime / transitionDuration);
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

    //Draw gizmos 
    private void OnDrawGizmos()
    {
        Vector3 r = target.TransformPoint(playerOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(r, 0.1f);
    }

    private void WallCheck()
    {
        Ray ray = new Ray(transform.position, -transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.5f, out hit, 0.7f, collisionMask))
        {
            pitchLock = true;
        }
        else
        {
            pitchLock = false;
        }
    }
    private void CollisionCheck(Vector3 retPoint)
    {
        RaycastHit hit;
        if (Physics.Linecast (followTarget.position, retPoint, out hit, collisionMask))
        {
            Vector3 norm = hit.normal * wallPush;
            Vector3 p = hit.point + norm;
            //TransparencyCheck();
            if (Vector3.Distance(Vector3.Lerp(transform.position, p, collideSpeed * Time.deltaTime), followTarget.position) <= evenCloserDistanceToPlayer)
            {

            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, p, collideSpeed * Time.deltaTime);
            }
            return;
        }
        //FullTransparency();
        transform.position = Vector3.Lerp(transform.position, retPoint, returnSpeed * Time.deltaTime);
        pitchLock = false;
    }
    //private void TransparencyCheck()
    //{
    //    if (changeTransparency) 
    //    {
    //        if (Vector3.Distance(transform.position, followTarget.position) <= closestDistanceToPlayer)
    //        {
    //            Color temp = targetRenderer.sharedMaterial.color;
    //            temp.a = Mathf.Lerp(temp.a, 0.2f, collideSpeed * Time.deltaTime);
    //            targetRenderer.sharedMaterial.color = temp;
    //        }
    //        else
    //        {
    //            if (targetRenderer.sharedMaterial.color.a <= 0.99f)
    //            {
    //                Color temp = targetRenderer.sharedMaterial.color;
    //                temp.a = Mathf.Lerp(temp.a, 1, collideSpeed * Time.deltaTime);
    //                targetRenderer.sharedMaterial.color = temp;
    //            }
    //        }
    //    }
    //}
    //private void FullTransparency()
    //{
    //    if (changeTransparency) 
    //    {
    //        if (targetRenderer.sharedMaterial.color.a <= 0.99f)
    //        {
    //            Color temp = targetRenderer.sharedMaterial.color;
    //            temp.a = Mathf.Lerp(temp.a, 0.2f, collideSpeed * Time.deltaTime);
    //            targetRenderer.sharedMaterial.color = temp;
    //        }
    //    }
    //}
}

