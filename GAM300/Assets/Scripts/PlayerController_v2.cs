using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_v2 : MonoBehaviour
{
    [SerializeField] CamController_v3 camScript;
    public enum PlayerCollection { COLLECT, THROW }
    public PlayerCollection currentState;
    [Header("Grab ingredient")]
    public bool NearCollectionPoint = false;
    public bool NearMergePoint = false;
    public bool NearSpreadKayaPoint = false;
    public bool Mergeable = false;
    public Transform hand;
    public int hand_amount = 0;
    public GameObject currentIngredient;
    public GameObject currentFoodCollectable;
    public GameObject currentFoodThrowable;
    public GameObject currentKopiMaker;
    public GameObject holdIngredient;
    public GameObject holdFood;
    public SpawnCustomer spawner;
    public int CurrentHoldIngredientID = 0; //0 means empty, 

    [Header("Throwing")]
    public bool canThrow;
    public bool ThrowOnce = true;
    public ProjectileThrow throwscript;
    public LineRenderer lr;
    public float time;
    [SerializeField] List<GameObject> PlayerUI;
    public Sprite[] ThrowPrompts; //org, new
    public int PressCount = 0;

    [Header("Spreading Kaya")]
    public SpreadKaya currentKayaMachine;
    private GameObject ToastedBread;

    private void Start()
    {
        lr.enabled = false;
        time = 0;
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
        PlayerStates();
    }
    void PlayerStates()
    {
        switch (currentState)
        {
            case PlayerCollection.COLLECT:
                //player can only collect within this mode
                //player cannot collect when they have food on their hand
                //if player collect ingredient, cannot collect anything else and must be near collectionpoint
                var canCollectIngredient = NearCollectionPoint && hand_amount < 1 && Input.GetKeyDown(KeyCode.E);
                if (canCollectIngredient)
                {
                    holdIngredient = Instantiate(currentIngredient, hand);
                    CurrentHoldIngredientID = holdIngredient.GetComponent<CollectableFood>().CollectableFoodID;
                    hand_amount = 1;
                    //GotEmptyCup = true;
                    AudioManager.instance.PlayRandom(FmodEvents.instance.collect, this.transform.position);
                    if (UIFinder("WrongIngredient").activeSelf)
                    {
                        //var IngredientWarn = UIFinder("WrongIngredient").GetComponent<IngredientIndicator>();
                        UIFinder("WrongIngredient").SetActive(false);
                    }
                    if (UIFinder("BusyKopiMaker").activeSelf)
                    {
                        UIFinder("BusyKopiMaker").SetActive(false);
                    }
                }
                var canMergeFood = NearMergePoint && Mergeable && hand_amount == 1 && Input.GetKeyDown(KeyCode.E) && !NearCollectionPoint 
                    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.READY;
                if (canMergeFood)
                {
                    hand_amount = 0;
                    Destroy(holdIngredient);
                    currentKopiMaker.GetComponent<MergeIngredient>().ChangeState(MergeIngredient.KopiMakerStates.PREP);
                }
                var canCollectKopi = NearMergePoint && hand_amount < 1 && Input.GetKeyDown(KeyCode.E) && !NearCollectionPoint
                    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.COMPLETE 
                    && currentKopiMaker.GetComponent<MergeIngredient>().makerType == MergeIngredient.MakerTypes.DRINK;
                if (canCollectKopi) 
                {
                    hand_amount = 2;
                    holdFood = Instantiate(currentFoodCollectable, hand);
                    currentKopiMaker.GetComponent<MergeIngredient>().ChangeState(MergeIngredient.KopiMakerStates.READY);
                    AudioManager.instance.PlayRandom(FmodEvents.instance.collect, this.transform.position);
                    //show throw UI
                    UIFinder("WrongIngredient").SetActive(false);
                    UIFinder("ActivateThrowmode").transform.GetChild(0).GetComponent<Image>().sprite = ThrowPrompts[1];
                    UIFinder("ActivateThrowmode").SetActive(true);
                    if (UIFinder("BusyKopiMaker").activeSelf)
                    {
                        UIFinder("BusyKopiMaker").SetActive(false);
                    }
                    throwscript.objectToThrow = currentFoodThrowable.GetComponent<Rigidbody>();
                    canThrow = true;
                }
                var canCollectMidToast = NearMergePoint && hand_amount < 1 && Input.GetKeyDown(KeyCode.E) && !NearCollectionPoint 
                    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.COMPLETE
                    && currentKopiMaker.GetComponent<MergeIngredient>().makerType == MergeIngredient.MakerTypes.TOAST;
                if (canCollectMidToast)
                {
                    hand_amount = 3;
                    holdFood = Instantiate(currentFoodCollectable, hand);
                    currentKopiMaker.GetComponent<MergeIngredient>().ChangeState(MergeIngredient.KopiMakerStates.READY);
                    AudioManager.instance.PlayRandom(FmodEvents.instance.collect, this.transform.position);
                }
                //if you are trying to merge a mid toast to full toast when you are near the kaya spreading spot, press E key,
                var canSpreadToast = NearSpreadKayaPoint && hand_amount == 3 && Input.GetKeyDown(KeyCode.E) && !NearCollectionPoint && !NearMergePoint;
                if (canSpreadToast)
                {
                    hand_amount = 0;
                    ToastedBread = Instantiate(holdFood, currentKayaMachine.breadLocation);
                    Destroy(holdFood);
                    currentKayaMachine.SliderVisibility(currentKayaMachine.spreadBreadProgressBar, true);
                    currentKayaMachine.ChangeState(SpreadKaya.KayaMakerStates.PREP);
                    AudioManager.instance.PlayRandom(FmodEvents.instance.collect, this.transform.position);
                }
                //if player press e at this state and is near kaya station and hand amount == 0 then change to ready state (this one must put on player controller side)
                var canCollectFullToast = NearSpreadKayaPoint && hand_amount == 0 && Input.GetKeyDown(KeyCode.E) && currentKayaMachine.currentState == SpreadKaya.KayaMakerStates.COMPLETE;
                if (canCollectFullToast)
                {
                    hand_amount = 2;
                    currentKayaMachine.ChangeState(SpreadKaya.KayaMakerStates.READY);
                    //kaya machine should store the collectable & throwable (have not done)
                    //instantiate kaya machine collectable to holdfood (have not done)
                    print("Throw toast now");
                    UIFinder("ActivateThrowmode").transform.GetChild(0).GetComponent<Image>().sprite = ThrowPrompts[1];
                    UIFinder("ActivateThrowmode").SetActive(true);
                    if (UIFinder("BusyKopiMaker").activeSelf)
                    {
                        UIFinder("BusyKopiMaker").SetActive(false);
                    }
                    throwscript.objectToThrow = currentFoodThrowable.GetComponent<Rigidbody>();
                    canThrow = true;
                }
                //you are going to merge an empty cup into a coffee but hand amount = 0 and not holding any empty cup ingredient
                var showGetCupWarning = CurrentHoldIngredientID == 0 && Input.GetKeyDown(KeyCode.E) && hand_amount == 0 && NearMergePoint
                    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.READY 
                    && currentKopiMaker.GetComponent<MergeIngredient>().makerType == MergeIngredient.MakerTypes.DRINK 
                    || CurrentHoldIngredientID == 2 && Input.GetKeyDown(KeyCode.E) && hand_amount == 1 && NearMergePoint
                    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.READY 
                    && currentKopiMaker.GetComponent<MergeIngredient>().makerType == MergeIngredient.MakerTypes.DRINK;
                if (showGetCupWarning) {
                    //show warning UI
                    UIFinder("WrongIngredient").SetActive(true);
                    UIFinder("WrongIngredient").GetComponent<Animator>().Play("PulsingThrowPromptUI");
                    var IngredientWarn = UIFinder("WrongIngredient").GetComponent<IngredientIndicator>();
                    IngredientWarn.UpdateIngredient(2);
                }
                //if I interact with the merge point without touching canCollectIngredient
                if (Input.GetMouseButtonDown(1) && PressCount == 0 && canThrow)
                {
                    PressCount = 1;
                    ChangeState(PlayerCollection.THROW);
                    UIFinder("ActivateThrowmode").SetActive(false);
                    lr.enabled = true;
                }
                //if both of the kopimaker is not ready, and player try to get cup by pressing E and its near collection point cannot collect, have error message pop up
                var CannotCollectIngre = spawner.NoOfKopiMakerBusy && Input.GetKeyDown(KeyCode.E) && NearCollectionPoint;
                if (CannotCollectIngre)
                {
                    Destroy(holdIngredient);
                    hand_amount = 0;
                    UIFinder("BusyKopiMaker").SetActive(true);
                    UIFinder("BusyKopiMaker").GetComponent<Animator>().Play("PulsingThrowPromptUI");
                }

                break;

            case PlayerCollection.THROW:
                //player can only throw after collecting
                if (Input.GetMouseButtonDown(0) && PressCount == 1)
                {
                    PressCount = 2;
                    throwscript.ThrowObject();
                    Destroy(holdFood);
                    AudioManager.instance.PlayOneShot(FmodEvents.instance.throwing, this.transform.position);
                    UIFinder("ActivateThrowmode").transform.GetChild(0).GetComponent<Image>().sprite = ThrowPrompts[0];
                    UIFinder("ActivateThrowmode").SetActive(true);
                    canThrow = false;
                }
                else if (Input.GetMouseButtonDown(1) && PressCount == 2)
                {
                    PressCount = 0;
                    hand_amount = 0;
                    UIFinder("ActivateThrowmode").SetActive(false);
                    ChangeState(PlayerCollection.COLLECT);
                }
                break;
        }
    }
    public void ChangeState(PlayerCollection newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }
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


