using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customer_v2 : MonoBehaviour
{
    public enum CustomerType { NORMAL, BIG, ANNOYING }
    public enum CustomerStates { MOVE, ORDER, WAIT, EAT, ANGRY, LEAVE }
    public CustomerType customerType;
    public CustomerStates currentState;

    [Header("Customer move")]
    public float spawnTime;
    public List<Transform> waypoints;
    private Transform targetwaypoint;
    private int targetWaypointIndex;
    private float minDistance = 0.1f;
    public float movementSpeed = 3f;
    public float rotationSpeed = 2f;
    public GameObject nearestTable;

    [Header("Customer Order")]
    [SerializeField] List<GameObject> OrderUI;//all the order UI that customer will generate
    [SerializeField] List<GameObject> OrderList; //all the genrated orderUI for this customer
    [SerializeField] int OrderUI_ID;
    [SerializeField] GameObject OrderUIHolder; //contains all orders
    [SerializeField] float OrderWaitTime;
    [SerializeField] float currentTime;
    [SerializeField] float DecreasingValue;

    [Header("Customer Angry")]
    [SerializeField] Material CustomerMaterial;
    [SerializeField] Color AngeredColor;
    private void OnEnable()
    {
        targetwaypoint = waypoints[targetWaypointIndex];
        OrderUIHolder = GameObject.Find("OrderList");
    }
    private void Update()
    {
        CustomerStateChange();
    }
    //For movement
    #region movement
    void CheckDistanceToWPNMove(float currentDistance)
    {
        if (currentDistance <= minDistance) 
        {
            targetWaypointIndex += 1;
            if (targetWaypointIndex == waypoints.Count)
            {
                targetWaypointIndex = 0;
            }
            targetwaypoint = waypoints[targetWaypointIndex];
        }
    }
    #endregion movement

    #region CustomerFSM
    void CustomerStateChange()
    {
        switch (currentState)
        {
            case CustomerStates.MOVE:
                float movementStep = movementSpeed * Time.deltaTime;
                float rotationStep = rotationSpeed * Time.deltaTime;
                Vector3 directionToTarget = targetwaypoint.position - transform.position;
                Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);
                //transform.LookAt(targetwaypoint);
                float distance = Vector3.Distance(transform.position, targetwaypoint.position);
                CheckDistanceToWPNMove(distance);
                transform.position = Vector3.MoveTowards(transform.position, targetwaypoint.position, movementStep);
                break;

            case CustomerStates.ORDER:
                KeepTrackOfOrders();
                break;

            case CustomerStates.WAIT:
                foreach (var order in OrderList)
                {
                    var orderTimer = order.GetComponent<Order>().DecreasingTimer;
                    StartTimer(orderTimer, DecreasingValue);
                }
                break;

            case CustomerStates.EAT:
                break;
            case CustomerStates.ANGRY:
                CustomerAngry();
                break;
            case CustomerStates.LEAVE:
                break;
        }
    }
    public void ChangeState(CustomerStates newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }
    #endregion

    #region CreateOrder
    GameObject CreateOrder()
    {
        var CreateNewOrder = Instantiate(OrderUI[OrderUI_ID], OrderUIHolder.transform);
        //start timer for that specific order in customer side
        var OrderScript = CreateNewOrder.GetComponent<Order>();
        SetTimer(OrderScript.DecreasingTimer, OrderWaitTime);
        OrderScript.addIngredientIcons();
        OrderScript.UpdateOrderName();
        //set order name to table
        nearestTable.GetComponent<CustomerTable>().FoodName = OrderScript.OrderName;
        nearestTable.GetComponent<CustomerTable>().customer = gameObject;
        nearestTable.GetComponent<CustomerTable>().orders.Add(CreateNewOrder);
        OrderUIHolder.GetComponent<OrderInfo>().numberOfOrders += 1;
        OrderUI_ID += 1;
        return CreateNewOrder;
    }

    public void KeepTrackOfOrders()
    {
        if (OrderUIHolder.GetComponent<OrderInfo>().numberOfOrders >= OrderUIHolder.GetComponent<OrderInfo>().maxOrders) return;
        if (OrderUI_ID == OrderUI.Count)
        {
            OrderUI_ID = 0;
            ChangeState(CustomerStates.WAIT);
        }
        else
        {
            OrderList.Add(CreateOrder());
        }
    }

    void StartTimer(Slider orderSlider, float DecreasingValue)
    {
        currentTime -= Time.deltaTime * DecreasingValue;
        orderSlider.value = currentTime;
        if (currentTime <= 0)
        {
            ChangeState(CustomerStates.ANGRY);
        }
        else if (nearestTable.GetComponent<CustomerTable>().CompletedMeal && currentTime > 0)
        {
            ChangeState(CustomerStates.EAT);
        }
    }
    void SetTimer(Slider orderSlider, float orderWaitTime)
    {
        orderSlider.maxValue = orderWaitTime;
        orderSlider.value = orderWaitTime;
        currentTime = orderWaitTime;
    }
    #endregion

    #region CustomerAngry
    public void CustomerAngry()
    {
        //customer turns red

        //customer deletes order, order flashes red
        foreach (var order in OrderList)
        {
            Destroy(order.gameObject);
        }
        OrderList.Clear();
        //movetowards the index
        ChangeState(CustomerStates.LEAVE);
    }
    #endregion

}
