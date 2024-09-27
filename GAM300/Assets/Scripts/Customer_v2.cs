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
    private Transform targetwaypoint;
    private int targetWaypointIndex;
    private float minDistance = 0.1f;
    private float lastWaypointIndex;
    public float movementSpeed = 3f;
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
                float distance = Vector3.Distance(transform.position, targetwaypoint.position);
                CheckDistanceToWPNMove(distance);
                transform.position = Vector3.MoveTowards(transform.position, targetwaypoint.position, movementStep);
                break;

        case CustomerStates.ORDER:
                break;

        case CustomerStates.WAIT:
                break;

        case CustomerStates.EAT:
                break;
        case CustomerStates.LEAVE:
                break;
        }
    }
    ///For movement
    void CheckDistanceToWPNMove(float currentDistance)
    {
        if (currentDistance <= minDistance) 
        {
            if (targetWaypointIndex != waypoints.Count -1)
            {
                targetWaypointIndex += 1;
            }
            targetwaypoint = waypoints[targetWaypointIndex];
        }
    }

    ///

}
