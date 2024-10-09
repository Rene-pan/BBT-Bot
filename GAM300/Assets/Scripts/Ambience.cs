using FMOD.Studio;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambience : MonoBehaviour
{
    //audio
    private EventInstance PlayAmbience;
    public EventInstance PlayBGM;
  
    void Start()
    {
        AmbiencePlay();
        PlayBGM = AudioManager.instance.CreateInstance(FmodEvents.instance.gameBGM);
        PlayBGM.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Vector3.zero));
        PlayMusic();

    }
   public void AmbiencePlay()
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
    public void PlayMusic()
    {
        PLAYBACK_STATE playbackState;
        PlayBGM.getPlaybackState(out playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            PlayBGM.start();
        }
    }
    public void StopMusic()
    {
        PlayBGM.stop(STOP_MODE.IMMEDIATE);
    }
}
