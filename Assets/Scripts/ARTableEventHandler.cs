using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System;


/// <summary>
/// Simple wraper class to provide signals when tracking is found and lost
/// </summary>
[RequireComponent(typeof(ImageTargetBehaviour))]
public class ARTableEventHandler : DefaultTrackableEventHandler
{
    // Event called when tracking found
    private event Action OnTrackingFoundEvent;

    // Event called when tracking lost
    private event Action OnTrackingLostEvent;

    [Tooltip("Spawner used to keep spawning plates")]
    [SerializeField]
    private PlateSpawner _spawner;

    /// <summary>
    /// Register a callback function to be called when the table is found
    /// </summary>
    /// <param name="onTrackingFoundHandler">Function to be called</param>
    public void RegisterOnTrackingFound(Action onTrackingFoundHandler) => OnTrackingFoundEvent += onTrackingFoundHandler;
    
    /// <summary>
    /// Unregister an already registered callback for when the table is found
    /// </summary>
    /// <param name="onTrackingFoundHandler">Function to unregister</param>
    public void UnregisterOnTrackingFound(Action onTrackingFoundHandler) => OnTrackingFoundEvent -= onTrackingFoundHandler;

    public void RegisterOnTrackingLost(Action onTrackingLostHandler) => OnTrackingLostEvent += onTrackingLostHandler;

    public void UnregisterOnTrackingLost(Action onTrackingLostHandler) => OnTrackingLostEvent -= onTrackingLostHandler;

    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();
        _spawner.StartSpawning();
        OnTrackingFoundEvent?.Invoke();
    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();
        _spawner.StopSpawning();
        OnTrackingLostEvent?.Invoke();
    }
}
