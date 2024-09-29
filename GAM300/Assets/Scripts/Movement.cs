using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity = 2;

    [SerializeField] Rigidbody playerbody;
    public Transform cam;

    private void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        Vector3 MoveVector = transform.TransformDirection(PlayerMovementInput) * moveSpeed;
        playerbody.velocity = new Vector3(MoveVector.x, playerbody.velocity.y, MoveVector.z);
        if (PlayerMovementInput.magnitude >= 0.1f )
        {
            float targetAngle = Mathf.Atan2(PlayerMovementInput.x, PlayerMovementInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
    }
}
