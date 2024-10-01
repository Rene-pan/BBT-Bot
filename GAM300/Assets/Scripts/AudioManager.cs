using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    //private string BankName;
    private void Awake()
    {
        if (instance != null)
        {
            print("Found more than one Audio Manager in the scene.");
            return;
        }
        instance = this;
        //FMODUnity.RuntimeManager.StudioSystem.getBank();
    }
    private void OnDestroy()
    {
        DontDestroyOnLoad(instance);
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
        return eventInstance;
    }
    public void StopSounds()
    {
        RuntimeManager.GetBus("Bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    

}
