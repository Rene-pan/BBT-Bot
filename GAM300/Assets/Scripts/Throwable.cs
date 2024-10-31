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
                AudioManager.instance.PlayOneShot(FmodEvents.instance.HitOtherAreas, this.transform.position);
                customerScript.ChangeState(Customer_v2.CustomerStates.ANGRY);
                Destroy(gameObject);
            }
        }
    }
}
