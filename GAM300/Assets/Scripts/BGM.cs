using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGM : MonoBehaviour
{
    //audio
    public EventInstance PlayBGM;
    private void Start()
    {
        PlayBGM = AudioManager.instance.CreateInstance(FmodEvents.instance.gameBGM);
        PlayBGM.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Vector3.zero));
        PlayMusic();
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
