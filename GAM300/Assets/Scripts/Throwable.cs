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
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) return;
        if (tag == "Customer")
        {
            var customerScript = collision.gameObject.GetComponent<Customer_v2>();
            if (customerScript.currentState == Customer_v2.CustomerStates.WAIT)
            {
                if (customerScript.customerType == Customer_v2.CustomerType.BIG || customerScript.customerType == Customer_v2.CustomerType.KAREN)
                {
                    AudioManager.instance.PlayOneShot(FmodEvents.instance.M_CustomerAngry, collision.gameObject.transform.position);
                }
                else if (customerScript.customerType == Customer_v2.CustomerType.NORMAL || customerScript.customerType == Customer_v2.CustomerType.ANNOYING)
                {
                    AudioManager.instance.PlayOneShot(FmodEvents.instance.F_CustomerAngry, collision.gameObject.transform.position);
                }
                AudioManager.instance.PlayOneShot(FmodEvents.instance.HitOtherAreas, this.transform.position);
                customerScript.ChangeState(Customer_v2.CustomerStates.ANGRY);
                Destroy(gameObject);
            }
        }
    }
}
