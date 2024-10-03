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
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonPress(string sceneName)
    {
            PlayUISFX();
        if (!PressOnce)
        {
            //AudioManager.instance.StopAllSounds();
            Destroy(AudioManager.instance.gameObject);
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1;
            PressOnce = true;
        }
    }
    public void Replay()
    {
        PlayUISFX();
        if (!PressReplay)
        {
            //AudioManager.instance.StopAllSounds();
            PlayUISFX();
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 1;
            PressReplay = true;
        }
    }

    public void MMPress(string sceneName)
    {
        PlayUISFX();
        if (!MainMenuPress)
        {
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1;
            MainMenuPress = true;
        }
    }

    public void Exit()
    {
        PlayUISFX();
        Application.Quit();
        print("exit");
    }
    void PlayUISFX()
    {
        AudioManager.instance.PlayOneShot(FmodEvents.instance.UI_Interact, PlayUIPosition.position);
    }
    
}
