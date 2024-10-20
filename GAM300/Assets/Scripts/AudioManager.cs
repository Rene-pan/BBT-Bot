using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public static List<EventInstance> eventInstances = new List<EventInstance>();

    //private string BankName;
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (instance == null)
        {
            instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);
        //StopAllSounds();
    }
  
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public void PlayOneShot2D(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }
    public void PlayRandom(List<EventReference> sounds, Vector3 worldPos)
    {
        var soundID = Random.Range(0, sounds.Count);
        RuntimeManager.PlayOneShot(sounds[soundID], worldPos);
    }
    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }
    public void StopAllSounds()
    {
        RuntimeManager.GetBus("bus:/Music_SFX").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //foreach (var sound in eventInstances) { }
    }

    public void PauseSounds(bool i)
    {
        RuntimeManager.GetBus("bus:/Music_SFX").setPaused(i);
    }
}
