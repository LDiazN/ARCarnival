using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple class used to manage actions by the buttons in the main menu
/// </summary>

public class MainMenuGame : MonoBehaviour
{
    [Tooltip("Scene asset of infinite mode")]
    [SerializeField]
    private string _infiniteModeSceneName = "InfiniteMode";

    public void PlayInfiniteMode()
    {
        SceneManager.LoadScene(_infiniteModeSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StopGame()
    {
        SceneManager.LoadScene(2);
    }
}
