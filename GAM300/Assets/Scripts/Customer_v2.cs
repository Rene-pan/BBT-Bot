using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_v2 : MonoBehaviour
{
    public enum CustomerType { NORMAL, BIG, ANNOYING }
    public enum CustomerStates { MOVE, ORDER, WAIT, EAT, ANGRY, LEAVE }
    public CustomerType customerType;
    public CustomerStates currentState;

    [Header("Customer move")]
    public float spawnTime;
    public List<Transform> waypoints;
    public List<Transform> waypointsToDoor;
    private Transform targetwaypoint;
    private int targetWaypointIndex;
    private float minDistance = 0.1f;
    private float lastWaypointIndex;
    public float movementSpeed = 3f;
    public float rotationSpeed = 2.0f;
    public GameObject NearestTable;
    private void OnEnable()
    {
        targetwaypoint = waypoints[targetWaypointIndex];
    }
    private void Update()
    {
        CustomerStateChange();
    }
    void CustomerStateChange()
    {
        switch (currentState) 
        {
        case CustomerStates.MOVE:
                float movementStep = movementSpeed * Time.deltaTime;
                float rotationStep = rotationSpeed * Time.deltaTime;
                Vector3 directionToTarget = targetwaypoint.position - targetwaypoint.position;
                Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);
                float distance = Vector3.Distance(transform.position, targetwaypoint.position);
                CheckDistanceToWPNMove(distance);
                //print(distance);
                transform.position = Vector3.MoveTowards(transform.position, targetwaypoint.position, movementStep);
                break;

        case CustomerStates.ORDER:
                break;

        case CustomerStates.WAIT:
                break;

        case CustomerStates.EAT:
                //after eating, set the targetwaypoint to another array of waypoints
                break;
        case CustomerStates.LEAVE:
                float movementStep_ = movementSpeed * Time.deltaTime;
                float rotationStep_ = rotationSpeed * Time.deltaTime;
                Vector3 directionToTarget_ = targetwaypoint.position - targetwaypoint.position;
                Quaternion rotationToTarget_ = Quaternion.LookRotation(directionToTarget_);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget_, rotationStep_);
                float distance_ = Vector3.Distance(transform.position, targetwaypoint.position);
                CheckDistanceToWPNMove(distance_);
                //print(distance);
                transform.position = Vector3.MoveTowards(transform.position, targetwaypoint.position, movementStep_);
                break;
        }
    }
    ///For movement
    void CheckDistanceToWPNMove(float currentDistance)
    {
        if (currentDistance <= minDistance) 
        {
            targetWaypointIndex += 1;
            if (targetWaypointIndex == waypoints.Count)
            {
                targetWaypointIndex = 0;
            }
            targetwaypoint = waypoints[targetWaypointIndex];
        }
    }
    public void ChangeState(CustomerStates newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }

    ///

}
