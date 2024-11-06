using FMOD.Studio;
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

    //audio
    EventInstance MovingComplains;
    void Start()
    {
        initPos = transform.position;
        //music
        MovingComplains = AudioManager.instance.CreateInstance(FmodEvents.instance.MovingEnter);
        MovingComplains.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(initPos));
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

    private void OnTriggerEnter(Collider other)
    {
        // if player collides with this person, play a sound
        var tag = other.tag;
        if (tag == "Player")
        {
            PLAYBACK_STATE playbackState;
            MovingComplains.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                MovingComplains.start();
            }
        }
    }
}
