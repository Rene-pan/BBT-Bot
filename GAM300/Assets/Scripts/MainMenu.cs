using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public bool PressOnce = false;
    public bool PressReplay = false;
    public bool MainMenuPress = false;
    public Transform PlayUIPosition;
    public GameObject LevelSelect;
    public GameObject StartScreen;
    public GameObject PauseScreen;

    [Header("Press Esc to Pause")]
    public int PressEcount = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var Money = FindAnyObjectByType<Money>();
            Money.UnlockCursor();
            switch (PressEcount)
            {
                case 0:
                    //pressed first time from gameplay
                    //open pause menu
                    PauseScreen.SetActive(true);
                    //stop time
                    Time.timeScale = 0;
                    //pause all sounds
                    AudioManager.instance.PauseSounds(true);
                    //indicate that player has been pressed
                    PressEcount = 1;
                    break;
                case 1:
                    //pressed second time from pause menu
                    //close pause menu
                    PauseScreen.SetActive(false);
                    //unstop time
                    Time.timeScale = 1;
                    //unpause all sounds
                    AudioManager.instance.PauseSounds(false);
                    //indicate that player has not been pressed
                    PressEcount = 0;
                    Money.lockCursor();
                    break;
            }
        }
    }
    public IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonPress(string sceneName)
    {
        if (!PressOnce)
        {
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1;
            PressOnce = true;
            var Money = FindAnyObjectByType<Money>();
            Money.StopSuccessMusic();
            Money.StopFailureMusic();
            //AudioManager.instance.StopAllSounds();
            //Destroy(AudioManager.instance.gameObject);
        }
    }
    public void Replay()
    {
        if (!PressReplay)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 1;
            PressReplay = true;
            var Money = FindAnyObjectByType<Money>();
            Money.StopFailureMusic();
            //AudioManager.instance.StopAllSounds();
            //Invoke("OffSounds", 0.02f);
        }
    }

    public void MMPress(string sceneName)
    {
        if (!MainMenuPress)
        {
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1;
            MainMenuPress = true;
            var BGM = FindAnyObjectByType<BGM>();
            BGM.StopMusic();
        }
    }
    public void OpenLevelSelect()
    {
        LevelSelect.SetActive(true);
        StartScreen.SetActive(false);
    }
    public void CloseLevelSelect()
    {
        LevelSelect.SetActive(false);
        StartScreen.SetActive(true);
    }

    public void UnPausePress()
    {
        AudioManager.instance.PauseSounds(false);
        PressEcount = 0;
        Time.timeScale = 1;
        PauseScreen.SetActive(false);
        var Money = FindAnyObjectByType<Money>();
        Money.lockCursor();
    }
    public void Exit()
    {
        //PlayUISFX();
        Application.Quit();
        print("exit");
    }
    public void PlayUISFX()
    {
        AudioManager.instance.PlayOneShot2D(FmodEvents.instance.UI_Interact);
    }
    
    void OffSounds()
    {
        AudioManager.instance.StopAllSounds();
    }
    
}
