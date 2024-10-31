using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static MergeIngredient;

public class MergeIngredient : MonoBehaviour
{
    public enum MakerTypes {TOAST, DRINK}
    public MakerTypes makerType;
    public enum KopiMakerStates {READY, PREP, COMPLETE}
    public KopiMakerStates currentState = KopiMakerStates.READY;
    public GameObject[] foods;
    public int CollectfoodID = 0;
    public int CurrentCollectfoodID = 0;
    public int CurrentThrowfoodID = 1;
    public float waitingTime = 5f;  
    [SerializeField] float cookingTimer = 0;
    [SerializeField] Slider slider;
    [SerializeField] GameObject PopUp;
    [SerializeField] GameObject CompletePopUp;
    public Color[] TimeSliderColors;
    private EventInstance MergingSFX;
    private EventInstance ToastSFX;
    private void Start()
    {
        SetSlider(slider, waitingTime, cookingTimer);
        PopUp.SetActive(false);
        //create drink Sfx
        MergingSFX = AudioManager.instance.CreateInstance(FmodEvents.instance.drinkMaking);
        MergingSFX.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform.parent));
        //create toast Sfx
        ToastSFX = AudioManager.instance.CreateInstance(FmodEvents.instance.ToastBread);
        ToastSFX.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform.parent));
    }

    private void Update()
    {
        KopiMachineAI();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            var playerScript = other.GetComponent<PlayerController_v2>();
            FoodMakerTypeTriggerStay(playerScript);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            var playerScript = other.GetComponent<PlayerController_v2>();
            FoodMakerTypeTriggerLeave(playerScript);
        }
    }

    void FoodMakerTypeTriggerStay(PlayerController_v2 playerScript)
    {
        switch (makerType)
        {
            case MakerTypes.TOAST:
                    playerScript.NearMergePoint = true;
                    playerScript.currentKopiMaker = this.gameObject;
                    playerScript.currentFoodCollectable = foods[CurrentCollectfoodID];
                    if (playerScript.holdIngredient == null) return;
                    var currentToastScript = playerScript.holdIngredient.GetComponent<CollectableFood>();
                    //print(currentFoodScript.CollectableFoodID);
                    //print(CollectfoodID);
                    //print("You reached collection area!");
                    if (currentToastScript.CollectableFoodID == CollectfoodID)
                    {
                        playerScript.Mergeable = true;
                    }
                    else
                    {
                        playerScript.Mergeable = false;
                    }
                    break;

            case MakerTypes.DRINK:
                    playerScript.NearMergePoint = true;
                    playerScript.currentKopiMaker = this.gameObject;
                    playerScript.currentFoodCollectable = foods[CurrentCollectfoodID];
                    playerScript.currentFoodThrowable = foods[CurrentThrowfoodID];
                if (playerScript.holdIngredient == null) return;
                    var currentDrinkScript = playerScript.holdIngredient.GetComponent<CollectableFood>();
                    //print(currentFoodScript.CollectableFoodID);
                    //print(CollectfoodID);
                    //print("You reached collection area!");
                    if (currentDrinkScript.CollectableFoodID == CollectfoodID)
                    {
                        playerScript.Mergeable = true;
                    }
                    else
                    {
                        playerScript.Mergeable = false;
                    }
                    break;
        }
    }
    void FoodMakerTypeTriggerLeave(PlayerController_v2 playerScript)
    {
        switch (makerType)
        {
            case MakerTypes.TOAST:
                {
                    playerScript.NearMergePoint = false;
                    playerScript.currentKopiMaker = null;
                    playerScript.currentFoodCollectable = null;
                    if (playerScript.UIFinder("WrongIngredient").activeSelf)
                    {
                        playerScript.UIFinder("WrongIngredient").SetActive(false);
                    }
                    break;
                }
            case MakerTypes.DRINK:
                {
                    print("You left collection area!");
                    playerScript.NearMergePoint = false;
                    playerScript.currentKopiMaker = null;
                    playerScript.currentFoodCollectable = null;
                    playerScript.currentFoodThrowable = null;
                    if (playerScript.UIFinder("WrongIngredient").activeSelf)
                    {
                        playerScript.UIFinder("WrongIngredient").SetActive(false);
                    }
                    break;
                }
        }
        if (playerScript.UIFinder("BusyKopiMaker").activeSelf)
        {
            playerScript.UIFinder("BusyKopiMaker").SetActive(false);
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
                PLAYBACK_STATE DrinkplaybackState;
                PLAYBACK_STATE ToastplaybackState;
                switch (makerType)
                {
                    case MakerTypes.TOAST:
                        ToastSFX.getPlaybackState(out ToastplaybackState);
                        if (ToastplaybackState.Equals(PLAYBACK_STATE.STOPPED))
                        {
                            ToastSFX.start();
                        }
                        else if (Time.timeScale == 0)
                        {
                            ToastSFX.stop(STOP_MODE.IMMEDIATE);
                        }
                        break;
                    case MakerTypes.DRINK:
                        MergingSFX.getPlaybackState(out DrinkplaybackState);
                        if (DrinkplaybackState.Equals(PLAYBACK_STATE.STOPPED))
                        {
                            MergingSFX.start();
                        }
                        else if (Time.timeScale == 0)
                        {
                            MergingSFX.stop(STOP_MODE.IMMEDIATE);
                        }
                        break;
                }
                if (cookingTimer >= waitingTime)
                {
                    if (makerType == MakerTypes.DRINK)
                    {
                        AudioManager.instance.PlayOneShot(FmodEvents.instance.cookingComplete, this.transform.position);
                    }
                    else if (makerType == MakerTypes.TOAST)
                    {
                        AudioManager.instance.PlayOneShot(FmodEvents.instance.ToastBreadComplete, this.transform.position);
                    }
                    ChangeState(KopiMakerStates.COMPLETE);
                    MergingSFX.stop(STOP_MODE.IMMEDIATE);
                    ToastSFX.stop(STOP_MODE.IMMEDIATE);
                }
                break;
            case KopiMakerStates.COMPLETE:
                //off UI
                CompletePopUp.SetActive(true);
                //MergingSFX.stop(STOP_MODE.IMMEDIATE);
                ResetTimer();
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
        ChangeSliderColor(slider);
    }

    void ChangeSliderColor(Slider slider)
    {
        if (currentState != KopiMakerStates.PREP) return;
        var sliderFillColour = slider.transform.GetChild(1).GetComponent<Image>();
        float percentage = (slider.value / slider.maxValue) * 100;
        //light to dark
        if (percentage <= 25)
        {
            sliderFillColour.color = TimeSliderColors[0];
        }
        else if (percentage <= 50 && percentage > 25)
        {
            sliderFillColour.color = TimeSliderColors[1];
        }
        else if (percentage <= 75 && percentage > 50)
        {
            sliderFillColour.color = TimeSliderColors[2];
        }
        else if (percentage <= 100 && percentage > 75)
        {
            sliderFillColour.color = TimeSliderColors[3];
        }
    }
    void ResetTimer()
    {
        var sliderFillColour = slider.transform.GetChild(1).GetComponent<Image>();
        cookingTimer = 0;
        sliderFillColour.color = TimeSliderColors[0];
    }

    public GameObject SetThrowable()
    {
        var throwable = foods[1];
        return throwable;
    }
}
