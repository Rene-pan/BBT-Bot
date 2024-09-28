using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnCustomer : MonoBehaviour
{
    public List<GameObject> Customers;
    public List<Transform> chairList = new List<Transform>();//chairs
    public List<GameObject> tableList = new List<GameObject>(); //tables
    public int currentCustomerID = 0;
    public int currentCustomerCount = 0;
    public float currentCustomerSpawnDelay;
    public float time;
    public int maxShopCapacity;
    [SerializeField] private int totalCustomers = 0;
    public bool canSpawn = false;
    public GameObject currentChair;

    private void Awake()
    {
        PopulateChairwayPoints();
    }
    private void Start()
    {
        time = 0;
        Time.timeScale = 1;
    }
    private void Update()
    {
        if (currentCustomerCount < maxShopCapacity)
        {
            ChairAvailability();
            if (canSpawn)
            {
                CustomerSpawnTimes(Customers[currentCustomerID]);
                print(Customers[currentCustomerID].name);
            }
        }
        else if (totalCustomers == Customers.Count)
        {
            canSpawn = false;
            //var Money = FindAnyObjectByType<Money>();
            //Money.CheckMoney();
        }
    }

    //check everytime for an available chair once
    //once there is an available chair, set it to unavailable
    //check if the customer timer has reached its own spawn delay time
    //if yes spawn the customer at the spawnpoint position
    //set its waypoints to the chair waypoints then make the customer move only once they have the waypoints in their script
    //
    public float count = 0;
    void ChairAvailability()
    {
        if (count > 0) return;
        foreach (var chair in chairList)
        {
            if (chair.GetComponent<CustomerChair>().currentState == CustomerChair.ChairState.AVAILABLE)
            {
                currentChair = chair.gameObject;
                count += 1;
                chair.GetComponent<CustomerChair>().currentState = CustomerChair.ChairState.UNAVAILABLE;
                canSpawn = true;
                break;
            }
        }
    }
    void CustomerSpawnTimes(GameObject customer)
    {
        var customerScript = customer.GetComponent<Customer_v2>();
        currentCustomerSpawnDelay = customerScript.spawnTime;
        CustomerSpawn(currentCustomerSpawnDelay, customer);
        //set the chair position(start) and end position in customer
        
    }
    //take the specific spawn time for the specific type of customer
    void CustomerSpawn(float CustomerSpawnTime, GameObject customer)
    {
        time += Time.deltaTime;
        if (time >= CustomerSpawnTime)
        {
            var customerScript = customer.GetComponent<Customer_v2>();
            if (customerScript.waypoints != null)
            {
                customerScript.waypoints.Clear();
                foreach (var waypoint in currentChair.GetComponent<CustomerChair>().waypoints)
                {
                    customerScript.waypoints.Add(waypoint);
                }
            }
            CreateCustomer(customer);
            //customer.transform.position = gameObject.transform.position;
            //customer.GetComponent<Customer>().UpdateChairLocation();
            //customer.GetComponent<Customer>().customerSpawn = gameObject;
            currentCustomerCount += 1;
            totalCustomers += 1;
            time = 0;
            currentCustomerID += 1;
            count = 0;
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
    void PopulateChairwayPoints()
    {
        if (chairList != null)
        {
            var chairs = GameObject.FindGameObjectsWithTag("Chair");
            foreach (var chair in chairs)
            {
                chairList.Add(chair.transform);
            }
        }
        if (tableList != null)
        {
            var tables = GameObject.FindGameObjectsWithTag("Table");
            foreach (var table in tables)
            {
                tableList.Add(table);
            }
        }
    }
<<<<<<< HEAD
=======

    void CreateCustomer(GameObject Spawnedcustomer)
    {
        var customer = Instantiate(Spawnedcustomer, gameObject.transform, true);
        var customerScript = customer.GetComponent<Customer_v2>();
        Parent(gameObject.transform, customer, 1);
    }

>>>>>>> parent of 930e8ed (Merge branch 'leveldesign_joanne' into Customer_Rene)
}
