using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{

    [Tooltip("Canvas element asking the user to focus on the table to start the game")]
    [SerializeField]
    private GameObject _focusTableMessage;

    /// <summary>
    /// Set tracking indicator to red or green depending on if you have tracking of the table
    /// </summary>
    [Tooltip("Canvas image to tell the user the tracking state of the table")]
    [SerializeField]
    private Image _trackingIndicator;

    [Tooltip("Color to set the indicator when tracking is lost")]
    [SerializeField]
    private Color _nonTrackingColor = Color.red;

    [Tooltip("Color to set the indicator when tracking is found")]
    [SerializeField]
    private Color _trackingColor = Color.green;

    [Tooltip("Level menu object in canvas")]
    [SerializeField]
    private GameObject _mainMenu;

    [Tooltip("Main menu Scene name to change once an exit-level is requested")]
    [SerializeField]
    private string _mainMenuSceneName = "MainMenu";

    // Start is called before the first frame update
    void Start()
    {
        SanityChecks();
        HideLevelMenu();
    }

    /// <summary>
    /// Hide focus-on-table message
    /// </summary>
    public void HideFocusTableMessage()
    {
        _focusTableMessage.SetActive(false);
    }

    public void SetTrackingIndicatorOff()
    {
        _trackingIndicator.color = _nonTrackingColor;
    }

    public void SetTrackingIndicatorOn()
    {
        _trackingIndicator.color = _trackingColor;
    }

    public void ShowLevelMenu()
    {
        _mainMenu.SetActive(true);
    }

    public void HideLevelMenu()
    {
        _mainMenu.SetActive(false);
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    /// <summary>
    /// Check starting state consistency
    /// </summary>
    private void SanityChecks()
    {
        Debug.Assert(_focusTableMessage != null, "Message asking the user to focus on the table not provided");
        Debug.Assert(_trackingIndicator != null, "Missing tracking indicator");
    }
}
