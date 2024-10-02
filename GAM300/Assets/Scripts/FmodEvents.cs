using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using Unity.VisualScripting;
using FMOD.Studio;

public class FmodEvents : MonoBehaviour
{
    public List<EventInstance> events;
    [field: SerializeField] public List<EventReference> collect { get; private set; }

    [field: Header("Crash SFX")]
    [field: SerializeField] public List<EventReference> crash { get; private set; }

    [field: Header("Kopi Movements SFX")]
    [field: SerializeField] public EventReference kopiMovements { get; private set; }
    [field: Header("Eat SFX")]
    [field: SerializeField] public EventReference eats { get; private set; }
    [field: Header("Cooking Complete SFX")]
    [field: SerializeField] public EventReference cookingComplete { get; private set; }

    [field: Header("Throwing SFX")]
    [field: SerializeField] public EventReference throwing { get; private set; }

    [field: Header("Drink Making SFX")]
    [field: SerializeField] public EventReference drinkMaking { get; private set; }

    [field: Header("Food Burning Timer SFX")]
    [field: SerializeField] public EventReference foodBurn { get; private set; }

    [field: Header("Food Cooking SFX")]
    [field: SerializeField] public List<EventReference> foodCooking { get; private set; }

    [field: Header("Food Land Success SFX")]
    [field: SerializeField] public EventReference foodLandSuccess { get; private set; }
    [field: Header("Game Over SFX")]
    [field: SerializeField] public EventReference gameOver { get; private set; }

    [field: Header("Success SFX")]
    [field: SerializeField] public EventReference gameSuccess { get; private set; }

    [field: Header("BGM SFX")]
    [field: SerializeField] public EventReference gameBGM { get; private set; }

    
    [field: Header("Ambience SFX")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference UI_Interact { get; private set; }

    [field: Header("Earn Money SFX")]
    [field: SerializeField] public EventReference EarnMoney { get; private set; }



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
