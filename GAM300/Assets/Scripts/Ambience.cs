using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambience : MonoBehaviour
{
    //audio
    private EventInstance PlayAmbience;
    void Start()
    {
        PlayAmbience = AudioManager.instance.CreateInstance(FmodEvents.instance.ambience);
        PlayAmbience.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        PLAYBACK_STATE playbackState;
        PlayAmbience.getPlaybackState(out playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            PlayAmbience.start();
        }
    }
}
