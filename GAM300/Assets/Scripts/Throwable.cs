using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    public string Name;
    public GameObject eatCanvas;
    private void Start()
    {
        this.name = Name;
    }
    private void OnCollisionEnter(Collision collision)
    {
        var tag = collision.gameObject.tag;
        if (tag == "Customer")
        {
            var customerScript = collision.gameObject.GetComponent<Customer_v2>();
            if (customerScript.currentState != Customer_v2.CustomerStates.MOVE || customerScript.currentState != Customer_v2.CustomerStates.LEAVE || customerScript.currentState != Customer_v2.CustomerStates.ANGRY || customerScript.currentState != Customer_v2.CustomerStates.EAT)
            {
                customerScript.ChangeState(Customer_v2.CustomerStates.ANGRY);
                Destroy(gameObject);
            }
        }
    }
}
