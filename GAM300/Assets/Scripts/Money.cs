using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq.Expressions;
using FMOD.Studio;

public class Money : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Goaltext;
    [SerializeField] GameObject AddMoneyPrefab;
    [SerializeField] GameObject EarningHolder;
    [SerializeField] int TargetEarnings;
    public int currentEarnings = 0;
    [SerializeField] string animationName;
    [SerializeField] PlayerController playerController;
    [SerializeField] MainTimer timer;
    public List<GameObject> UIs; // 0 is success, 1 is failure
    public GameObject player;

    //audio & ui
    private bool GameOverPlayOnce;
    private bool GameSuccessPlayOnce;
    public bool StopOnce = true;
    public MainMenu mainMenu;
    private EventInstance success;
    private EventInstance failure;
    private void Start()
    {
        SetGoalAmount();
        timer = FindAnyObjectByType<MainTimer>();
        GameOverPlayOnce = true;
        GameSuccessPlayOnce = true;

    }
    private void Update()
    {
        Cheat();
    }
    public void SetGoalAmount()
    {
        currentEarnings = 0;
        Goaltext.text = "";
        Goaltext.text = "$" + (currentEarnings).ToString() + "/" + TargetEarnings.ToString(); ;
    }
    public void AddMoney(int amount)
    {
        var moneyPopup = Instantiate(AddMoneyPrefab, EarningHolder.transform);
        AudioManager.instance.PlayOneShot(FmodEvents.instance.EarnMoney, playerController.gameObject.transform.position);
        moneyPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+ $"+amount.ToString();
        moneyPopup.GetComponent<Animator>().Play(animationName);
        currentEarnings += amount;
        Goaltext.text = "$"+(currentEarnings).ToString() + "/" + TargetEarnings.ToString();
    }
    void Cheat()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentEarnings = TargetEarnings;
            GameSuccessPlayOnce = true;
        }
    }
    public void CheckMoney()
    {
        if (currentEarnings >= TargetEarnings && timer.TimerIsRunning)
        {
            if (StopOnce)
            {
                StopSounds();
                StopOnce = false;
                print("why");
            }
            UnlockCursor();

            mainMenu.PressOnce = false;
            if (GameSuccessPlayOnce)
            {
                AudioManager.instance.PlayOneShot(FmodEvents.instance.gameSuccess, Vector3.zero);
                //success = AudioManager.instance.CreateInstance(FmodEvents.instance.gameSuccess);
                GameSuccessPlayOnce = false;
                //success.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Vector3.zero));
                //success.start();
                //success.release();
            }
            print("Game Win");
            UIs[1].SetActive(true);
            Time.timeScale = 0;
        }
        else if (currentEarnings < TargetEarnings && !timer.TimerIsRunning)
        {
            if (StopOnce)
            {
                StopSounds();
                StopOnce = false;
                print("why");
            }
            UnlockCursor();
            StopSounds();
            mainMenu.PressOnce = false;
            mainMenu.PressReplay = false;
            if (GameOverPlayOnce)
            {
                AudioManager.instance.PlayOneShot(FmodEvents.instance.gameOver, Vector3.zero);
                //music.StopMusic();
                GameOverPlayOnce = false;
            }
            print("Game Lost");
            UIs[0].SetActive(true);
            Time.timeScale = 0;
        }
    }
    void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    void StopSounds()
    {
        AudioManager.instance.StopAllSounds();
        //success.stop(STOP_MODE.IMMEDIATE);

    }
    void PlayMusic()
    {
        success = AudioManager.instance.CreateInstance(FmodEvents.instance.gameSuccess);
        success.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(Vector3.zero));
        success.start();
        success.release();
    }
}
