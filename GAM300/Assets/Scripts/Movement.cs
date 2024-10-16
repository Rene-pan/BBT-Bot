using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;

public class Movement : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity = 2;

    [SerializeField] Rigidbody playerbody;
    public Transform cam;
    public Transform Newcam;
    public Vector3 MoveVector;

    //audio
    private EventInstance playerMovement;

    private void Start()
    {
        playerMovement = AudioManager.instance.CreateInstance(FmodEvents.instance.kopiMovements);
        playerMovement.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(playerbody.gameObject.transform));
    }
    private void FixedUpdate()
    {
        Move();
        UpdatePlayerMovementSFX();
    }
    void Move()
    {
            PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            MoveVector = transform.TransformDirection(PlayerMovementInput) * moveSpeed;
            playerbody.velocity = new Vector3(MoveVector.x, playerbody.velocity.y, MoveVector.z);

        if (PlayerMovementInput.magnitude >= 0.1f && cam.gameObject.GetComponent<CamController_v3>().currentState == CamController_v3.CamState.THIRDPERSON)
        {
            float targetAngle = Mathf.Atan2(PlayerMovementInput.x, 0) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        }
        if (PlayerMovementInput.magnitude >= 0.1f && cam.gameObject.GetComponent<CamController_v3>().currentState == CamController_v3.CamState.OVERSHOULDER)
        {
            float targetAngle = Mathf.Atan2(PlayerMovementInput.x,0) * Mathf.Rad2Deg + Newcam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            Vector3 moveDir = Quaternion.Euler(0, 0, 0) * Vector3.back;
        }
    }

    private void UpdatePlayerMovementSFX()
    {
        if (MoveVector != Vector3.zero)
        {
            //Player has moved
            PLAYBACK_STATE playbackState;
            playerMovement.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED)) 
            {
                playerMovement.start();
            }
        }
        else
        {
            //Player has not moved
            playerMovement.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

}
