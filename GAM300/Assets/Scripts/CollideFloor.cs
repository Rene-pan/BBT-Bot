using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollideFloor : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        var tag = other.gameObject.tag;
        switch (tag)
        {
            case "Food":
                print(other);
                AudioManager.instance.PlayRandom(FmodEvents.instance.crash, this.transform.position);
                Destroy(other.gameObject);
                break;
        }
    }
}
