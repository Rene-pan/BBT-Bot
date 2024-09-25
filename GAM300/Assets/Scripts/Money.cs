using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

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
    private void Start()
    {
        SetGoalAmount();
        timer = FindAnyObjectByType<MainTimer>();

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
        if (currentEarnings >= TargetEarnings && timer.TimerIsRunning)
        {
            print("Game Win");
            UIs[1].SetActive(true);
            Time.timeScale = 0;
        }
        else if (currentEarnings < TargetEarnings && !timer.TimerIsRunning)
        {
            print("Game Lost");
            UIs[0].SetActive(true);
            Time.timeScale = 0;
        }
    }
}
