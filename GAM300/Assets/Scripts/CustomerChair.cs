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
    public GameObject Table;
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
                var customerScript = other.GetComponent<Customer_v2>();
                print("customer reached");
                var seatPivot = gameObject.transform.GetChild(1).transform;
                other.GetComponent<Rigidbody>().isKinematic = true;
                Parent(seatPivot, other.gameObject, 0);
                other.gameObject.transform.localPosition = Vector3.zero;
                other.gameObject.transform.localRotation = Quaternion.Euler(0, -90, 0);
                //customerScript.ChangeState(Customer.Customerstates.WAIT);
                //customerScript.NearestTable = Tables[ChairID].gameObject;
                //customerScript.KeepTrackOfOrders();
                //OrderCounter += 1;
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
