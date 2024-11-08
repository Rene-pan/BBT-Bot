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
    public float trackedValue; //for sound, if value is positive, means its rising, if its negative means its dropping


    //audio
    EventInstance MovingComplains;
    EventInstance Moving;
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
            trackedValue = ((Mathf.Sin(Time.time * freq) * amp));
            transform.position = new Vector3(initPos.x, ((Mathf.Sin(Time.time * freq) * amp) + initPos.y), initPos.z);
            if (ActivateSFX)
            {
                PlayPlatformSFX(trackedValue);
            }
        }
        else if (PlatformTYPE == PlatformTypes.FRONTBACK)
        {
            trackedValue = ((Mathf.Sin(Time.time * freq) * amp));
            transform.position = new Vector3(((Mathf.Sin(Time.time * freq) * amp) + initPos.x), initPos.y, initPos.z);
        }
        else if (PlatformTYPE == PlatformTypes.LEFTRIGHT)
        {
            trackedValue = ((Mathf.Sin(Time.time * freq) * amp));
            transform.position = new Vector3(initPos.x, initPos.y, ((Mathf.Sin(Time.time * freq) * amp) + initPos.z));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if player collides with this person, play a sound
        var tag = other.tag;
        if (tag == "Player")
        {
            if (PlatformTYPE == PlatformTypes.LEFTRIGHT)
            {
                PLAYBACK_STATE playbackState;
                MovingComplains.getPlaybackState(out playbackState);
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                {
                    MovingComplains.start();
                }
            }
            else if (PlatformTYPE == PlatformTypes.UPDOWN)
            {
                ActivateSFX = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var tag = other.tag;
        if (tag == "Player")
        {
            if (PlatformTYPE == PlatformTypes.UPDOWN)
            {
                ActivateSFX = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var tag = other.tag;
        if (tag == "Player")
        {
            if (PlatformTYPE == PlatformTypes.UPDOWN)
            {
                ActivateSFX = false;
            }
        }
    }
    int count = 0;
    bool ActivateSFX = false;
    void PlayPlatformSFX(float trackedValue)
    {
        if (trackedValue >= 0 && count == 0)
        {
            Moving = AudioManager.instance.CreateInstance(FmodEvents.instance.PlatformRiseSFX);
            Moving.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            Moving.start();
            count = 1;
            PLAYBACK_STATE playbackState;
            Moving.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED) && count == 1)
            {
                Moving.stop(STOP_MODE.IMMEDIATE);
            }
        }
        else if (trackedValue < 0 && count == 1)
        {
            Moving = AudioManager.instance.CreateInstance(FmodEvents.instance.PlatformDropSFX);
            Moving.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
            Moving.start();
            count = 0;
            PLAYBACK_STATE playbackState;
            Moving.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED) && count == 0)
            {
                Moving.stop(STOP_MODE.IMMEDIATE);
            }
        }
    }
}
