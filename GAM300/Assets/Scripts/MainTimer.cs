using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainTimer : MonoBehaviour
{
    public float mainDuration;
    float currentTime = 0;
    public TextMeshProUGUI timerText;
    public bool TimerIsRunning = false;
    public Slider TimerSlider;

    private void Start()
    {
        TimerIsRunning = true;
        currentTime = mainDuration;
        TimerSlider.maxValue = mainDuration;
        TimerSlider.value = mainDuration;
    }
    private void Update()
    {
        CountDown();
    }
    void CountDown()
    {
        if (TimerIsRunning == false) return;
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            DisplayTime(currentTime);
        }
        else
        {
            print("time gone");
            currentTime = 0;
            TimerIsRunning = false;
        }
    }
    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        //slider display
        TimerSlider.value = timeToDisplay;
    }

}
