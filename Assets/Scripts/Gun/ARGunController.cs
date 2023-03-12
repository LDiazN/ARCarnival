using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

/// <summary>
/// This class is a gun controller, it will manage operations
/// over a given gun object, handling AR information, such as 
/// if the target it's still visible or not
/// </summary>
[RequireComponent(typeof(ImageTargetBehaviour))]
public class ARGunController : DefaultTrackableEventHandler
{

    /// <summary>
    /// Gun object to use to shoot 
    /// </summary>
    [SerializeField]
    private GameObject _gun;

    /// <summary>
    /// Image target behavior component for this game object
    /// </summary>
    private ImageTargetBehaviour _imgTarget;

    /// <summary>
    /// Maximum time to wait before starting a reload
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _maxTimeToStartReload = 2.0f;
    public float MaxTimeToStartReload { get => _maxTimeToStartReload; }

    /// <summary>
    /// If you can reload when the target is found
    /// </summary>
    private bool _canStartReload = false;


    // Start is called before the first frame update
    protected override void Start()
    {
        // Call start method of default trackable event handler
        base.Start();

        // Sanity checking 
        Debug.Assert(_gun != null, "Error in ARGunController: Gun object not already set to an object");
        Debug.Assert(_gun.GetComponent<Gun>() != null, "Error in ARGunController: Gun object should have a Gun component");


        // Set up image target component
        _imgTarget = GetComponent<ImageTargetBehaviour>();
    }


    protected override void OnTrackingFound()
    {
        base.OnTrackingFound();

        var gunComp = _gun.GetComponent<Gun>();

        // Try to start shooting when the gun is in scope once again
        gunComp.StartShooting();

        if (_canStartReload)
            gunComp.Reload();

    }

    protected override void OnTrackingLost()
    {
        base.OnTrackingLost();

        // Start interval to reload 
        StartCoroutine(SetUpReloadableTimeInterval());
        // Stop shooting
        _gun.GetComponent<Gun>().StopShooting();

    }

    /// <summary>
    /// This coroutine controls the time where it's possible to reload. When it starts,
    /// sets can reload to true, then it waits for a few seconds, and then reloads
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetUpReloadableTimeInterval()
    {
        _canStartReload = true;
        yield return new WaitForSeconds(_maxTimeToStartReload);
        _canStartReload = false;
    }
}
