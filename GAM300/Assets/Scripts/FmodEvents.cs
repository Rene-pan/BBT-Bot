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
    [field: SerializeField] public List<EventReference> eats { get; private set; }
    [field: Header("Cooking Complete SFX")]
    [field: SerializeField] public EventReference cookingComplete { get; private set; }

    [field: Header("Throwing SFX")]
    [field: SerializeField] public EventReference throwing { get; private set; }

    [field: Header("Drink Making SFX")]
    [field: SerializeField] public EventReference drinkMaking { get; private set; }

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

    [field: Header("Place Food SFX")]
    [field: SerializeField] public EventReference PlaceFood { get; private set; }

    [field: Header("Toast Bread SFX")]
    [field: SerializeField] public EventReference ToastBread { get; private set; }

    [field: Header("Toast Bread Complete SFX")]
    [field: SerializeField] public EventReference ToastBreadComplete { get; private set; }

    [field: Header("Customer Movement SFX")]
    [field: SerializeField] public EventReference CustomerMovement { get; private set; }

    [field: Header("Lose Money SFX")]
    [field: SerializeField] public EventReference LoseMoney { get; private set; }

    [field: Header("Time Ring SFX")]
    [field: SerializeField] public EventReference TimeRing { get; private set; }

    [field: Header("Kaya Spread SFX")]
    [field: SerializeField] public EventReference KayaSpread { get; private set; }

    [field: Header("Hit Table SFX")]
    [field: SerializeField] public EventReference HitTable { get; private set; }

    [field: Header("Hit Other Areas SFX")]
    [field: SerializeField] public EventReference HitOtherAreas { get; private set; }

    [field: Header("Drinking SFX")]
    [field: SerializeField] public EventReference Drinking { get; private set; }

    [field: Header("Tear Order SFX")]
    [field: SerializeField] public EventReference OrderFail { get; private set; }

    [field: Header("Activate Throwmode SFX")]
    [field: SerializeField] public EventReference ActivateThrow { get; private set; }

    [field: Header("Customer Jump SFX")]
    [field: SerializeField] public EventReference CustomerJump { get; private set; }

    [field: Header("Order Created SFX")]
    [field: SerializeField] public EventReference OrderCreated { get; private set; }

    [field: Header("Customer Angry Male SFX")]
    [field: SerializeField] public EventReference M_CustomerAngry { get; private set; }

    [field: Header("Customer Angry Female SFX")]

    [field: SerializeField] public EventReference F_CustomerAngry { get; private set; }

    [field: Header("KOPI floating SFX")]

    [field: SerializeField] public EventReference Float { get; private set; }

    [field: Header("Moving Wall SFX")]
    [field: SerializeField] public EventReference MovingEnter { get; private set; }

    [field: Header("Karen Order SFX")]
    [field: SerializeField] public EventReference KarenOrder { get; private set; }

    [field: Header("Male Kopi Order SFX")]
    [field: SerializeField] public EventReference M_KopiOrder { get; private set; }

    [field: Header("Female Kopi Order SFX")]
    [field: SerializeField] public EventReference F_KopiOrder { get; private set; }

    [field: Header("Female Kaya Toast Order SFX")]
    [field: SerializeField] public EventReference F_KayaTostOrder { get; private set; }



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
