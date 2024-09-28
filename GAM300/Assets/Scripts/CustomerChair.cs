using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CustomerChair : MonoBehaviour
{
    public enum ChairState {AVAILABLE, UNAVAILABLE}
    public ChairState currentState = ChairState.AVAILABLE;
    public int ChairID;
    public GameObject[] Tables;
    private int OrderCounter = 0;
    public int OrderCount = 1;
    public List<Transform> waypoints;
    private void OnTriggerEnter(Collider other)
    {
        var tag = other.tag;
        switch (tag) 
        {
            case "Customer":
                if (OrderCounter >= OrderCount) return;
                var customerScript = other.GetComponent<Customer>();
                print("customer reached");
<<<<<<< HEAD
                var seatPivot = gameObject.transform.GetChild(0).transform.GetChild(0).transform;
                Parent(seatPivot, other.gameObject, 0);
                other.gameObject.transform.localPosition = Vector3.zero;
                other.gameObject.transform.localRotation = Quaternion.Euler(0, -90, 0);
                print(other.transform.localPosition);

                other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                customerScript.ChangeState(Customer.Customerstates.WAIT);
                customerScript.NearestTable = Tables[ChairID].gameObject;
                customerScript.KeepTrackOfOrders();
                OrderCounter += 1;
=======
                //var seatPivot = gameObject.transform.GetChild(0).transform.GetChild(0).transform;
                //Parent(seatPivot, other.gameObject, 0);
                //other.gameObject.transform.localPosition = Vector3.zero;
                //other.gameObject.transform.localRotation = Quaternion.Euler(0, -90, 0);
                //print(other.transform.localPosition);
                //customerScript.ChangeState(Customer.Customerstates.WAIT);
                //customerScript.NearestTable = Tables[ChairID].gameObject;
                //customerScript.KeepTrackOfOrders();
                //OrderCounter += 1;
>>>>>>> parent of 930e8ed (Merge branch 'leveldesign_joanne' into Customer_Rene)
                //Parent(seatPivot, other.gameObject, 1);
                break;
        
        }

    }
    private void Parent(Transform Parent, GameObject child, int state)
    {
        switch (state)
        {
            case 0:
                child.transform.SetParent(Parent);
                break;
            case 1:
                child.transform.SetParent(null);
                break;
        }
    }
}
