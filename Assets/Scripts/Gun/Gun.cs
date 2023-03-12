using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Draw a line at the top of the current object, compute direction of sight 
/// based on object normal (y direction).
/// The three main events you may be interested are:
///     * ShotTriggered: Called whenever a new shot is made 
///     * OutOfAmmo: Called when this gun runs out of ammo
///     * Reloaded: Called after reloading
/// </summary>
[RequireComponent(typeof(LineRenderer), typeof(ObjectPooler))]
public class Gun : MonoBehaviour
{
    /// <summary>
    /// Handler function to do something when a shot is triggered
    /// </summary>
    /// <param name="shot"> Shot object triggered </param>
    public delegate void ShotTriggeredHandler(GameObject shot);

    /// <summary>
    /// Event triggered when a new shot is triggered
    /// </summary>
    private event ShotTriggeredHandler ShotTriggered;

    /// <summary>
    /// Event triggered when swaping to out of ammo state
    /// </summary>
    private event Action OutOfAmmo;

    /// <summary>
    /// Event called when a reaload is finished
    /// </summary>
    private event Action Reloaded;

    /// <summary>
    /// Initial position in local coordinates of every shot 
    /// </summary>
    [SerializeField]
    private Vector3 _shotStartPosition = Vector3.zero;

    /// <summary>
    /// Child game object that is the actual 3D model for this gun
    /// </summary>
    [SerializeField]
    private GameObject _model;

    /// <summary>
    /// If gun should start shooting;
    /// </summary>
    [SerializeField]
    private bool _startShooting = false;

    /// <summary>
    /// How long is the sight ray
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _sightLength = 10;
    public float SightLength { get => _sightLength; set => _sightLength = value; }

    /// <summary>
    /// How much time to wait between shots 
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _shotsTimeToLive = 0.1f;

    /// <summary>
    /// Time in seconds between shots
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _timeBetweenShots = 1.0f;
    public float TimeBetweenShots { get => _timeBetweenShots; set => _timeBetweenShots = value; }

    /// <summary>
    /// Time in seconds for this gun to reload
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _reloadTime = 2.0f;
    public float ReloadTime { get => _reloadTime; set => _reloadTime = value; }

    /// <summary>
    /// Amount of bullets to shot before reloading 
    /// </summary>
    [SerializeField]
    [Min(0)]
    private int _maxAmmo = 100;
    public int MaxAmmo { get => _maxAmmo; set => _maxAmmo = value; }

    [Min(0)]
    private int _currentAmmo = 0;
    public int CurrentAmmo { get => _currentAmmo; }

    /// <summary>
    /// where's the gun pointing at
    /// </summary>
    private Vector3 _sightDirection;
    public Vector3 SightDirection { get => _sightDirection; }

    /// <summary>
    /// Line renderer to draw line at the sight of this object
    /// </summary>
    private LineRenderer _lineRenderer;

    /// <summary>
    /// Pool of shot objects to shoot
    /// </summary>
    private ObjectPooler _shotPool;

    /// <summary>
    /// If gun is currently shooting
    /// </summary>
    private bool _shooting = false;
    public bool Shooting { get => _shooting; }

    /// <summary>
    /// Manager object for shooting routine
    /// </summary>
    private IEnumerator _shootRoutine = null;

