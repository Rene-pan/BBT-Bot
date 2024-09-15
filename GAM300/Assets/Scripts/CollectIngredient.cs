using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectIngredient : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            print("You reached collection area!");
            FindAnyObjectByType<PlayerController>().NearCollectionPoint = true;
            FindAnyObjectByType<PlayerController>().ingredientNo = 0;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            print("You left collection area!");
            FindAnyObjectByType<PlayerController>().NearCollectionPoint = false;
        }
    }
}
