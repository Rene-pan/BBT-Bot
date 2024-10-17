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
    public int currentCustomerID = 0;
    public int currentCustomerCount = 0;
    public float currentCustomerSpawnDelay;
    public float time;
    public int maxShopCapacity;
    [SerializeField] private int totalCustomers = 0;
    public bool canSpawn = false;
    public GameObject currentChair;
    public OrderInfo orderInfo;
    public List<GameObject> kopiMakers;
    public List<GameObject> toasters;
    public List<GameObject> kayaStations;

    private void Awake()
    {
        PopulateChairwayPoints();
        Addlist(kopiMakers, "kopiMakers");
        Addlist(toasters, "toastMakers");
        Addlist(kayaStations, "kayaStations");

        //AudioManager.instance.StopAllSounds();
    }
    private void Start()
    {
        time = 0;
        Time.timeScale = 1;
        //reset the stopOnce for the audio
        var Money = FindAnyObjectByType<Money>();
        Money.StopOnce = true;
        Money.StopSuccessMusic();
    }
    private void Update()
    {
        var Money = FindAnyObjectByType<Money>();
        Money.CheckMoney();
        CheckKopiMachine();
        if (currentCustomerCount < maxShopCapacity)
        {
            ChairAvailability();
            if (canSpawn && totalCustomers <= Customers.Count)
            {
                //print(totalCustomers);
                //print (Customers.Count);
                //print(Customers[currentCustomerID].name);
                CustomerSpawnTimes(Customers[currentCustomerID]);
                //print(Customers[currentCustomerID].name);
            }
        }
        else if (totalCustomers == Customers.Count)
        {
            canSpawn = false;
            Money.CheckMoney();
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
        customerScript.nearestTable = currentChair.GetComponent<CustomerChair>().Table;
        CustomerSpawn(currentCustomerSpawnDelay, customer);
        //set the chair position(start) and end position in customer

    }
    //take the specific spawn time for the specific type of customer
    void CustomerSpawn(float CustomerSpawnTime, GameObject customer)
    {
        time += Time.deltaTime;
        if (time >= CustomerSpawnTime)
        {
            totalCustomers += 1;
            currentCustomerCount += 1;
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
    }
    void CreateCustomer(GameObject Spawnedcustomer)
    {
        var customer = Instantiate(Spawnedcustomer, gameObject.transform);
        var customerScript = customer.GetComponent<Customer_v2>();
        customerScript.nearestTable.GetComponent<CustomerTable>().customer = customer;
        Parent(gameObject.transform, customer, 1);
    }
    private void OnTriggerEnter(Collider other)
    {
        var tag = other.tag;
        switch (tag)
        {
            case "Customer":
                if (other.GetComponent<Customer_v2>().currentState != Customer_v2.CustomerStates.LEAVE) return;
                Destroy(other.gameObject);
                currentCustomerCount -= 1;
                //orderInfo.numberOfOrders -= 1;
                break;
        }
    }
    void Addlist(List<GameObject> gameObjects, string tag)
    {
        if (gameObjects != null)
        {
            var gameobj = GameObject.FindGameObjectsWithTag(tag);
            foreach (var gameObj in gameobj)
            {
                gameObjects.Add(gameObj);
            }
        }
    }
    public bool NoOfKopiMakerBusy = true;
    //as long as either one of the kopi maker is ready, it will be not busy
    public bool ToastersBusy = true;
    public bool KayaStationsBusy = true;
    void CheckKopiMachine()
    {
        NoOfKopiMakerBusy = true;
        ToastersBusy = true;
        KayaStationsBusy = true;
        foreach (var kopimaker in kopiMakers)
        {
            if (kopimaker.transform.GetChild(0).GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.READY)
            {
                //if any maker is free
                NoOfKopiMakerBusy= false;
            }
        }
        foreach (var toaster in toasters)
        {
            if (toaster.transform.GetChild(0).GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.READY)
            {
                //if any maker is free
                ToastersBusy = false;
            }
        }
        foreach (var kayaStation in kayaStations)
        {
            if (kayaStation.transform.GetChild(0).GetComponent<SpreadKaya>().currentState == SpreadKaya.KayaMakerStates.READY)
            {
                //if any maker is free
                KayaStationsBusy = false;
            }
        }
    }
}
