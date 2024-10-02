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
        if (instance != null)
        {
            print("Found more than one Audio Manager in the scene.");
        }
        instance = this;
    }
    private void OnDestroy()
    {
        DontDestroyOnLoad(instance);
        print("HELP");
    }
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
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
    //public void StopSounds()
    //{
    //    foreach (var sound in eventInstances)
    //    {
    //        sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    //    }

    //}
    public void StopAllSounds()
    {
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    

}
