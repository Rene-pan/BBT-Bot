using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeIngredient : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            print("You reached collection area!");
            FindAnyObjectByType<PlayerController>().NearMergePoint = true;
            FindAnyObjectByType<PlayerController>().foodNo = 0;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("You left collection area!");
            FindAnyObjectByType<PlayerController>().NearMergePoint = false;
        }
    }
}
