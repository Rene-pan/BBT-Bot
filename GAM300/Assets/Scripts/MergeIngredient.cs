using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeIngredient : MonoBehaviour
{
    public enum KopiMakerStates {READY, PREP, COMPLETE}
    public KopiMakerStates currentState = KopiMakerStates.READY;
    public int foodid = 0;

    private void Update()
    {
        KopiMachineAI();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            var playerScript = other.GetComponent<PlayerController>();
            print("You reached collection area!");
            playerScript.NearMergePoint = true;
            //playerScript.foodNo = 0;
            playerScript.currentKopiMaker = this.gameObject;
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            var playerScript = other.GetComponent<PlayerController>();
            print("You left collection area!");
            playerScript.NearMergePoint = false;
            playerScript.currentKopiMaker = null;
        }
    }

    void KopiMachineAI()
    {
        switch (currentState)
        {
            case KopiMakerStates.READY:
                print("Boooo rene what you doing");
                break;
            case KopiMakerStates.PREP:
                print("YAY!!!");
                break;
            case KopiMakerStates.COMPLETE:
                break;
        }
    }

    public void ChangeState(KopiMakerStates newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
    }
}
