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
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonPress(string sceneName)
    {
        if (!PressOnce)
        {
            AudioManager.instance.StopAllSounds();
            AudioManager.instance.PlayOneShot(FmodEvents.instance.UI_Interact, transform.position);
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1;
            PressOnce = true;
        }
    }
    public void Replay()
    {
        if (!PressReplay)
        {
            AudioManager.instance.StopAllSounds();
            AudioManager.instance.PlayOneShot(FmodEvents.instance.UI_Interact, transform.position);
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
            Time.timeScale = 1;
            PressReplay = true;
        }
    }

    public void MMPress(string sceneName)
    {
        if (!MainMenuPress)
        {
            AudioManager.instance.PlayOneShot(FmodEvents.instance.UI_Interact, transform.position);
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1;
            MainMenuPress = true;
        }
    }
}
