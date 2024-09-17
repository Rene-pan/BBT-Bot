using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergeIngredient : MonoBehaviour
{
    public enum KopiMakerStates {READY, PREP, COMPLETE}
    public KopiMakerStates currentState = KopiMakerStates.READY;
    public GameObject[] foods;
    public int CurrentfoodID = 0;
    public float waitingTime = 5f;  
    [SerializeField] float cookingTimer = 0;
    [SerializeField] Slider slider;
    [SerializeField] GameObject PopUp;

    private void Start()
    {
        SetSlider(slider, waitingTime, cookingTimer);
        PopUp.SetActive(false);
    }

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
            playerScript.currentFood = foods[CurrentfoodID];
            
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
            playerScript.currentFood = null;
        }
    }

    void KopiMachineAI()
    {
        switch (currentState)
        {
            case KopiMakerStates.READY:
                break;
            case KopiMakerStates.PREP:
                PopUp.SetActive(true);
                cookingTimer += Time.deltaTime;
                UpdateSlider(slider, cookingTimer);
                if (cookingTimer >= waitingTime)
                {
                    ChangeState(KopiMakerStates.COMPLETE);
                }
                break;
            case KopiMakerStates.COMPLETE:
                //off UI
                print("YAY");
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

    void SetSlider(Slider slider, float maxValue, float StartingValue)
    {
        slider.maxValue = maxValue;
        slider.value = StartingValue;
    }
    void SliderVisibility(GameObject slider, bool visible)
    {
        slider.SetActive(visible);
    }
    void UpdateSlider(Slider slider, float currentValue)
    {
        slider.value = currentValue;
    }
}
