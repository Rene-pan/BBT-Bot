using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Transform[] waypoints;
    int waypointIndex;
    Vector3 target;
    public List<GameObject> chairs;
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


    private void OnEnable()
    {
        foreach (var chair in chairs)
        {
            if (chair.GetComponent<CustomerChair>().ChairID == CustomerID)
            {
                waypoints[0] = chair.transform;
                break;
            }
        }
        waypoints[1] = GameObject.FindWithTag("SpawnPoint").transform;
        UpdateDestination();
        OrderUIHolder = GameObject.Find("OrderList");
    }
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
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
                break; 
            case Customerstates.ANGRY:
                break;
            case Customerstates.LEAVE:
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
    void UpdateDestination()
    {
        target = waypoints[CustomerID].position;
        agent.SetDestination(target);
    }
    public void IterateWayPointIndex()
    {
        waypointIndex++;
        if (waypointIndex ==  waypoints.Length)
        {
            waypointIndex = 0;
        }
    }
    GameObject CreateOrder()
    {
        var CreateNewOrder = Instantiate(OrderUI[OrderUI_ID], OrderUIHolder.transform);
        //start timer for that specific order in customer side
        SetTimer(CreateNewOrder.GetComponent<Order>().DecreasingTimer, OrderWaitTime);
        CreateNewOrder.GetComponent<Order>().addIngredientIcons();
        CreateNewOrder.GetComponent<Order>().UpdateOrderName();
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

    //public static void ForceCleanupNavMesh()
    //{
    //    if (Application.isPlaying)
    //        return;

    //    NavMesh.RemoveAllNavMeshData();
    //}
}