    /// <summary>
    /// If gun is currently reloading
    /// </summary>
    private bool _reloading = false;
    public bool Reloading { get => _reloading; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(_model != null, "ERROR IN GUN GAME OBJECT: Missing model object, forgot to configure it? Changed the gun model itself?");
        // Init line render
        _lineRenderer = GetComponent<LineRenderer>();
        Debug.Assert(_lineRenderer != null, "Missing line renderer component in gun component");

        // Init shot object manager
        _shotPool = GetComponent<ObjectPooler>();
        Debug.Assert(_shotPool != null, "Missing line renderer component in gun component");

        _currentAmmo = _maxAmmo;

        if (_startShooting) StartShooting();
        // Add counter
        Counter.ScoreValue = _currentAmmo;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateSight();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GetShortStartPos(), 0.1f);
    }

    /// <summary>
    /// Update locally stored direction of sight
    /// </summary>
    private void UpdateSightDirection()
    {
        _sightDirection = transform.localToWorldMatrix * new Vector4(0,1,0,0);
    }

    /// <summary>
    /// Update Ray direction 
    /// </summary>
    private void UpdateSight()
    {
        UpdateSightDirection();

        var shotStartPos = GetShortStartPos();
        _lineRenderer.SetPositions(new Vector3[]{
            shotStartPos,
            shotStartPos + _sightLength * _sightDirection
        });
    }

    /// <summary>
    /// Action to shoot every few seconds (time specified by _timeBetweenShots variable).
    /// Everytime a shoot is triggered, an event will trigger notifying that it was triggered
    /// </summary>
    private IEnumerator Shoot()
    {
        _shooting = true;

        while (true)
        {
            // Check for ammo before shooting
            if (_currentAmmo <= 0)
            {
                NoAmmoShoot();
            }
            else
            {
                Debug.Log("Im shooting");
                Debug.Assert(_currentAmmo > 0, "Ammunition is not checked since shooting is controlled by state machine, not by a single function");

                Counter.Ammo -= 1;

                // Get a new object and configure it 
                var shot = _shotPool.GetObject();
                shot.transform.position = GetShortStartPos();

                var shotComp = shot.GetComponent<Shot>();

                // Sanity check
                Debug.Assert(shotComp != null, "Provided shot object doesn't have a Shot component, it can't be used as a bullet");

                // update projectile direction
                shotComp.ShotAt(_sightDirection);

                // Destroy shot when it's not relevant anymore
                StartCoroutine(CheckShotLifeTime(shot));
                
                // call shot event
                ShotTriggered?.Invoke(shot);

                _currentAmmo -= 1;
            }

            yield return new WaitForSeconds(TimeBetweenShots);
        }
    }

    /// <summary>
    /// Action to do when no ammo
    /// </summary>
    private void NoAmmoShoot()
    {
        Debug.Log("No ammo :(");
    }

    /// <summary>
    /// Coroutine to reload gun, reseting its max ammo and starting shooting is necesary
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadCoroutine()
    {
        // Start reloading
        _reloading = true;

        // Stop coroutines if necessary
        StopShooting();

        // reload 
        Debug.Log("Gun reloading");
        _currentAmmo = _maxAmmo;

        yield return new WaitForSeconds(_reloadTime);

        // Call reload event
        Reloaded?.Invoke();

        // End reloading
        _reloading = false;

        // Start shooting again
        StartShooting();
    }

    /// <summary>
    /// Function to tell the gun to start a reload
    /// </summary>
    public void Reload()
    {
        Debug.Log("Gonna reload");
        StartCoroutine(ReloadCoroutine());
    }

    /// <summary>
    /// Use this function to stop shooting
    /// </summary>
    public void StopShooting()
    {
        if (_shootRoutine == null) return;
        StopCoroutine(_shootRoutine);
        _shootRoutine = null;
    }

    /// <summary>
    /// Use this function to start shooting. It is idempotent, if already shooting, no new 
    /// shooting will be started
    /// </summary>
    public void StartShooting()
    {
        if (_shootRoutine != null || _reloading) return;

        StartCoroutine(_shootRoutine = Shoot());
    }

    /// <summary>
    /// Coroutine to destroy a shot after a few seconds (specified by shotTimeToLive variable)
    /// </summary>
    /// <param name="shot"> Shot to destroy </param>
    /// <returns></returns>
    private IEnumerator CheckShotLifeTime(GameObject shot)
    {
        yield return new WaitForSeconds(_shotsTimeToLive);
        
        if (shot.activeSelf)
            _shotPool.ReturnObject(shot);
    }

    private Vector3 GetShortStartPos()
    {
        Vector3 globalOffsetVector = transform.localToWorldMatrix * (_shotStartPosition);
        return _model.transform.position + globalOffsetVector;
    }

    /// <summary>
    /// Register a handler for a shot triggered even, it's called whenever a new shoot is triggered
    /// </summary>
    /// <param name="handler"> Handler function to execute when event it's called </param>
    public void RegisterShotTriggeredHandler(ShotTriggeredHandler handler)
    {
        ShotTriggered += handler;
    }

    /// <summary>
    /// Unregister an existent handler function for ShotTriggered event 
    /// </summary>
    /// <param name="handler"> Handler function to remove </param>
    public void UnregisterShotTriggeredHandler(ShotTriggeredHandler handler)
    {
        ShotTriggered -= handler;
    }

    /// <summary>
    /// Register handler function to be called whenever the gun runs out of ammo
    /// </summary>
    /// <param name="handler"> Handler function to register </param>
    public void RegisterOutOfAmmoHandler(Action handler)
    {
        OutOfAmmo += handler;
    }

    /// <summary>
    /// UnRegister an existent handler function for out of ammo event
    /// </summary>
    /// <param name="handler"> Handler function to remove </param>
    public void UnregisterOutOfAmmoHandler(Action handler)
    {
        OutOfAmmo -= handler;
    }

    /// <summary>
    /// Register handler fucntion to be called whenever a reload is just finished
    /// </summary>
    /// <param name="Handler"> Handler function to register </param>
    public void RegisterReloadHandler(Action Handler)
    {
        Reloaded += Handler;
    }

    /// <summary>
    /// Remove and existent handler fucntion to be called whenever a reload is just finished
    /// </summary>
    /// <param name="Handler"> Handler function to register </param>
    public void UnregisterReloadHandler(Action Handler)
    {
        Reloaded -= Handler;
    }
}
