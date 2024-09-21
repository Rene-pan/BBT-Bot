using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerChair : MonoBehaviour
{
    public int ChairID;
    public GameObject[] Tables;
    private int OrderCounter = 0;
    public int OrderCount = 1;
    private void OnTriggerEnter(Collider other)
    {
        var tag = other.tag;
        switch (tag) 
        {
            case "Customer":
                if (OrderCounter >= OrderCount) return;
                var customerScript = other.GetComponent<Customer>();
                print("customer reached");
                var seatPivot = gameObject.transform.GetChild(0).transform;
                Parent(seatPivot, other.gameObject, 0);
                other.gameObject.transform.localPosition = Vector3.zero;
                customerScript.ChangeState(Customer.Customerstates.WAIT);
                customerScript.NearestTable = Tables[ChairID].gameObject;
                customerScript.KeepTrackOfOrders();
                OrderCounter += 1;
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
