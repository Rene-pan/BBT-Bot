using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OS_Cam : MonoBehaviour
{
     float idealDistance;
    public float zoomInSpeed;
    void Start()
    {
        idealDistance = Vector3.Distance(transform.parent.position, transform.position);
    }
    void Update()
    {
        Vector3 target = transform.parent.position + transform.parent.up;
        if (Physics.Raycast(target, transform.position - target, out RaycastHit hit, idealDistance))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target + (transform.position - target).normalized * idealDistance, zoomInSpeed * Time.deltaTime);
        }
    }

}
