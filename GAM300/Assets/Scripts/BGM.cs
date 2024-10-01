using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    //audio
    private EventInstance PlayBGM;
    private void Start()
    {
        PlayBGM = AudioManager.instance.CreateInstance(FmodEvents.instance.gameBGM);
        PlayBGM.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject.transform));
        PLAYBACK_STATE playbackState;
        PlayBGM.getPlaybackState(out playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            PlayBGM.start();
        }
    }
}
