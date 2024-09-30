using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FmodEvents : MonoBehaviour
{
    [field:Header("Collect SFX")]
    [field: SerializeField] public EventReference collect { get; private set; }
    public static FmodEvents instance {  get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("more than 1 Fmod");
        }
        instance = this;
    }
}
