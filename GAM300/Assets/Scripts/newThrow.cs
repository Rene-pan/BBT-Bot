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
    int i = 0;
    public int NumberOfPoints = 50;
    public float timer = 0.1f;
    public CamController_v3 cam;
    [SerializeField] GameObject player;
    [SerializeField] PlayerController playerScript;

    //private Scene _simulationScene;
    //private PhysicsScene _physicsScene;
    //[SerializeField] private Transform _obstaclesParent;
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
            drawline(4.5f);
            lr.enabled = true;
        }
        else
        {
            lr.enabled = false;
        }
        //if (Input.GetMouseButtonDown(1))
        //{
        //    drawline();
        //    lr.enabled = true;
        //}
        //else 
        //{
        //    lr.enabled = false;
        //}

    }

    public void drawline(float heightmultiplier)
    {
        i = 0;
        lr.positionCount = NumberOfPoints;
        lr.enabled = true;
        startPosition = Handarea.position;
        //startVelocity = rot * (InitialForce * transform.forward) / rb.mass;
        startVelocity = (InitialForce * -player.transform.forward);
        lr.SetPosition(i, startPosition);
        for (float j = 0; i < lr.positionCount - 1; j += timer, i++)
        {
            //i++;
            Vector3 linePosition = startPosition + (j * startVelocity);
            //linePosition.y = startPosition.y + (startVelocity.y * j + 0.5f * Physics.gravity.y * j * j);
            linePosition.y = startPosition.y + (startVelocity.y * j + 0.5f * Physics.gravity.y * j * j * heightmultiplier);
            lr.SetPosition(i, linePosition);
        }
    }

    //void CreatePhysicsScene()
    //{
    //    _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
    //    foreach (Transform obj in _obstaclesParent)
    //    {
    //        var ghostObject = Instantiate(obj.gameObject, obj.position, obj.rotation);
    //        if (ghostObject.GetComponent<Renderer>() != null)
    //        {
    //            ghostObject.GetComponent<Renderer>().enabled = false;
    //        }
    //        SceneManager.MoveGameObjectToScene(ghostObject, _simulationScene);
    //    }
    //}
    //public void drawline(GameObject Throwable, Vector3 pos, Vector3 velocity)
    //{
    //    var ghostObj = Instantiate(Throwable, pos, Quaternion.identity);
    //    ghostObj.GetComponent<Renderer>().enabled = false;
    //    //ghostObj.
    //}

    public void Throw()
    {
        Rigidbody thrownObject = Instantiate(rb.gameObject, Handarea.position, Quaternion.identity).GetComponent<Rigidbody>();
        //thrownObject.AddForce(rot * (InitialForce * transform.forward), ForceMode.Impulse);
        thrownObject.AddForce((InitialForce * -player.transform.forward), ForceMode.VelocityChange);
    }

   
}
