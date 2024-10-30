using FMOD.Studio;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Ambience : MonoBehaviour
{
    //audio
    private EventInstance PlayAmbience;
    public EventInstance PlayBGM;
    //track current time
    public MainTimer timer;
    public float StartFastBGMTime;
    bool SwitchOnce = false;

    //switching between music tracks stuff
    public enum BGMStates
    {
        ORG_BGM = 0,
        FAST_BGM = 1
    }
    public BGMStates currentState = BGMStates.ORG_BGM;
  
    void Start()
    {
        AmbiencePlay();
        PlayBGM = AudioManager.instance.CreateInstance(FmodEvents.instance.gameBGM);
        PlayBGM.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Vector3.zero));
        PlayMusic();

    }
    public int count = 0;
    private void Update()
    {
        //var pressB = Input.GetKeyDown(KeyCode.B);
        //if (count == 0 && pressB)
        //{
        //    AudioManager.instance.SwitchTrack(1, PlayBGM);
        //    count++;
        //}
        //else if (count == 1 && pressB)
        //{
        //    AudioManager.instance.SwitchTrack(0, PlayBGM);
        //    count--;
        //}
        //if (Input.GetKeyDown(KeyCode.B)){
        //    timer.currentTime = 65;
        //}
        if (timer.currentTime <= StartFastBGMTime && !SwitchOnce) 
        {
            AudioManager.instance.SwitchTrack(1, PlayBGM);
            SwitchOnce = true;
        }
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

    public void ResetMusic()
    {
        AudioManager.instance.SwitchTrack(0, PlayBGM);
        SwitchOnce = false;
    }
}
