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
    Transform StartPosition;
    Vector3 startVelocity;
    public float InitialForce = 15;
    [SerializeField, Range(10, 100), Tooltip("The maximum number of points the LineRenderer can have")]
    int maxPoints = 50;
    [SerializeField, Range(0.01f, 0.5f), Tooltip("The time increment used to calculate the trajectory")]
    float increment = 0.025f;
    [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")]
    float rayOverlap = 1.1f;
    //public float InitialAngle = -45;
    //Quaternion rot;
    //public int NumberOfPoints = 50;
    //public float timer = 0.1f;
    public CamController_v3 cam;
    [SerializeField] GameObject player;
    //[SerializeField] PlayerController playerScript;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        //rot = Quaternion.Euler(InitialAngle,0, 0);
        cam = GameObject.Find("3rdPersonTopViewCam").GetComponent<CamController_v3>(); 

    }

    void Update()
    {
        Predict();
        lr.enabled = true;

        if (cam.currentState == CamController_v3.CamState.OVERSHOULDER)
        {
            //DrawLine(4f);
            lr.enabled = true;
        }
        else
        {
            //lr.enabled = false;
        }
    }
    //public void DrawLine(float heightMultiplier)
    //{
    //    lr.positionCount = NumberOfPoints; // Set number of points
    //    lr.enabled = true; // Enable LineRenderer

    //    Vector3 startPosition = Handarea.position; // Starting position of the throw
    //    Vector3 startVelocity = InitialForce * -player.transform.forward; // Initial velocity

    //    // Set the first point of the line
    //    lr.SetPosition(0, startPosition);

    //    // Calculate trajectory points
    //    for (int i = 1; i < NumberOfPoints; i++)
    //    {
    //        float t = i * timer; // Time increment
    //        Vector3 linePosition = startPosition + (startVelocity * t); // Horizontal motion

    //        // Vertical motion with gravity
    //        linePosition.y = startPosition.y + (startVelocity.y * t + 0.5f * Physics.gravity.y * t * t) * heightMultiplier;

    //        lr.SetPosition(i, linePosition); // Set the calculated position in the LineRenderer
    //    }
    //}

    void Predict()
    {
        PredictTrajectory(ProjectileData());
    }
    public void PredictTrajectory(ProjectileProperties projectile)
    {
        Vector3 newDirection = new Vector3(projectile.direction.x * -player.transform.forward.x, projectile.direction.y * -player.transform.forward.y, projectile.direction.z *- player.transform.forward.z);
        Vector3 velocity = newDirection * (projectile.initialSpeed / projectile.mass);
        Vector3 position = projectile.initialPosition;
        Vector3 nextPosition;
        float overlap;

        UpdateLineRender(maxPoints, (0, position));

        for (int i = 1; i < maxPoints; i++)
        {
            // Estimate velocity and update next predicted position
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            nextPosition = position + velocity * increment;

            // Overlap our rays by small margin to ensure we never miss a surface
            overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            //When hitting a surface we want to show the surface marker and stop updating our line
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap))
            {
                UpdateLineRender(i, (i - 1, hit.point));
                print("line off");
                //MoveHitMarker(hit);
                break;
            }

            //If nothing is hit, continue rendering the arc without a visual marker
            //hitMarker.gameObject.SetActive(false);
            position = nextPosition;
            UpdateLineRender(maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
        }
    }
    ProjectileProperties ProjectileData()
    {
        ProjectileProperties properties = new ProjectileProperties();
        Rigidbody r = rb.gameObject.GetComponent<Rigidbody>();

        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
        properties.initialSpeed = InitialForce;
        properties.mass = r.mass;
        properties.drag = r.drag;

        return properties;
    }

    public void Throw()
    {
        Rigidbody thrownObject = Instantiate(rb.gameObject, Handarea.position, Quaternion.identity).GetComponent<Rigidbody>();
        //thrownObject.AddForce(rot * (InitialForce * transform.forward), ForceMode.Impulse);
        thrownObject.AddForce((InitialForce * -player.transform.forward), ForceMode.Impulse);
    }

    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        lr.positionCount = count;
        lr.SetPosition(pointPos.point, pointPos.pos);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }


}
