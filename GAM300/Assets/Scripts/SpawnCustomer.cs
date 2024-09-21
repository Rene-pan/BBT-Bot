using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnCustomer : MonoBehaviour
{
    public List<GameObject> Customers;
    public int currentCustomerID = 0;
    public int currentCustomerCount = 0;
    public float SpawnDelay = 10;
    public float time;
    public int maxShopCapacity;

    private void Start()
    {
        time = 0;
    }
    private void Update()
    {
        CustomerSpawn();
    }
    void CustomerSpawn()
    {
        if (currentCustomerCount < maxShopCapacity)
        {
            time += Time.deltaTime;
            if (time >= SpawnDelay)
            {
                var customer = Instantiate(Customers[currentCustomerID], gameObject.transform, true);
                Parent(gameObject.transform, customer, 1);
                time = 0;
                currentCustomerID++;
                currentCustomerCount++;
            }
        }
        else if (currentCustomerCount == Customers.Count) return;

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
