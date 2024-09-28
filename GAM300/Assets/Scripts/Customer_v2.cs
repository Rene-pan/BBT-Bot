using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    //[SerializeField] Material CustomerMaterial;
    //[SerializeField] Color AngeredColor;
    [SerializeField] List<Sprite> Espressions; //this variable is shared with Customer EAT state
    [SerializeField] GameObject EmotionHolder; //this variable is shared with Customer EAT state

    [Header("Customer Leave")]
    [SerializeField] List<Transform> waypointsBack;
    [SerializeField] Transform Exitdoor;
    public Transform targetwaypoint_back;
    public int targetWaypoint_backIndex = 0;
    private int startIndex = 0;
    private float minDistance_back = 0.01f;
    private void OnEnable()
    {
        targetwaypoint = waypoints[targetWaypointIndex];
        OrderUIHolder = GameObject.Find("OrderList");
        Exitdoor = GameObject.Find("Spawner").transform;
    }
    private void Update()
    {
        CustomerStateChange();
    }
    //For movement
    #region movement
    void CheckDistanceToWPNMove(float currentDistance)
    {
        if (currentState == CustomerStates.LEAVE)
        {
            //print(currentDistance);
            if (currentDistance <= minDistance_back)
            print(targetWaypoint_backIndex);
            {
                targetWaypoint_backIndex += 1;
                if (targetWaypoint_backIndex == waypointsBack.Count)
                {
                    targetWaypoint_backIndex = waypointsBack.Count;
                }
                targetwaypoint_back = waypointsBack[targetWaypoint_backIndex];
                print("help"+ targetWaypoint_backIndex);
            }
        }
        else
        {
            if (currentDistance <= minDistance)
            {
                targetWaypointIndex += 1;
                if (targetWaypointIndex == waypoints.Count)
                {
                    print(targetWaypointIndex);
                    targetWaypointIndex = 0;
                }
                targetwaypoint = waypoints[targetWaypointIndex];
                print(targetWaypointIndex);
            }
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
                CustomerLeave();
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
        OrderUI_ID += 1;
        return CreateNewOrder;
    }

    public void KeepTrackOfOrders()
    {
        if (OrderUIHolder.GetComponent<OrderInfo>().numberOfOrders >= OrderUIHolder.GetComponent<OrderInfo>().maxOrders) return;
        if (OrderUI_ID == OrderUI.Count)
        {
            OrderUI_ID = 0;
            OrderUIHolder.GetComponent<OrderInfo>().numberOfOrders += 1;
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
        EmotionHolder.SetActive(true);
        EmotionHolder.GetComponent<Image>().sprite = Espressions[0];
        //customer deletes order, order flashes red
        foreach (var order in OrderList)
        {
            Destroy(order.gameObject);
        }
        OrderList.Clear();
        //add the waypoints to go back;
        for (int i = waypoints.Count - 1; i >= 0; i--)
        {
            waypointsBack.Add(waypoints[i]);
        }
        waypointsBack.Add(Exitdoor);
        targetwaypoint_back = waypointsBack[startIndex];
        //GetComponent<Rigidbody>().isKinematic = false;
        //movetowards the index
        ChangeState(CustomerStates.LEAVE);
    }
    #endregion
    #region 
    //customer moves back to spawn position
    //add the waypoints from back to the front
    public void CustomerLeave()
    {
        float movementStep = movementSpeed * Time.deltaTime;
        float rotationStep = rotationSpeed * Time.deltaTime;
        print(targetwaypoint_back.name);
        Vector3 directionToTarget = targetwaypoint_back.position- transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);
        float distance = Vector3.Distance(transform.position, targetwaypoint_back.position);
        transform.position = Vector3.MoveTowards(transform.position, targetwaypoint_back.position, movementStep);
        CheckDistanceToWPNMove(distance);
        //transform.LookAt(targetwaypoint_back);
        print("help"+targetwaypoint_back.name);
    }

    #endregion

}
