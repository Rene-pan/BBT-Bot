using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float cameraZDistance;//bigger = viewport will be bigger, smaller = viewport will be smaller
    [SerializeField] float minVerticalAngle = 10;
    [SerializeField] float maxVerticalAngle = 45;
    float rotationX;
    float rotationY;

    private void Update()
    {
        if (followTarget != null)
        {
            rotationX += Input.GetAxis("Mouse Y");
            rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);
            rotationY += Input.GetAxis("Mouse X");
            var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
            //make sure camera is always behind player and how far it is looking at, Rotation By cameraAngle 
            transform.position = followTarget.position - Quaternion.Euler(rotationX, rotationY, 0) * new Vector3(0, 0, cameraZDistance);
            transform.rotation = targetRotation;
        }
    }
}
