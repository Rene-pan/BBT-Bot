using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Vector3 PlayerMouseInput;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity = 2;

    [SerializeField] Rigidbody playerbody;
    [SerializeField] Transform cam;
    [SerializeField] CamController_v1 camScript;

    [Header("Grab ingredient")]
    public bool NearCollectionPoint = false;
    public bool NearMergePoint = false;
    [SerializeField] GameObject[] ingredients;
    [SerializeField] GameObject[] foods;
    public int ingredientNo;
    public int foodNo;
    public Transform hand;
    public int hand_amount = 0;
    public GameObject currentIngredient;
    public GameObject currentFood;
    public GameObject currentKopiMaker;


    [Header("Throwing")]
    public bool canThrow;
    private ProjectileThrow throwscript;
    public float minThrowDistance;
    [SerializeField] Slider throwStrength;
    [SerializeField] int maxStrengthValue;


    private void Start()
    {
        throwscript = FindAnyObjectByType<ProjectileThrow>();
        SetSlider(throwStrength,maxStrengthValue,throwscript.force);
    }
    private void Update()
    {
        Move();
        Collect();
        Merge();
        Throw();
    }
    void Move()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * moveSpeed;
        playerbody.velocity = new Vector3(MoveVector.x, playerbody.velocity.y, MoveVector.z);
        if (PlayerMovementInput.magnitude >= 0.1f && camScript.currentState == CamController_v1.CamState.THIRDPERSON)
        {
            float targetAngle = Mathf.Atan2(PlayerMovementInput.x, PlayerMovementInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
    }

    void Collect()
    {
        var canCollect = NearCollectionPoint && Input.GetKeyDown(KeyCode.E);
        if (canCollect && hand_amount <1)
        {
            currentIngredient = Instantiate(ingredients[ingredientNo],hand);
            hand_amount += 1;
        }
        
    }

    private void Parent(Transform Parent, GameObject child, int state)
    {
        switch (state)
        {
            case 0:
                child.transform.SetParent(Parent);
                break;
            case 1:
                child.transform.SetParent(null);
                break;
        }
    }

    void Merge()
    {
        var startCookingTimer = hand_amount == 1 && NearMergePoint && Input.GetKeyDown(KeyCode.E);
        if (currentKopiMaker == null) return;
        if (startCookingTimer)
        {
            Destroy(currentIngredient);
            var KopiMakerScript = currentKopiMaker.GetComponent<MergeIngredient>();
            KopiMakerScript.ChangeState(MergeIngredient.KopiMakerStates.PREP);
            //currentFood = Instantiate(foods[0],hand);
        }
    }

    void Throw()
    {
        if (camScript.currentState == CamController_v1.CamState.THIRDPERSON)
        {
            canThrow = false;
            SliderVisibility(throwStrength.gameObject, canThrow);
        }
        else if (camScript.currentState == CamController_v1.CamState.FIRSTPERSON)
        {
            canThrow = true;
            SliderVisibility(throwStrength.gameObject,canThrow);
        }
        if (canThrow && Input.GetMouseButton(0))
        {
            throwscript.force += 1;
            UpdateSlider(throwStrength, throwscript.force);
        }
        else if (canThrow && throwscript.force >= minThrowDistance)
        {
            print("Reduce boost");
            throwscript.force -= 1;
            UpdateSlider(throwStrength, throwscript.force);
        }
        if (canThrow && Input.GetMouseButtonDown(1))
        {
            throwscript.ThrowObject();
            canThrow = false;
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
