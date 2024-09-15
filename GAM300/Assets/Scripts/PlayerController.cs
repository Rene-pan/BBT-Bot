using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void Update()
    {
        Move();
        Collect();
        Merge();
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
        var canMerge = hand_amount == 1 && NearMergePoint && Input.GetKeyDown(KeyCode.E);
        if (canMerge)
        {
            Destroy(currentIngredient);
            currentFood = Instantiate(foods[0],hand);
        }
    }
}
