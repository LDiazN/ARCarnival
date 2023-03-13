using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

/// <summary>
/// Manage the infinite game mode
/// </summary>
public class InfiniteGameMode : MonoBehaviour
{

    [Tooltip("AR Table object with marker")]
    [SerializeField]
    private GameObject _table;

    [Tooltip("GameUI manager object in the scene")]
    [SerializeField]
    private GameUIManager _gameUIManager;
    
    [Tooltip("Gun currently in use in the game")]
    [SerializeField]
    private Gun _gun;

    // If the game already started
    private bool _gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        SanityChecks();

        _gameStarted = false;

        // Register event to be called when tracking is found
        var tableEventHandler = _table.GetComponent<ARTableEventHandler>();
        tableEventHandler?.RegisterOnTrackingFound(StartGameOnTableFound);
        tableEventHandler?.RegisterOnTrackingFound(_gameUIManager.SetTrackingIndicatorOn);
        tableEventHandler?.RegisterOnTrackingLost(_gameUIManager.SetTrackingIndicatorOff);
    }

    /// <summary>
    /// Check consistency of starting state
    /// </summary>
    private void SanityChecks()
    {
        Debug.Assert(_table != null, "AR Table object should be provided");
        Debug.Assert(_table.GetComponent<ARTableEventHandler>() == null, "Table object doesn't have a DefaultTrackableEventHandler. Are you sure it is a AR-capable object?");
        Debug.Assert(_gameUIManager != null, "UI Manager still not provided");
        Debug.Assert(_gun != null, "Scene gun not provided");
    }

    private void StartGameOnTableFound()
    {
        // Start game
        _gameUIManager.HideFocusTableMessage();
        var arEventHandler = _table.GetComponent<ARTableEventHandler>();

        // We now longer care if the table stops 
        arEventHandler.UnregisterOnTrackingFound(StartGameOnTableFound);

        _gameStarted = true;
    }
}
