using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            print("Found more than one Audio Manager in the scene.");
        }
        instance = this;
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
}
