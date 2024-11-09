using NUnit.Framework.Internal.Builders;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
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
    public GameObject CreditsScreen;
    public Ambience MusicScript;
    public GameObject HowToPlayScreen;

    [Header("Press Esc to Pause")]
    public int PressEcount = 0;
    public bool CanPressESC = false;

    private void Start()
    {
        CanPressESC = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && CanPressESC)
        {
            var Money = FindAnyObjectByType<Money>();
            Money.UnlockCursor();
            switch (PressEcount)
            {
                case 0:
                    //pressed first time from gameplay
                    //play pause SFX
                    AudioManager.instance.PlayOneShot2D(FmodEvents.instance.PauseSFX);
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
                    //play pause SFX
                    AudioManager.instance.PlayOneShot2D(FmodEvents.instance.UNPauseSFX);
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
            Time.timeScale = 0;
            PressOnce = true;
            var Money = FindAnyObjectByType<Money>();
            Money.StopSuccessMusic();
            Money.StopFailureMusic();
            AudioManager.instance.PauseSounds(false);
            AudioManager.instance.StopAllSounds();
            //MusicScript.ResetMusic();
            //Destroy(AudioManager.instance.gameObject);
        }
    }
    public void Replay()
    {
        if (!PressReplay)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 0;
            PressReplay = true;
            var Money = FindAnyObjectByType<Money>();
            Money.StopFailureMusic();
            AudioManager.instance.PauseSounds(false);
            AudioManager.instance.StopAllSounds();
            HowToPlayScreen.SetActive(false);
            //Invoke("OffSounds", 0.02f);
        }
    }

    public void MMPress(string sceneName)
    {
        if (!MainMenuPress)
        {
            SceneManager.LoadScene(sceneName);
            var BGM = FindAnyObjectByType<BGM>();
            BGM.StopMusic();
            MainMenuPress = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
        AudioManager.instance.PlayOneShot2D(FmodEvents.instance.UNPauseSFX);
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
        //print("exit");
    }
    public void PlayUISFX()
    {
        AudioManager.instance.PlayOneShot2D(FmodEvents.instance.UI_Interact);
    }
    
    void OffSounds()
    {
        AudioManager.instance.StopAllSounds();
    }

    public void OpenCredits()
    {
        StartScreen.SetActive(false);
        CreditsScreen.SetActive(true);
    }
    public void CloseCredits()
    {
        StartScreen.SetActive(true);
        CreditsScreen.SetActive(false);
    }
    public void CloseHowToPlayButton()
    {
        CanPressESC = true;
        HowToPlayScreen.SetActive(false);
        PauseScreen.SetActive(true);
    }

    public void OpenHowToPlay()
    {
        //must close how to play then can unpause
        CanPressESC = false;
        HowToPlayScreen.SetActive(true);
        PauseScreen.SetActive(false);

    }
    public void CloseHowToPlayNStartButton()
    {
        CanPressESC = true;
        HowToPlayScreen.SetActive(false);
        Time.timeScale = 1.0f;
        var Money = FindAnyObjectByType<Money>();
        Money.lockCursor();
    }

}
