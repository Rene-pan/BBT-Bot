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
                Destroy(other.gameObject);
                break;
        }
    }
}
