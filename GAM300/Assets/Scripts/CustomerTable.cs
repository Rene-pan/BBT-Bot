using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerTable : MonoBehaviour
{
    public int TableID;
    public bool CompletedMeal;
    //public string FoodName;
    //public List<string> FoodNames;
    public Color WrongFoodErrorColour;
    public float FlashTimeInterval;
    public GameObject customer;
    public GameObject foodPosition;
    public List<GameObject> orders;
    public GameObject eatArea;
    public Collider destroyCollider;
    public int succeedCount = 0;
    public int TotalOrderCount = 0;
    public GameObject StarBurst;

    [Header("Table Stand Display")]
    public List<TextMeshProUGUI> TableStandNumberText;
    public GameObject tableStand;

    [Header("Customer Type Big")]
    public Transform[] StandPos;
    private void Start()
    {
        eatArea.SetActive(false);
        foreach (var text in TableStandNumberText)
        {
            text.text = TableID.ToString();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        var tag = other.tag;
        switch (tag)
        {
            case "Food":
                //check the throwable food name with this table food name
                var FoodScript = other.GetComponent<Throwable>();
                var FoodTransform = other.transform;
                //var CustomerTransform = customer.transform;
                if (orders == null) return;
                if (other == null) return;
                foreach (var order in orders) //look through all the orders, if the thrown food name matches the current order name, I will delete that order
                {
                    print(FoodScript.Name);
                    print(order.GetComponent<Order>().OrderName);
                    if (order == null) { print("no food"); 
                        AudioManager.instance.PlayRandom(FmodEvents.instance.crash, this.transform.position);
                        Destroy(other.gameObject);
                        break;
                    }
                    //if customer still has orders, customer will remain on seat
                    else if (FoodScript.Name == order.GetComponent<Order>().OrderName)
                    {
                        //Off TableStand
                        if (tableStand.activeSelf)
                        {
                            tableStand.SetActive(false);
                        }
                        //Create star burst prefab
                        var suddenBurst = Instantiate(StarBurst, FoodTransform);
                        Destroy(suddenBurst,2);
                        //off destroy collider
                        succeedCount += 1;
                        print(succeedCount);
                        destroyCollider.enabled = false;
                        AudioManager.instance.PlayOneShot(FmodEvents.instance.foodLandSuccess, this.transform.position);
                        //print(other);
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        FoodTransform.position = foodPosition.transform.position;
                        Parent(foodPosition.transform, other.gameObject, 0);
                        FoodTransform.localRotation = Quaternion.Euler(Vector3.zero);
                        FoodTransform.localPosition = Vector3.zero;
                        print(other);
                        print("FixPosition");
                        //get the food to remain on the table
                        if (customer == null) return;
                        //change Customer_v2 state to eat
                        //if customer type is BIG, transform its positon back to the seat
                        var customerScript = customer.GetComponent<Customer_v2>();
                        customerScript.Food = other.gameObject;
                        customerScript.OrderToDelete = order;
                        //FlashColour(FlashTimeInterval, eatArea.GetComponent<Material>(), eatArea.GetComponent<Material>().color, CorrectFoodColour);
                        customerScript.ChangeState(Customer_v2.CustomerStates.EAT);
                        switch (customerScript.customerType) 
                        {
                            case Customer_v2.CustomerType.BIG:
                            case Customer_v2.CustomerType.ANNOYING:
                                //return to chair pos
                                customer.transform.position = customerScript.nearestChair.GetComponent<CustomerChair>().seatPivot.position;
                                customer.transform.LookAt(gameObject.transform);
                                break;

                        
                        }
                    }
                    else if (FoodScript.Name != order.GetComponent<Order>().OrderName && orders.Count > 1)
                    {
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        Parent(foodPosition.transform, other.gameObject, 0);
                        other.transform.localPosition = Vector3.zero;
                        other.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        //change EatAreaColour to red then back to green after a while
                        //FlashColour(FlashTimeInterval,eatArea.GetComponent<Renderer>().material, eatArea.GetComponent<Renderer>().material.color, WrongFoodErrorColour);
                        //delete food
                        //Destroy(other.gameObject, FlashTimeInterval);
                        print(other);
                        continue;
                    }
                    else if (FoodScript.Name != order.GetComponent<Order>().OrderName && orders.Count == 1)
                    {
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        Parent(foodPosition.transform, other.gameObject, 0);
                        other.transform.localPosition = Vector3.zero;
                        other.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        //change EatAreaColour to red then back to green after a while
                        FlashColour(FlashTimeInterval,eatArea.GetComponent<Renderer>().material, eatArea.GetComponent<Renderer>().material.color, WrongFoodErrorColour);
                        //delete food
                        Destroy(other.gameObject, FlashTimeInterval);
                        break;
                    }
                }
                break;
        }
    }

    IEnumerator FlashColour(float delay, Material material, Color org, Color nextColour)
    {
        material.SetColor("_Color", nextColour);
        yield return new WaitForSeconds(delay);
        material.SetColor("_Color", org);
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
