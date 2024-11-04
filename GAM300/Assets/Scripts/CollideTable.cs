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
                    var vfx  = vfxScript.CreateVFX(vfxScript.FindVFX("ErrorBurst"), other.transform);
                    AudioManager.instance.PlayOneShot(FmodEvents.instance.HitOtherAreas, vfx.transform.position);
                    vfx.transform.parent = null;
                    Destroy(vfx, 3f);
                    Destroy(other.gameObject);
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