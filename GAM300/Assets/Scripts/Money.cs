using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq.Expressions;

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
    private bool GameOverPlayOnce;
    private bool GameSuccessPlayOnce;
    private void Start()
    {
        SetGoalAmount();
        timer = FindAnyObjectByType<MainTimer>();
        GameOverPlayOnce = true;
        GameSuccessPlayOnce = true;

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
        moneyPopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "+ $"+amount.ToString();
        moneyPopup.GetComponent<Animator>().Play(animationName);
        currentEarnings += amount;
        Goaltext.text = "$"+(currentEarnings).ToString() + "/" + TargetEarnings.ToString();
    }
    public void CheckMoney()
    {
        UnlockCursor();
        if (currentEarnings >= TargetEarnings && timer.TimerIsRunning)
        {
            if (GameSuccessPlayOnce)
            {
                AudioManager.instance.PlayOneShot(FmodEvents.instance.gameSuccess, playerController.gameObject.transform.position);
                GameSuccessPlayOnce = false;
            }
            print("Game Win");
            UIs[1].SetActive(true);
            AudioManager.instance.enabled = false;
            Time.timeScale = 0;
        }
        else if (currentEarnings < TargetEarnings && !timer.TimerIsRunning)
        {
            if (GameOverPlayOnce)
            {
                AudioManager.instance.PlayOneShot(FmodEvents.instance.gameOver, playerController.gameObject.transform.position);
                GameOverPlayOnce = false;
            }
            print("Game Lost");
            UIs[0].SetActive(true);
            Time.timeScale = 0;
            AudioManager.instance.enabled = false;
        }
    }
    void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
