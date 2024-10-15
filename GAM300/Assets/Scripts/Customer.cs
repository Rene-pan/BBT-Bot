using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Video;
using static Customer;

public class Customer : MonoBehaviour
{
    public enum Customerstates { ORDER, WAIT, EAT, ANGRY, LEAVE }
    [SerializeField] Customerstates currentstate = Customerstates.ORDER;
    [Header("Customer Movement")]
    NavMeshAgent agent;
    public List<Transform> waypoints;
    int waypointIndex;
    Vector3 target;
    public GameObject[] chairs;
    public int CustomerID;

    [Header("Customer Orders")]
    [SerializeField] GameObject[] OrderUI;
    [SerializeField] int OrderUI_ID;
    [SerializeField] GameObject OrderUIHolder;
    [SerializeField] List<GameObject> OrderList;
    [SerializeField] float OrderWaitTime;
    [SerializeField] float currentTime;
    [SerializeField] float DecreasingValue;
    public GameObject NearestTable;

    [Header("Customer Leaves")]
    [SerializeField] Material CustomerMaterial;
    [SerializeField] Color AngeredColor;
    private float org_agentSpeed;
    public GameObject customerSpawn;

    [Header("Customer Eats")]
    public GameObject Food;
    [SerializeField] float EatingSpeedMultiplier;
    [SerializeField] float EatingDuration;
    public float currentEatTime;
    public GameObject OrderToDelete;
    public int CustomerMoney;
    public Money MoneyScript; 

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        org_agentSpeed = agent.speed;
        currentEatTime = 0;
        MoneyScript = FindFirstObjectByType<Money>();
    }
    private void Update()
    {
        switch (currentstate)
        {
            case Customerstates.ORDER:
                if (Vector3.Distance(transform.position, target) < 1f)
                {
                    UpdateDestination();
                }
                break;
            case Customerstates.WAIT:
                agent.speed = 0;
                foreach(var order in OrderList)
                {
                    var orderTimer = order.GetComponent<Order>().DecreasingTimer;
                    StartTimer(orderTimer, DecreasingValue);
                }
                break;
            case Customerstates.EAT:
                CustomerEats(EatingSpeedMultiplier, EatingDuration);
                break; 
            case Customerstates.ANGRY:
                CustomerAngry();
                break;
            case Customerstates.LEAVE:
                agent.speed = org_agentSpeed;
                if (Vector3.Distance(transform.position, target) < 1f)
                {
                    UpdateDestination();
                    CustomerLeaves();
                }
                break;
        }
    }
    public void ChangeState(Customerstates newState)
    {
        if (currentstate != newState)
        {
            currentstate = newState;
        }
    }
    public void UpdateDestination()
    {
        target = waypoints[waypointIndex].position;
        agent.SetDestination(target);
    }
    public void IterateWayPointIndex()
    {
        waypointIndex++;
        if (waypointIndex == waypoints.Count)
        {
            waypointIndex = 0;
        }
    }
    GameObject CreateOrder()
    {
        var CreateNewOrder = Instantiate(OrderUI[OrderUI_ID], OrderUIHolder.transform);
        //start timer for that specific order in customer side
        SetTimer(CreateNewOrder.GetComponent<Order>().DecreasingTimer, OrderWaitTime);
        var OrderScript = CreateNewOrder.GetComponent<Order>();
        OrderScript.addIngredientIcons();
        //OrderScript.UpdateOrderName();
        //set order name to table
        NearestTable.GetComponent<CustomerTable>().FoodName = OrderScript.OrderName;
        NearestTable.GetComponent<CustomerTable>().customer = gameObject;
        NearestTable.GetComponent<CustomerTable>().orders.Add(CreateNewOrder);
        return CreateNewOrder;
    }
    void StartTimer(Slider orderSlider, float DecreasingValue)
    {
        currentTime -= Time.deltaTime * DecreasingValue;
        orderSlider.value = currentTime;
        if (currentTime <= 0)
        {
            ChangeState(Customerstates.ANGRY);
        }
         else if (NearestTable.GetComponent<CustomerTable>().CompletedMeal && currentTime > 0)
        {
            ChangeState(Customerstates.EAT);
        }
    }
    void SetTimer(Slider orderSlider, float orderWaitTime)
    {
        orderSlider.maxValue = orderWaitTime;
        orderSlider.value = orderWaitTime;
        currentTime = orderWaitTime;
    }

    public void KeepTrackOfOrders()
    {
        OrderList.Add(CreateOrder());
    }

    public void UpdateChairLocation()
    {
        chairs = GameObject.FindGameObjectsWithTag("Chair");
        foreach (var chair in chairs)
        {
            if (chair.GetComponent<CustomerChair>().ChairID == CustomerID)
            {
                waypoints.Add(chair.transform);
                break;
            }
        }
        waypoints.Add(GameObject.FindWithTag("SpawnPoint").transform);
        OrderUIHolder = GameObject.Find("OrderList");
    }
    public void CustomerAngry()
    {
        //customer turns red
        CustomerMaterial = gameObject.GetComponent<MeshRenderer>().material;
        CustomerMaterial.SetColor("_Color", AngeredColor);
        //customer deletes order, order flashes red
        foreach (var order in OrderList)
        {
            Destroy(order.gameObject);
        }
        OrderList.Clear();
        //change waypointIndex
        IterateWayPointIndex();
        agent.ResetPath();
        UpdateDestination();
        //movetowards the index
        ChangeState(Customerstates.LEAVE);
    }

    public void CustomerEats(float EatingSpeedMultiplier, float eatduration)
    {
        if (OrderToDelete != null)
        {
            OrderToDelete.GetComponent<Image>().color = Color.green;
            Destroy(OrderToDelete, 2);
        }
        currentEatTime += Time.deltaTime * EatingSpeedMultiplier;
        if (currentEatTime >= eatduration)
        {
            //delete food on table
            Destroy(Food);
            //add money
            MoneyScript.AddMoney(CustomerMoney);
            //change waypointIndex
            IterateWayPointIndex();
            agent.ResetPath();
            UpdateDestination();
            currentEatTime = 0;
            //movetowards the index
            ChangeState(Customerstates.LEAVE);
        }
    }

    void CustomerLeaves()
    {
        //decrease current customer count by 1
        customerSpawn.GetComponent<SpawnCustomer>().currentCustomerCount -= 1;
        //destroys this customer
        Destroy(gameObject);
    }

    //public static void ForceCleanupNavMesh()
    //{
    //    if (Application.isPlaying)
    //        return;

    //    NavMesh.RemoveAllNavMeshData();
    //}
}
