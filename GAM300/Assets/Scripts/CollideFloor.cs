using System.Collections;
using UnityEngine;

public class CollideFloor : MonoBehaviour
{
    [Header("Error VFX")]
    public VFX vfxScript;
    private void OnCollisionEnter(Collision other)
    {
        var tag = other.gameObject.tag;
        switch (tag)
        {
            case "Food":
                print("Broke something");
                vfxScript.PlayVFX(vfxScript.FindVFX("ErrorBurst"), other.transform, 1.5f);
                AudioManager.instance.PlayOneShot(FmodEvents.instance.HitOtherAreas, other.transform.position);
                Destroy(other.gameObject, 1.5f);
                break;
        }
    }
    //IEnumerator DelayCrashSFX(float delay, Transform here)
    //{
    //    yield return new WaitForSeconds(delay);
    //    AudioManager.instance.PlayRandom(FmodEvents.instance.crash, here.position);
    //}
}
