using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CamController_v3;

public class PlayerController_v2 : MonoBehaviour
{
    [SerializeField] CamController_v3 camScript;
    public enum PlayerCollection {COLLECT, THROW}
    public PlayerCollection currentState; 
    [Header("Grab ingredient")]
    public bool NearCollectionPoint = false;
    public bool NearMergePoint = false;
    public bool GotEmptyCup = false;
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
    public float time;
    [SerializeField] List<GameObject> PlayerUI;
    public Sprite[] ThrowPrompts; //org, new
    public int PressCount = 0;

    private void Start()
    {
        //find throw script
        throwscript = FindAnyObjectByType<newThrow>();
        throwscript.lr.enabled = false;
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
                    hand_amount = 1;
                    GotEmptyCup = true;
                    UIFinder("CollectACupFirst").SetActive(false);
                }
                var canMergeFood = NearMergePoint && hand_amount == 1 && Input.GetKeyDown(KeyCode.E) && !NearCollectionPoint 
                    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.READY;
                if (canMergeFood)
                {
                    hand_amount = 0;
                    Destroy(holdIngredient);
                    currentKopiMaker.GetComponent<MergeIngredient>().ChangeState(MergeIngredient.KopiMakerStates.PREP);
                    GotEmptyCup = false;
                }
                var canCollectFood = NearMergePoint && hand_amount < 1 && Input.GetKeyDown(KeyCode.E) && !NearCollectionPoint
                    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.COMPLETE;
                if (canCollectFood)
                {
                    hand_amount = 2;
                    holdFood = Instantiate(currentFoodCollectable, hand);
                    currentKopiMaker.GetComponent<MergeIngredient>().ChangeState(MergeIngredient.KopiMakerStates.READY);
                    GotEmptyCup = false;
                    //show throw UI
                    UIFinder("CollectACupFirst").SetActive(false);
                    UIFinder("ActivateThrowmode").transform.GetChild(0).GetComponent<Image>().sprite = ThrowPrompts[1];
                    UIFinder("ActivateThrowmode").SetActive(true);
                }
                //you are going to merge food but hand amount = 0;
                var showGetCupWarning = !GotEmptyCup && Input.GetKeyDown(KeyCode.E) && hand_amount == 0 && NearMergePoint
                    && currentKopiMaker.GetComponent<MergeIngredient>().currentState == MergeIngredient.KopiMakerStates.READY;
                if (showGetCupWarning) {
                    //show warning UI
                    UIFinder("CollectACupFirst").SetActive(true);
                    UIFinder("CollectACupFirst").GetComponent<Animator>().Play("PulsingThrowPromptUI");
                }
               //if I interact with the merge point without touching canCollectIngredient
               if (Input.GetMouseButtonDown(1) && PressCount == 0)
                {
                    ChangeState(PlayerCollection.THROW);
                    PressCount = 1;
                    UIFinder("ActivateThrowmode").SetActive(false);
                }

                break;

            case PlayerCollection.THROW:
                //player can only throw after collecting
                if (Input.GetMouseButtonDown(0) && PressCount ==1)
                {
                    PressCount = 2;
                    throwscript.Throw();
                    Destroy(holdFood);
                    UIFinder("ActivateThrowmode").transform.GetChild(0).GetComponent<Image>().sprite = ThrowPrompts[0];
                    UIFinder("ActivateThrowmode").SetActive(true);
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


