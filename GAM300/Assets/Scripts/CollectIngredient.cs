using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectIngredient : MonoBehaviour
{
    public GameObject[] ingredients;
    public int CurrentingredientID = 0;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            var playerScript = other.GetComponent<PlayerController_v2>();
            print("You reached collection area!");
            print(other.name);
            playerScript.NearCollectionPoint = true;
            playerScript.currentIngredient = ingredients[CurrentingredientID];
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            var playerScript = other.GetComponent<PlayerController_v2>();
            print("You left collection area!");
            playerScript.NearCollectionPoint = false;
            playerScript.currentIngredient = null;
        }
    }
}
