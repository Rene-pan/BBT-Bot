using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX : MonoBehaviour
{
    public List<GameObject> VFXprefabs;

    public GameObject FindVFX (string VFXname)
    {
        foreach (var vfx in VFXprefabs)
        {
            if (vfx.name == VFXname)
            {
                return vfx;
            }
        }
            return null;
    }

    public void PlayVFX (GameObject VFX, Transform PlayPosition, float DestroyDelay)
    {
        var currentVFX = Instantiate(VFX, PlayPosition);
        Destroy(currentVFX, DestroyDelay);
    }
}
