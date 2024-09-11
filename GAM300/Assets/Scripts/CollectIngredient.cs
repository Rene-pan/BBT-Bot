using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectIngredient : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            print("You reached collection area!");
        }
    }
}
