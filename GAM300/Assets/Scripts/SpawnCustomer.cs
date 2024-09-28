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
    [SerializeField] private int totalCustomers = 0;

    private void Start()
    {
        time = 0;
        Time.timeScale = 1;
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
                time = 0;
                currentCustomerCount+=1;
                totalCustomers+=1;
                var customer = Instantiate(Customers[currentCustomerID], gameObject.transform, true);
                Parent(gameObject.transform, customer, 1);
                customer.transform.position = gameObject.transform.position;
                customer.GetComponent<Customer>().UpdateChairLocation();
                customer.GetComponent<Customer>().customerSpawn = gameObject;
                StartCoroutine(SetandMoveToLocation(customer,1));
                currentCustomerID+=1;
            }
        }
        else if (totalCustomers == Customers.Count)
        {
            var Money = FindAnyObjectByType<Money>();
            Money.CheckMoney();
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
    IEnumerator SetandMoveToLocation(GameObject customer, float delay)
    {
        yield return new WaitForSeconds(delay);
        customer.GetComponent<Customer>().UpdateDestination();
    }
<<<<<<< Updated upstream
=======

    void CreateCustomer(GameObject Spawnedcustomer)
    {
        var customer = Instantiate(Spawnedcustomer, gameObject.transform);
        var customerScript = customer.GetComponent<Customer_v2>();
        Parent(gameObject.transform, customer, 1);
        //rmb to set the set spawn position as the last waypoint position
    }

>>>>>>> Stashed changes
}
