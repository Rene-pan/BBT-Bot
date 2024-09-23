using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    [SerializeField] CamController_v3 camScript;

    [Header("Grab ingredient")]
    public bool NearCollectionPoint = false;
    public bool NearMergePoint = false;
    public Transform hand;
    public int hand_amount = 0;
    public GameObject currentIngredient;
    public GameObject currentFoodCollectable;
    public GameObject currentFoodThrowable;
    public GameObject currentKopiMaker;
    private GameObject holdIngredient;
    private GameObject holdFood;


    [Header("Throwing")]
    public bool canThrow;
    public bool ThrowOnce = true;
    private ProjectileThrow throwscript;
    public float minThrowDistance;
    [SerializeField] Slider throwStrength;
    [SerializeField] int maxStrengthValue;
    [SerializeField] float addStrengthValue;
    public float time;


    private void Start()
    {
        throwscript = FindAnyObjectByType<ProjectileThrow>();
        throwscript.force = 0;
        time = 0;
        SetSlider(throwStrength,maxStrengthValue,throwscript.force);
    }
    private void Update()
    {
        if (camScript.currentState == CamController_v3.CamState.THIRDPERSON)
        {
            CollectIngredient();
            CollectFood();
            Merge();
            print("Hi");
            canThrow = false;
            SliderVisibility(throwStrength.gameObject, canThrow);
        }
        else if (camScript.currentState == CamController_v3.CamState.OVERSHOULDER)
        {
            canThrow = true;
            SliderVisibility(throwStrength.gameObject, canThrow);
            Throw();
        }
    }

    void CollectIngredient()
    {
        if (currentIngredient == null) return;
        var canCollectIngredient = NearCollectionPoint && Input.GetKeyDown(KeyCode.E);
        var canCollectFood = NearMergePoint && Input.GetKeyDown(KeyCode.E) && hand_amount < 1
            && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.COMPLETE;
        if (canCollectIngredient && hand_amount <1 && !canCollectFood)
        {
            holdIngredient = Instantiate(currentIngredient, hand);
            hand_amount += 1;
        }
        
    }
    void CollectFood()
    {
        var canCollectIngredient = NearCollectionPoint && Input.GetKeyDown(KeyCode.E);
        var canCollectFood = NearMergePoint && Input.GetKeyDown(KeyCode.E) && hand_amount < 1
            && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.COMPLETE;
        if (!canCollectIngredient && canCollectFood)
        {
            holdFood = Instantiate(currentFoodCollectable, hand);
            throwscript.objectToThrow = currentKopiMaker.GetComponent<MergeIngredient>().SetThrowable().GetComponent<Rigidbody>();
            currentKopiMaker.GetComponent<MergeIngredient>().currentState = MergeIngredient.KopiMakerStates.READY;
            ThrowOnce = false;
        }
    }


    void Merge()
    {
        var startCookingTimer = hand_amount == 1 && NearMergePoint && Input.GetKeyDown(KeyCode.E);
        if (currentKopiMaker == null) return;
        if (startCookingTimer)
        {
            Destroy(holdIngredient);
            hand_amount = 0;
            var KopiMakerScript = currentKopiMaker.GetComponent<MergeIngredient>();
            KopiMakerScript.ChangeState(MergeIngredient.KopiMakerStates.PREP);
        }
    }

    void Throw()
    { //press for a while, once bar reach max for 3 seconds, force minus
        if (canThrow && Input.GetMouseButton(0))
        {
            throwscript.force += addStrengthValue;
            UpdateSlider(throwStrength, throwscript.force);

        }
        if (canThrow && Input.GetMouseButtonUp(0) && !ThrowOnce)
        {
            throwscript.ThrowObject();
            throwscript.force = minThrowDistance;
            UpdateSlider(throwStrength, throwscript.force);
            ThrowOnce = true;
            Destroy(holdFood);
            camScript.ChangeState(CamController_v3.CamState.THIRDPERSON);
            throwscript.trajectoryPredictor.SetTrajectoryVisible(false);
            throwscript.force = 0;
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
