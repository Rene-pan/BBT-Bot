using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Customer_v2 : MonoBehaviour
{
    public enum CustomerType { NORMAL, BIG, ANNOYING, KAREN }
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
    public GameObject nearestChair;//linked with WAIT state to determine which chair to go back to

    [Header("Customer Order")]
    [SerializeField] List<GameObject> OrderUI;//all the order UI that customer will generate
    [SerializeField] List<GameObject> OrderList; //all the genrated orderUI for this customer
    [SerializeField] int OrderUI_ID;
    [SerializeField] GameObject OrderUIHolder; //contains all orders
    [SerializeField] float OrderWaitTime;
    [SerializeField] float currentTime;
    [SerializeField] float DecreasingValue;

    [Header("Customer Eat")]
    public GameObject Food;
    [SerializeField] float EatingSpeedMultiplier;
    [SerializeField] float EatingDuration;
    public float currentEatTime;
    public GameObject OrderToDelete;
    public int CustomerMoney;
    public Money MoneyScript;

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
    public bool CreatedOrder = false;

    [Header("Annoying Customer")]
    [SerializeField] float JumpingDuration;
    [SerializeField] float JumpForce;

    [Header("Karen Customer")]
    [SerializeField] int DecreaseMoneyAmount;

    private void OnEnable()
    {
        targetwaypoint = waypoints[targetWaypointIndex];
        OrderUIHolder = GameObject.Find("OrderList");
        Exitdoor = GameObject.Find("Spawner").transform;
        MoneyScript = FindFirstObjectByType<Money>();
        EatSFX = AudioManager.instance.CreateInstance(FmodEvents.instance.eats);
        EatSFX.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        CreatedOrder = false;
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
            //print(targetWaypoint_backIndex);
            {
                targetWaypoint_backIndex += 1;
                if (targetWaypoint_backIndex == waypointsBack.Count)
                {
                    targetWaypoint_backIndex = waypointsBack.Count;
                }
                targetwaypoint_back = waypointsBack[targetWaypoint_backIndex];
                //print("help"+ targetWaypoint_backIndex);
            }
        }
        else
        {
            if (currentDistance <= minDistance)
            {
                targetWaypointIndex += 1;
                if (targetWaypointIndex == waypoints.Count)
                {
                    //print(targetWaypointIndex);
                    targetWaypointIndex = 0;
                }
                targetwaypoint = waypoints[targetWaypointIndex];
                //print(targetWaypointIndex);
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
                CustomerTypes();
                break;

            case CustomerStates.EAT:
                CustomerEats(EatingSpeedMultiplier, EatingDuration);
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
        var TableScript = nearestTable.GetComponent<CustomerTable>();
        SetTimer(OrderScript.DecreasingTimer, OrderWaitTime);
        OrderScript.addIngredientIcons();
        OrderScript.UpdateOrderUI(TableScript.TableID);
        //set order name to table
        //TableScript.FoodNames.Add(OrderScript.OrderName);
        //TableScript.orders.Clear();
        TableScript.orders.Add(CreateNewOrder);
        TableScript.eatArea.SetActive(true);
        OrderUI_ID += 1;
        TableScript.TotalOrderCount += 1;
        return CreateNewOrder;
    }

    public void KeepTrackOfOrders()
    {
        //if (OrderUIHolder.GetComponent<OrderInfo>().numberOfOrders >= OrderUIHolder.GetComponent<OrderInfo>().maxOrders) return;
        if (OrderUI_ID == OrderUI.Count)
        {
            OrderUI_ID = 0;
            CreatedOrder = true;
            ChangeState(CustomerStates.WAIT);
        }
        else
        {
            OrderList.Add(CreateOrder());
            OrderUIHolder.GetComponent<OrderInfo>().numberOfOrders += 1;
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
    #region CustomerEat
    private EventInstance EatSFX;
    public void CustomerEats(float EatingSpeedMultiplier, float eatduration)
    {
        if (OrderToDelete != null)
        {
            OrderToDelete.GetComponent<Image>().color = Color.green;
            OrderList.Remove(OrderToDelete);
            Destroy(OrderToDelete, 2);
        }
        currentEatTime += Time.deltaTime * EatingSpeedMultiplier;
        Food.transform.GetChild(0).GetComponent<Animator>().Play("CupFadeOut");
        Food.GetComponent<Throwable>().eatCanvas.SetActive(true);
        PLAYBACK_STATE playbackState;
        EatSFX.getPlaybackState(out playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            //print("eating");
            EatSFX.start();
        }
        else if (Time.timeScale == 0)
        {
            EatSFX.stop(STOP_MODE.IMMEDIATE);
        }
        if (currentEatTime >= eatduration)
        {
            EatSFX.stop(STOP_MODE.ALLOWFADEOUT);
            Food.GetComponent<Throwable>().eatCanvas.SetActive(false);
            Food.transform.GetChild(0).GetComponent<Animator>().SetBool("CupStop", true);
            //delete food on table
            Destroy(Food);
            //add money
            MoneyScript.AddMoney(CustomerMoney);
            //change waypointIndex
            for (int i = waypoints.Count - 1; i >= 0; i--)
            {
                waypointsBack.Add(waypoints[i]);
            }
            waypointsBack.Add(Exitdoor);
            targetwaypoint_back = waypointsBack[startIndex];
            currentEatTime = 0;
            nearestTable.GetComponent<CustomerTable>().destroyCollider.enabled = true;
            //movetowards the index
            if (nearestTable.GetComponent<CustomerTable>().succeedCount == nearestTable.GetComponent<CustomerTable>().TotalOrderCount)
            {
                //print(nearestTable.GetComponent<CustomerTable>().succeedCount);
                //print(nearestTable.GetComponent<CustomerTable>().TotalOrderCount);
                nearestTable.GetComponent<CustomerTable>().eatArea.SetActive(false);
                nearestTable.GetComponent<CustomerTable>().orders.Remove(OrderToDelete);
                ChangeState(CustomerStates.LEAVE);
            }
            else if (nearestTable.GetComponent<CustomerTable>().succeedCount < nearestTable.GetComponent<CustomerTable>().TotalOrderCount)
            {
                //print(nearestTable.GetComponent<CustomerTable>().succeedCount);
                //print(nearestTable.GetComponent<CustomerTable>().TotalOrderCount);
                nearestTable.GetComponent<CustomerTable>().eatArea.SetActive(true);
                nearestTable.GetComponent<CustomerTable>().orders.Remove(OrderToDelete);
                ChangeState(CustomerStates.WAIT);
            }
            OrderUIHolder.GetComponent<OrderInfo>().numberOfOrders -= 1;
        }
    }
    #endregion

    #region CustomerAngry
    public void CustomerAngry()
    {
        //customer turns red
        EmotionHolder.SetActive(true);
        EmotionHolder.GetComponent<Image>().sprite = Espressions[0];
        //customer deletes order, order flashes red
        OrderUIHolder.GetComponent<OrderInfo>().numberOfOrders -= OrderList.Count;
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
        nearestTable.GetComponent<CustomerTable>().destroyCollider.enabled = true;

        //check if the Customer Type is KAREN
        if (customerType == CustomerType.KAREN)
        {
            MoneyScript.DecreaseMoney(DecreaseMoneyAmount);
        }
        //movetowards the index
        ChangeState(CustomerStates.LEAVE);
    }
    #endregion
    #region CustomerLeave
    //customer moves back to spawn position
    //add the waypoints from back to the front
    public void CustomerLeave()
    {
        float movementStep = movementSpeed * Time.deltaTime;
        float rotationStep = rotationSpeed * Time.deltaTime;
        Vector3 directionToTarget = targetwaypoint_back.position- transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);
        float distance = Vector3.Distance(transform.position, targetwaypoint_back.position);
        transform.position = Vector3.MoveTowards(transform.position, targetwaypoint_back.position, movementStep);
        CheckDistanceToWPNMove(distance);
        var TableScript = nearestTable.GetComponent<CustomerTable>();
        TableScript.orders.Clear();
    }

    #endregion

    #region Customer Types
    private bool TransfromOnce = true;
    private float time = 0;
    private bool StartAnimation = false;
    private float time1 = 0;
    public GameObject AnimatorObj;
    void CustomerTypes()
    {
        switch (customerType) 
        { 
            case CustomerType.BIG:
                {
                    if (TransfromOnce)
                    {
                        var tableScript = nearestTable.GetComponent<CustomerTable>();
                        var randomPositionID = (int)Random.Range(0, (float)tableScript.StandPos.Length);
                        print(randomPositionID);
                        transform.position = tableScript.StandPos[randomPositionID].position;
                        transform.LookAt(nearestTable.transform.position);
                        TransfromOnce = false;
                    }
                    break;
                }
            case CustomerType.ANNOYING:
                {
                    time += Time.deltaTime;
                    if (time < JumpingDuration && !StartAnimation)
                    {
                        transform.position = nearestTable.GetComponent<CustomerTable>().foodPosition.transform.position;
                        //play animation
                        AnimatorObj.GetComponent<Animator>().SetBool("Jump", true);
                        nearestTable.GetComponent<Collider>().enabled = false;
                    }else if (time >= JumpingDuration)
                    {
                        time1 += Time.deltaTime;
                        if (time1 < JumpingDuration){
                            AnimatorObj.GetComponent<Animator>().SetBool("Jump", false);
                            StartAnimation = true;
                            nearestTable.GetComponent<Collider>().enabled = true;
                            transform.position = nearestChair.GetComponent<CustomerChair>().seatPivot.transform.position;
                        }
                        else if (time1 >= JumpingDuration)
                        {
                            StartAnimation = false;
                            time = 0;
                            time1 = 0;
                        }
                    }
                    //jump on table
                    //after a period of time
                    //seat back down
                    break;
                }
            case CustomerType.KAREN:
                break;
        
        }

    }
    #endregion

}
