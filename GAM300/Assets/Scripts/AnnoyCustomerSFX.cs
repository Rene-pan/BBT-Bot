using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnnoyCustomerSFX : MonoBehaviour
{
    public void JumpedSFX()
    {
        AudioManager.instance.PlayOneShot(FmodEvents.instance.CustomerJump, transform.position);   
    }

    public void LandSFX()
    {
        AudioManager.instance.PlayOneShot(FmodEvents.instance.F_JumpLand, transform.position);
    }
}
