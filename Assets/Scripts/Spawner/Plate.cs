using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Plate : MonoBehaviour
{

    /// <summary>
    /// Time in seconds for this plate to burn and dissapear
    /// </summary>
    [Min(0)]
    [SerializeField]
    private float _timeToBurn = 1.0f;

    /// <summary>
    /// Model with the plate mesh and its corresponding materials
    /// </summary>
    [SerializeField]
    private GameObject _model;

    /// <summary>
    /// Event to be called when this plate is already gone 
    /// </summary>
    private event Action<GameObject> PlateGone;

    /// <summary>
    /// If currently burning
    /// </summary>
    private bool _burning = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_model != null, "Model property for this plate not properly configured");
        var renderer = _model.GetComponent<MeshRenderer>();
        Debug.Assert(renderer != null, "Given model should have a renderer with materials");


        InitPlate();
    }

    /// <summary>
    /// Register a handler function to be called whenever this plate is gone
    /// </summary>
    /// <param name="handler"> 
    /// Handler function to call when this plate is gone, the plate itself is given 
    /// as an argument
    /// </param>
    public void RegisterPlaneGoneEvent( Action<GameObject> handler )
    {
        PlateGone += PlateGone;
    }

    /// <summary>
    /// Remove a handler function to be called whenever this plate is gone
    /// </summary>
    /// <param name="handler"> 
    /// Handler function to remove from event triggering
    /// as an argument
    /// </param>
    public void UnregisterPlaneGoneEvent(Action<GameObject> handler)
    {
        PlateGone -= PlateGone;
    }

    /// <summary>
    /// Trigger plate gone event
    /// </summary>
    private void TriggerPlateGone()
    {
        PlateGone?.Invoke(gameObject);
    }

    private void OnEnable()
    {
        // Init plate whenever it is enabled out of the pool
        InitPlate();    
    }

    /// <summary>
    /// Set how much burned is this plate right now 
    /// </summary>
    /// <param name="amount"> amout of burning effect, only visible changes in the interval [-1,1] </param>
    private void SetBurnAmount(float amount)
    {
        var dissolveMaterial = _model?.GetComponent<MeshRenderer>()?.material;

        Debug.Assert(dissolveMaterial != null, "Cannot burn plate, as it doesn't have a properly configured dissolve material");

        dissolveMaterial.SetFloat("_Dissolve", amount);

        Debug.Log($"current dissolve: {dissolveMaterial.GetFloat("_Dissolve")}");
    }

    /// <summary>
    /// Function to call to restore the state of this object
    /// </summary>
    private void InitPlate()
    {
        _burning = false;
        SetBurnAmount(-1);
    }

    /// <summary>
    /// Burn this plate. If already running, ignore this function call
    /// </summary>
    public void BurnPlate()
    {
        Debug.Log("Burning");
        if (!_burning)
            StartCoroutine(Burn());
    }

    /// <summary>
    /// Coroutine to burn this plate periodically between frames, and then call the plate gone function
    /// </summary>
    /// <returns></returns>
    private IEnumerator Burn()
    {
        // Time since started to burn 
        var timeSpent = 0.0f;

        // set burning to true, so you can't trigger a burning twice
        _burning = true;

        var i = 0;
        while (timeSpent <= _timeToBurn)
        {
            // Interpolate burnning amount
            SetBurnAmount(Mathf.Lerp(-1, 1, timeSpent / _timeToBurn));
            timeSpent  += Time.deltaTime;

            yield return new WaitForEndOfFrame();
            i++;
        }

        Debug.Log($"i is {i}");
        // Stop burning and call PlateGone
        _burning = false;
        TriggerPlateGone();
    }
}
