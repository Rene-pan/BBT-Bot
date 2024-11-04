using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System;

public class CustomerTable : MonoBehaviour
{
    public int TableID;
    public bool CompletedMeal;
    public Color WrongFoodErrorColour;
    public float FlashTimeInterval;
    public GameObject customer;
    public GameObject foodPosition;
    public List<GameObject> orders;
    public GameObject eatArea;
    public Collider destroyCollider;
    public int succeedCount = 0;
    public int TotalOrderCount = 0;
    public CollideTable CollideScript;

    [Header("VFXs")]
    public VFX vfxScript;

    [Header("Table Stand Display")]
    public List<TextMeshProUGUI> TableStandNumberText;
    public GameObject tableStand;
    public Animator tableStandAnim;

    [Header("Customer Type Big")]
    public Transform[] StandPos;

    [Header("Customer Type Annoying")]
    [SerializeField] Vector3 RotateVector;
    private void Start()
    {
        //find vfx list
        vfxScript = FindFirstObjectByType<VFX>();
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
                if (tableStand.activeSelf)
                {
                    tableStandAnim.Play("TableStandShake");
                    tableStand.SetActive(false);
                }
                AudioManager.instance.PlayOneShot(FmodEvents.instance.HitTable, this.transform.position);
                if (orders.Count == 0)
                {
                    AudioManager.instance.PlayRandom(FmodEvents.instance.crash, FoodTransform.position);
                    vfxScript.PlayVFX(vfxScript.FindVFX("ErrorBurst"), FoodTransform, 1.5f);
                    Destroy(other.gameObject, 1.5f);
                    StartCoroutine(ReActivateStand(1.5f));
                    //print("no food");
                }
                else if (!CollideScript.Fail)
                {
                    foreach (var order in orders) //look through all the orders, if the thrown food name matches the current order name, I will delete that order
                    {
                        //print(order.GetComponent<Order>().OrderName);
                        if (order == null)
                        {
                            AudioManager.instance.PlayRandom(FmodEvents.instance.crash, FoodTransform.position);
                            vfxScript.PlayVFX(vfxScript.FindVFX("ErrorBurst"), FoodTransform,1.5f);
                            Destroy(other.gameObject, 1.5f);
                            //print("food not destroyed");
                            break;
                        }
                        //if customer still has orders, customer will remain on seat
                        else if (order != null && FoodScript.Name == order.GetComponent<Order>().OrderName)
                        {
                            //Off TableStand
                            //Find star burst prefab
                            //Create and play star burst prefab at Food current position
                            vfxScript.PlayVFX(vfxScript.FindVFX("StarBurst_1"), FoodTransform, 3);
                            vfxScript.PlayVFX(vfxScript.FindVFX("StarBurst_2"), FoodTransform, 3);
                            //var suddenBurst = Instantiate(StarBurst, FoodTransform);
                            //Destroy(suddenBurst,3);
                            //var MoreSuddenBurst = Instantiate(StarBurstv2, FoodTransform);
                            //Destroy(MoreSuddenBurst,3);

                            //off destroy collider
                            succeedCount += 1;
                            print(succeedCount);
                            destroyCollider.enabled = false;
                            AudioManager.instance.PlayOneShot(FmodEvents.instance.foodLandSuccess, FoodTransform.position);
                            //print(other);
                            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                            FoodTransform.position = foodPosition.transform.position;
                            Parent(foodPosition.transform, other.gameObject, 0);
                            FoodTransform.localRotation = Quaternion.Euler(Vector3.zero);
                            FoodTransform.localPosition = Vector3.zero;
                            //print(other);
                            //print("FixPosition");
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
                                    customer.transform.position = customerScript.nearestChair.GetComponent<CustomerChair>().seatPivot.position;
                                    customer.transform.LookAt(gameObject.transform);
                                    //customer.transform.rotation = Quaternion.Euler(RotateVector);
                                    break;
                                case Customer_v2.CustomerType.ANNOYING:
                                    customerScript.CustomerEat.SetBool("Jump", false);
                                    //return to chair pos
                                    customer.transform.position = customerScript.nearestChair.GetComponent<CustomerChair>().seatPivot.position;
                                    customer.transform.GetChild(0).localEulerAngles = RotateVector;
                                    //customer.transform.GetChild(0).rotation = Quaternion.Euler(RotateVector);
                                    //customer.transform.LookAt(gameObject.transform);
                                    break;
                            }
                        }
                        else if (FoodScript.Name != order.GetComponent<Order>().OrderName && orders.Count > 1)
                        {
                            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                            Parent(foodPosition.transform, other.gameObject, 0);
                            other.transform.localPosition = Vector3.zero;
                            other.transform.localRotation = Quaternion.Euler(Vector3.zero);
                            //vfxScript.PlayVFX(vfxScript.FindVFX("ErrorBurst"), FoodTransform, 3);
                            //delete food
                            //Destroy(other.gameObject, FlashTimeInterval);
                            print(other);
                            StartCoroutine(ReActivateStand(4f));
                            continue;
                        }
                        else if (FoodScript.Name != order.GetComponent<Order>().OrderName && orders.Count == 1)
                        {
                            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                            Parent(foodPosition.transform, other.gameObject, 0);
                            other.transform.localPosition = Vector3.zero;
                            other.transform.localRotation = Quaternion.Euler(Vector3.zero);
                            vfxScript.PlayVFX(vfxScript.FindVFX("ErrorBurst"), FoodTransform, 3);
                            //change EatAreaColour to red then back to green after a while
                            AudioManager.instance.PlayRandom(FmodEvents.instance.crash, FoodTransform.position);
                            //FlashColour(FlashTimeInterval, eatArea.GetComponent<Renderer>().material, eatArea.GetComponent<Renderer>().material.color, WrongFoodErrorColour);
                            //delete food
                            Destroy(other.gameObject, 1.5f);
                            StartCoroutine(ReActivateStand(4f));
                            break;
                        }
                    }
                }
                break;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    var tag = other.tag;
    //    switch (tag)
    //    {
    //        case "Food":
    //            {
    //                if (!tableStand.activeSelf)
    //                {
    //                    tableStand.SetActive(true);
    //                }
    //            }
    //            break;
    //    }
    //}

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
    IEnumerator ReActivateStand(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!tableStand.activeSelf)
        {
            tableStand.SetActive(true);
        }
    }
}
