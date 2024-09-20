using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergeIngredient : MonoBehaviour
{
    public enum KopiMakerStates {READY, PREP, COMPLETE}
    public KopiMakerStates currentState = KopiMakerStates.READY;
    public GameObject[] foods;
    public int CurrentCollectfoodID = 0;
    public int CurrentThrowfoodID = 1;
    public float waitingTime = 5f;  
    [SerializeField] float cookingTimer = 0;
    [SerializeField] Slider slider;
    [SerializeField] GameObject PopUp;
    [SerializeField] GameObject CompletePopUp;
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
            print(other.name);
            print("You reached collection area!");
            playerScript.NearMergePoint = true;
            //playerScript.foodNo = 0;
            playerScript.currentKopiMaker = this.gameObject;
            playerScript.currentFoodThrowable = foods[CurrentCollectfoodID];
            playerScript.currentFoodThrowable = foods[CurrentThrowfoodID];
            
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
            playerScript.currentFoodThrowable = null;
            playerScript.currentFoodThrowable = null;
        }
    }

    void KopiMachineAI()
    {
        switch (currentState)
        {
            case KopiMakerStates.READY:
                CompletePopUp.SetActive(false);
                PopUp.SetActive(false);
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
                CompletePopUp.SetActive(true);
                cookingTimer = 0;
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
