using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Customer;

public class Customer : MonoBehaviour
{
    public enum Customerstates { ORDER, WAIT, EAT, ANGRY, LEAVE }
    [SerializeField] Customerstates currentstate = Customerstates.ORDER;
    NavMeshAgent agent;
    public Transform[] waypoints;
    int waypointIndex;
    Vector3 target;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        UpdateDestination();
    }
    private void Update()
    {
        switch (currentstate)
        {
            case Customerstates.ORDER:
                if (Vector3.Distance(transform.position, target) < 1.5f)
                {
                    UpdateDestination();
                }
                break;
            case Customerstates.WAIT:
                agent.speed = 0;
                break;
            case Customerstates.EAT:
                break; 
            case Customerstates.ANGRY:
                break;
            case Customerstates.LEAVE:
                break;
        }
    }
    public void ChangeState(Customerstates newState)
    {
        if (currentstate != newState)
        {
            currentstate = newState;
        }
    }
    void UpdateDestination()
    {
        target = waypoints[waypointIndex].position;
        agent.SetDestination(target);
    }
    public void IterateWayPointIndex()
    {
        waypointIndex++;
        if (waypointIndex ==  waypoints.Length)
        {
            waypointIndex = 0;
        }
    }

    //public static void ForceCleanupNavMesh()
    //{
    //    if (Application.isPlaying)
    //        return;

    //    NavMesh.RemoveAllNavMeshData();
    //}
}
