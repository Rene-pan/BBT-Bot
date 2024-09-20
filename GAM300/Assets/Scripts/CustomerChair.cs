using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CustomerChair : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        var tag = other.tag;
        switch (tag) 
        {
            case "Customer":
                print("customer reached");
                var seatPivot = gameObject.transform.GetChild(0).transform;
                Parent(seatPivot, other.gameObject, 0);
                other.gameObject.transform.localPosition = Vector3.zero;
                other.GetComponent<Customer>().ChangeState(Customer.Customerstates.WAIT);
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
