using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CollectIngredient : MonoBehaviour
{
    public enum CollectTypes { TOAST, DRINK }
    public CollectTypes currentType;
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
            playerScript.currentCollectionArea = gameObject;
            if (currentType == CollectTypes.DRINK)
            {
                playerScript.UIFinder("PressEToCollect").GetComponent<IngredientIndicator>().UpdateIngredient(2);
            }
            else if (currentType == CollectTypes.TOAST)
            {
                playerScript.UIFinder("PressEToCollect").GetComponent<IngredientIndicator>().UpdateIngredient(0);
            }
            playerScript.UIFinder("PressEToCollect").SetActive(true);
            playerScript.UIFinder("PressEToCollect").GetComponent<Animator>().Play("PulseUI");
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
            playerScript.currentCollectionArea = null;
            if (playerScript.UIFinder("BusyKopiMaker").activeSelf)
            {
                playerScript.UIFinder("BusyKopiMaker").SetActive(false);
            }
            if (playerScript.UIFinder("PressEToCollect").activeSelf)
            {
                playerScript.UIFinder("PressEToCollect").SetActive(false);
            }
        }
    }
}
