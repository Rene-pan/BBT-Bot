using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public enum PlatformTypes
    {
        FRONTBACK, UPDOWN, LEFTRIGHT
    }
    public PlatformTypes PlatformTYPE;
    public float amp;
    public float freq;
    Vector3 initPos;
    void Start()
    {
        initPos = transform.position;
    }
    void Update()
    {
        if (PlatformTYPE == PlatformTypes.UPDOWN)
        {
            transform.position = new Vector3(initPos.x, ((Mathf.Sin(Time.time * freq) * amp) + initPos.y), initPos.z);
        }
        else if (PlatformTYPE == PlatformTypes.FRONTBACK)
        {
            transform.position = new Vector3(((Mathf.Sin(Time.time * freq) * amp) + initPos.x), initPos.y, initPos.z);
        }
        else if (PlatformTYPE == PlatformTypes.LEFTRIGHT)
        {
            transform.position = new Vector3(initPos.x, initPos.y, ((Mathf.Sin(Time.time * freq) * amp) + initPos.z));
        }
    }
}
