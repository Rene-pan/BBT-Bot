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
    bool MovingNow;

    [Header("Player Animations")]
    public Animator PlayerAnim;

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
        //    PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
        //    MoveVector = transform.TransformDirection(PlayerMovementInput) * moveSpeed;
        //    playerbody.velocity = new Vector3(MoveVector.x, playerbody.velocity.y, MoveVector.z);

        //if (PlayerMovementInput.magnitude >= 0.1f && cam.gameObject.GetComponent<CamController_v3>().currentState == CamController_v3.CamState.THIRDPERSON)
        //{
        //    float targetAngle = Mathf.Atan2(PlayerMovementInput.x, 0) * Mathf.Rad2Deg + cam.eulerAngles.y;
        //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        //    transform.rotation = Quaternion.Euler(0, angle, 0);
        //    Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        //}
        //if (PlayerMovementInput.magnitude >= 0.1f && cam.gameObject.GetComponent<CamController_v3>().currentState == CamController_v3.CamState.OVERSHOULDER)
        //{
        //    float targetAngle = Mathf.Atan2(PlayerMovementInput.x,0) * Mathf.Rad2Deg + Newcam.eulerAngles.y;
        //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        //    transform.rotation = Quaternion.Euler(0, angle, 0);
        //    Vector3 moveDir = Quaternion.Euler(0, 0, 0) * Vector3.back;
        //}
        // Get player input
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;

        // Check if there is any movement input
        if (PlayerMovementInput.magnitude >= 0.1f)
        {
            MovingNow = true;
            float targetAngle = 0.0f;
            // Calculate movement direction relative to the camera
            if (cam.gameObject.GetComponent<CamController_v3>().currentState == CamController_v3.CamState.THIRDPERSON)
                targetAngle = Mathf.Atan2(PlayerMovementInput.x, PlayerMovementInput.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            else if (cam.gameObject.GetComponent<CamController_v3>().currentState == CamController_v3.CamState.OVERSHOULDER)
                targetAngle = Mathf.Atan2(PlayerMovementInput.x, PlayerMovementInput.z) * Mathf.Rad2Deg + Newcam.eulerAngles.y;

            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rotate the player gradually to face the movement direction
            // Over the shoulder has that weird camera shake when moving, so this will only apply to 3rd person camera
            if (cam.gameObject.GetComponent<CamController_v3>().currentState == CamController_v3.CamState.THIRDPERSON)
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            // Move the player in the direction the camera is facing
            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            MoveVector = moveDirection * moveSpeed;
            playerbody.velocity = new Vector3(MoveVector.x, playerbody.velocity.y, MoveVector.z);
        }
        else
        {
            // If no input, maintain player's vertical velocity and stop horizontal movement
            MovingNow = false;
            playerbody.velocity = new Vector3(0, playerbody.velocity.y, 0);
            //transform.rotation = Quaternion.Euler(0f, 0, 0f);
        }

        if (MovingNow)
        {
            PlayerAnim.SetBool("MoveTrue", true);
        }else
        {
            PlayerAnim.SetBool("MoveTrue", false);
        }
    }

    private void UpdatePlayerMovementSFX()
    {
        if (MovingNow)
        {
            //Player has moved
            PLAYBACK_STATE playbackState;
            playerMovement.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED)) 
            {
                playerMovement.start();
            }
        }
        else if (!MovingNow)
        {
            //Player has not moved
            playerMovement.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

}
