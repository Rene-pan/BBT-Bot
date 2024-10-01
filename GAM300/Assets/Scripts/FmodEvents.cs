using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FmodEvents : MonoBehaviour
{
    [field:Header("Collect SFX")]
    [field: SerializeField] public List<EventReference> collect { get; private set; }

    [field: Header("Crash SFX")]
    [field: SerializeField] public List<EventReference> crash { get; private set; }

    [field: Header("Kopi Movements SFX")]
    [field: SerializeField] public List<EventReference> kopiMovements { get; private set; }
    [field: Header("Eat SFX")]
    [field: SerializeField] public List<EventReference> eats { get; private set; }
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
    [field: SerializeField] public List<EventReference> foodLandSuccess { get; private set; }
    [field: Header("Game Over SFX")]
    [field: SerializeField] public List<EventReference> gameOver { get; private set; }

    [field: Header("Success SFX")]
    [field: SerializeField] public List<EventReference> gameSuccess { get; private set; }



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
