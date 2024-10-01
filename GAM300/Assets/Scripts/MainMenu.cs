using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    bool PressOnce = false;
    bool PressReplay = false;
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ButtonPress(string sceneName)
    {
        if (!PressOnce)
        {
            SceneManager.LoadScene(sceneName);
            Time.timeScale = 1;
            PressOnce = true;
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
        }
    }
}
