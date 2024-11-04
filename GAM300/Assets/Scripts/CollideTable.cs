using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideTable : MonoBehaviour
{
    public bool Fail = false;
    [Header("Error VFX")]
    public VFX vfxScript;
    private void OnCollisionEnter(Collision other)
    {
        var tag = other.gameObject.tag;
        switch (tag)
        {
            case "Food":
                {
                    vfxScript.PlayVFX(vfxScript.FindVFX("ErrorBurst"), other.transform, 1.5f);
                    AudioManager.instance.PlayOneShot(FmodEvents.instance.HitOtherAreas, other.transform.position);
                    Destroy(other.gameObject, 1f);
                    Fail= true;
                    break;
                }
        }
    }
    private void OnCollisionExit(Collision other)
    {
        var tag = other.gameObject.tag;
        switch (tag)
        {
            case "Food":
                {
                    Fail = false;
                    break;
                }
        }
    }
}