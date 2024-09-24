using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newThrow : MonoBehaviour
{
    LineRenderer lr;
    public Rigidbody rb; //need to assign throwable rigidbody
    public Transform Handarea;
    Vector3 startPosition;
    Vector3 startVelocity;
    public float InitialForce = 15;
    public float InitialAngle = -45;
    Quaternion rot;
    int i = 0;
    public int NumberOfPoints = 50;
    public float timer = 0.1f;
    public CamController_v3 cam;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        cam = GameObject.Find("3rdPersonTopViewCam").GetComponent<CamController_v3>(); 

    }

    // Update is called once per frame
    void Update()
    {
        rot = Quaternion.Euler(InitialAngle, 0, 0);
        if (cam.currentState == CamController_v3.CamState.THIRDPERSON) return;
        if (Input.GetMouseButton(0))
        {
            drawline();
            lr.enabled = true;
        }
        else
        {
            lr.enabled = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            lr.enabled=false;
        }

    }

    private void drawline()
    {
        i = 0;
        lr.positionCount = NumberOfPoints;
        lr.enabled = true;
        startPosition = Handarea.position;
        startVelocity = rot * (InitialForce * transform.forward) / rb.mass;
        lr.SetPosition(i, startPosition);
        for (float j = 0; i < lr.positionCount - 1; j += timer)
        {
            i++;
            Vector3 linePosition = startPosition + j * startVelocity;
            linePosition.y = startPosition.y + startVelocity.y * j + 0.5f * Physics.gravity.y * j * j;
            lr.SetPosition(i, linePosition);
        }
    }

    public void Throw()
    {
        Rigidbody thrownObject = Instantiate(rb.gameObject, Handarea.position, Quaternion.identity).GetComponent<Rigidbody>();
        thrownObject.AddForce(rot * (InitialForce * transform.forward), ForceMode.Impulse);
    }

   
}
