using UnityEngine;
using UnityEngine.UI;
public class SpreadKaya : MonoBehaviour
{
    public enum KayaMakerStates { READY, PREP, COMPLETE }
    public KayaMakerStates currentState;
    public Transform breadLocation;

    [Header("Spreading Bar")]
    public Slider spreadBreadProgressBar;
    public Color[] TimeSliderColors;
    public float spreadBreadMaxValue;
    public float spreadBreadIncreasingValue;
    private float currentSpreadValue;
    private PlayerController_v2 playerScript;
    public GameObject completeUI;
    //when player reaches here, player will be required to press e to increase the spreading kaya bar
    //when spreading kaya bar reaches the max amount, the status of the bread completion changes to fulfilled
    //player can collect the fulfilled bread and throw
    private void Start()
    {
        SetSlider(spreadBreadProgressBar, spreadBreadMaxValue, 0);
        SliderVisibility(spreadBreadProgressBar, false);
    }
    private void Update()
    {
        if (playerScript == null) return;
        KayaMaker(playerScript);

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            playerScript = other.GetComponent<PlayerController_v2>();
            playerScript.NearSpreadKayaPoint = true;
            playerScript.currentKayaMachine = gameObject.GetComponent<SpreadKaya>();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerScript = other.GetComponent<PlayerController_v2>();
            playerScript.NearSpreadKayaPoint = false;
            playerScript.currentKayaMachine = null;
        }
    }
    void KayaMaker(PlayerController_v2 player)
    {
        switch (currentState)
        {
            case SpreadKaya.KayaMakerStates.READY:
                completeUI.SetActive(false);
                break;
            case SpreadKaya.KayaMakerStates.PREP:
                var canSpread = player.NearSpreadKayaPoint && Input.GetKey(KeyCode.E);
                if (canSpread)
                {
                    currentSpreadValue += spreadBreadIncreasingValue;
                    UpdateSlider(spreadBreadProgressBar, currentSpreadValue);
                }
                if (currentSpreadValue >= spreadBreadProgressBar.maxValue)
                {
                    ChangeState(KayaMakerStates.COMPLETE);
                }
                break;
            case SpreadKaya.KayaMakerStates.COMPLETE:
                completeUI.SetActive(true);
                SetSlider(spreadBreadProgressBar, spreadBreadMaxValue, 0);
                SliderVisibility(spreadBreadProgressBar, false);
                break;
        }
    }
    public void ChangeState(KayaMakerStates newState)
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
    public void SliderVisibility(Slider slider, bool visible)
    {
        slider.gameObject.SetActive(visible);
    }
    void UpdateSlider(Slider slider, float currentValue)
    {
        slider.value = currentValue;
        ChangeSliderColor(slider);
    }
    void ChangeSliderColor(Slider slider)
    {
        if (currentState != KayaMakerStates.PREP) return;
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

}
