using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class CustomerTable : MonoBehaviour
{
    public bool CompletedMeal;
    public string FoodName;
    public Color WrongFoodErrorColour;
    public float FlashTimeInterval;
    public GameObject customer;
    public GameObject foodPosition;
    public List<GameObject> orders;
    public GameObject eatArea;
    private void Start()
    {
        eatArea.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        var tag = other.tag;
        switch (tag)
        {
            case "Food":
                //check the throwable food name with this table food name
                var FoodScript = other.GetComponent<Throwable>();
                if (orders == null) return;
                foreach (var order in orders) //look through all the orders, if the thrown food name matches the current order name, I will delete that order
                {
                    if (order == null) { print("no food"); } 
                    if (FoodScript.Name == order.GetComponent<Order>().OrderName)
                    {
                        //print(other);
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        other.gameObject.transform.position = foodPosition.transform.position;
                        Parent(foodPosition.transform, other.gameObject, 0);
                        other.gameObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        other.gameObject.transform.localPosition = Vector3.zero;
                        print(other);
                        print("FixPosition");
                        //get the food to remain on the table
                        if (customer == null) return;
                        //change Customer_v2 state to eat
                        var customerScript = customer.GetComponent<Customer_v2>();
                        customerScript.Food = other.gameObject;
                        customerScript.OrderToDelete = order;
                        eatArea.SetActive(false);
                        //FlashColour(FlashTimeInterval, eatArea.GetComponent<Material>(), eatArea.GetComponent<Material>().color, CorrectFoodColour);
                        customerScript.ChangeState(Customer_v2.CustomerStates.EAT);
                    }
                    else if (FoodScript.Name != order.GetComponent<Order>().OrderName)
                    {
                        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        Parent(foodPosition.transform, other.gameObject, 0);
                        other.transform.localPosition = Vector3.zero;
                        other.transform.localRotation = Quaternion.Euler(Vector3.zero);
                        //change EatAreaColour to red then back to green after a while
                        FlashColour(FlashTimeInterval, eatArea.GetComponent<Material>(), eatArea.GetComponent<Material>().color, WrongFoodErrorColour);
                        //delete food
                        Destroy(other.gameObject, FlashTimeInterval);
                    }
                    break;
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
