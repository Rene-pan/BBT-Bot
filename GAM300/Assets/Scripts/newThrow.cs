using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class newThrow : MonoBehaviour
{
    public LineRenderer lr;
    public Rigidbody rb; //need to assign throwable rigidbody
    public Transform Handarea;
    Vector3 startPosition;
    Vector3 startVelocity;
    public float InitialForce = 15;
    public float InitialAngle = -45;
    Quaternion rot;
    public int NumberOfPoints = 50;
    public float timer = 0.1f;
    public CamController_v3 cam;
    [SerializeField] GameObject player;
    [SerializeField] PlayerController playerScript;
    void Start()
    {
        //CreatePhysicsScene();
        lr = GetComponent<LineRenderer>();
        rot = Quaternion.Euler(InitialAngle,0, 0);
        cam = GameObject.Find("3rdPersonTopViewCam").GetComponent<CamController_v3>(); 

    }

    void Update()
    {
        //lr.transform.rotation = Quaternion.Euler(0, cam.transform.rotation.y, 0);
        if (cam.currentState == CamController_v3.CamState.OVERSHOULDER)
        {
            DrawLine(4f);
            lr.enabled = true;
        }
        else
        {
            lr.enabled = false;
        }
    }

    //public void drawline(float heightmultiplier)
    //{
    //    i = 0;
    //    lr.positionCount = NumberOfPoints;
    //    lr.enabled = true;
    //    startPosition = Handarea.position;
    //    //startVelocity = rot * (InitialForce * transform.forward) / rb.mass;
    //    startVelocity = (InitialForce * -player.transform.forward);
    //    lr.SetPosition(i, startPosition);
    //    for (float j = 0; i < lr.positionCount - 1; j += timer, i++)
    //    {
    //        //i++;
    //        Vector3 linePosition = startPosition + (j * startVelocity);
    //        //linePosition.y = startPosition.y + (startVelocity.y * j + 0.5f * Physics.gravity.y * j * j);
    //        linePosition.y = startPosition.y + (startVelocity.y * j + 0.5f * Physics.gravity.y * j * j) * heightmultiplier; 
    //        lr.SetPosition(i, linePosition);
    //    }
    //}

    public void DrawLine(float heightMultiplier)
    {
        lr.positionCount = NumberOfPoints; // Set number of points
        lr.enabled = true; // Enable LineRenderer

        Vector3 startPosition = Handarea.position; // Starting position of the throw
        Vector3 startVelocity = InitialForce * -player.transform.forward; // Initial velocity

        // Set the first point of the line
        lr.SetPosition(0, startPosition);

        // Calculate trajectory points
        for (int i = 1; i < NumberOfPoints; i++)
        {
            float t = i * timer; // Time increment
            Vector3 linePosition = startPosition + (startVelocity * t); // Horizontal motion

            // Vertical motion with gravity
            linePosition.y = startPosition.y + (startVelocity.y * t + 0.5f * Physics.gravity.y * t * t) * heightMultiplier;

            lr.SetPosition(i, linePosition); // Set the calculated position in the LineRenderer
        }
    }
    public void Throw()
    {
        Rigidbody thrownObject = Instantiate(rb.gameObject, Handarea.position, Quaternion.identity).GetComponent<Rigidbody>();
        //thrownObject.AddForce(rot * (InitialForce * transform.forward), ForceMode.Impulse);
        thrownObject.AddForce((InitialForce * -player.transform.forward), ForceMode.Impulse);
    }

   
}
