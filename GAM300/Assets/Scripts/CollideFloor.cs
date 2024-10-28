using System.Collections;
using UnityEngine;

public class CollideFloor : MonoBehaviour
{
    [Header("Error VFX")]
    public GameObject ErrorVFX;
    private void OnCollisionEnter(Collision other)
    {
        var tag = other.gameObject.tag;
        switch (tag)
        {
            case "Food":
                print("Broke something");
                var MoreSuddenBurst = Instantiate(ErrorVFX, other.transform);
                Destroy(MoreSuddenBurst, 3);
                AudioManager.instance.PlayRandom(FmodEvents.instance.crash, other.transform.position);
                Destroy(other.gameObject, 2);
                break;
        }
    }
}
