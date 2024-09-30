using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
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
    private newThrow throwscript;
    public float minThrowDistance;
    [SerializeField] Slider throwStrength;
    [SerializeField] int maxStrengthValue;
    [SerializeField] float addStrengthValue;
    public float time;
    [SerializeField] List<GameObject> PlayerUI;
    public Sprite[] ThrowPrompts; //org, new

    private void Start()
    {
        throwscript = FindAnyObjectByType<newThrow>();
        throwscript.lr.enabled = false;
        //throwscript.InitialAngle = 0;
        time = 0;
        //SetSlider(throwStrength,maxStrengthValue,throwscript.InitialAngle);
        foreach (GameObject UI in GameObject.FindGameObjectsWithTag("PlayerUI"))
        {
            PlayerUI.Add(UI);
            if (UI.name == "OrderList" || UI.name == "Earnings")
            {
                UI.SetActive(true);
            }
            else if (UI.name == "Success" || UI.name == "GameOver")
            {
                var moneyScript = GameObject.FindFirstObjectByType<Money>();
                moneyScript.UIs.Add(UI);
                UI.SetActive(false);
            }
            else
            {
                UI.SetActive(false);
            }
            
        }
    }
    private void Update()
    {
        if (camScript.currentState == CamController_v3.CamState.THIRDPERSON)
        {
            CollectIngredient();
            CollectFood();
            Merge();
            canThrow = false;
        }
        else if (camScript.currentState == CamController_v3.CamState.OVERSHOULDER)
        {
            canThrow = true;
            Throw();
        }
    }
    //trying to fix, if collected food, cannot collect anything else

    void CollectIngredient()
    {
        //if no current ingredient
        if (currentIngredient == null) return;
        //trying to find if UI is active
        if (UIFinder("CollectACupFirst").activeSelf)
        {
            //if UI is active, make it disappear
            UIFinder("CollectACupFirst").SetActive(false);
        }
        //can collect ingredient when its near collection point & press down E key and currently not near merge point && handamount is <1 (so no food/ingredient on hand)
        var canCollectIngredient = NearCollectionPoint && Input.GetKeyDown(KeyCode.E) && hand_amount < 1 && !NearMergePoint;
        var canCollectFood = NearMergePoint && Input.GetKeyDown(KeyCode.E) && hand_amount < 1
            && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.COMPLETE && !NearCollectionPoint;
        if (canCollectIngredient && !canCollectFood)
        {
            holdIngredient = Instantiate(currentIngredient, hand);
            hand_amount = 1;
        }
        
    }
    void CollectFood()
    {
        var canCollectIngredient = NearCollectionPoint && Input.GetKeyDown(KeyCode.E) && hand_amount < 1 && !NearMergePoint;
        var canCollectFood = NearMergePoint && Input.GetKeyDown(KeyCode.E) && hand_amount < 1
            && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.COMPLETE && !NearCollectionPoint;
        var cannotCollectFood = NearMergePoint && Input.GetKeyDown(KeyCode.E) && hand_amount < 1
    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.READY && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.PREP;
        var OffActivateThrowmode = !canThrow && hand_amount < 1 && Input.GetMouseButtonDown(1);
        if (!canCollectIngredient && canCollectFood)
        {
            holdFood = Instantiate(currentFoodCollectable, hand);
            throwscript.rb = currentKopiMaker.GetComponent<MergeIngredient>().SetThrowable().GetComponent<Rigidbody>();
            currentKopiMaker.GetComponent<MergeIngredient>().currentState = MergeIngredient.KopiMakerStates.READY;
            ThrowOnce = false;
            hand_amount = 1;
            //activate throw mode prompt flashes
            UIFinder("ActivateThrowmode").SetActive(true);
            UIFinder("ActivateThrowmode").transform.GetChild(0).GetComponent<Image>().sprite = ThrowPrompts[0];
            UIFinder("ActivateThrowmode").GetComponent<Animator>().Play("PulsingThrowPromptUI");

        }
        else if (!canCollectIngredient && cannotCollectFood)
        {
            UIFinder("CollectACupFirst").SetActive(true);
            UIFinder("CollectACupFirst").GetComponent<Animator>().Play("PulsingThrowPromptUI");
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
    { 
        //press for a while, once bar reach max for 3 seconds, force minus
        //if (canThrow && Input.GetMouseButton(0))
        //{
        //    throwscript.InitialAngle += addStrengthValue;
        //    UpdateSlider(throwStrength, throwscript.InitialAngle);
        //}
        //else if (canThrow && Input.GetMouseButtonUp(0)) 
        //{
        //    throwscript.InitialAngle -= addStrengthValue;
        //    UpdateSlider(throwStrength, throwscript.InitialAngle);
        //}
        if (canThrow && Input.GetMouseButtonDown(0) && !ThrowOnce)
        {
            throwscript.Throw();
            ThrowOnce = true;
            Destroy(holdFood);
            //throwscript.lr.enabled = false;
            UIFinder("ActivateThrowmode").transform.GetChild(0).GetComponent<Image>().sprite = ThrowPrompts[1];
            UIFinder("ActivateThrowmode").SetActive(true);
            //camScript.ChangeState(CamController_v3.CamState.THIRDPERSON);
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

    public GameObject UIFinder(string UIName)
    {
        foreach (var UI in PlayerUI)
        {
            if (UI.name == UIName)
            {
                return UI;
            }
        }
        return null;
    }
}
