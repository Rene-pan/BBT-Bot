using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollideFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var tag = other.tag;
        switch (tag)
        {
            case "Food":
                print(other);
                Destroy(other);
                break;
        }
    }
}
