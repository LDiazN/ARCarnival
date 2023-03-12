using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System;

/// <summary>
/// Component to manage plate spawning. Plate will be spawned randomly in a rectangular 
/// region centered on the spawner object position, and launching them with a random force
/// </summary>
[RequireComponent(typeof(PlatePooler))]
public class PlateSpawner : MonoBehaviour
{

    /// <summary>
    /// Event triggered whenever a new plate is spawned
    /// </summary>
    private event Action<GameObject> PlateSpawned;

    /// <summary>
    /// Width of spawn region
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _spawnRegionWidth = 3;
    public float SpawnRegionWidth { get => _spawnRegionWidth; }

    /// <summary>
    /// Height of spawn region
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _spawnRegionHeight = 3;
    public float SpawnRegionHeight { get => _spawnRegionHeight; }

    /// <summary>
    /// Minimum launch force to launch a plate
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _minLaunchForce = 1;
    public float MinLaunchForce { get => _minLaunchForce; set => _minLaunchForce = value; }

    /// <summary>
    /// Minimum launch force to launch a plate
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _maxLaunchForce = 5;
    public float MaxLaunchForce { get => _maxLaunchForce; set => _maxLaunchForce = value; }

    /// <summary>
    /// Max distance from spawner to plate before destroying plate 
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _maxDistanceToSpawner = 1;
    public float MaxDistanceToSpawner { get => _maxDistanceToSpawner; set => _maxDistanceToSpawner = value; }

    /// <summary>
    /// Minimum time to wait between plate spawnings
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _minTimeBetweenSpawns = 1.0f;
    public float MinTimeBetweenSpawns { get => _minTimeBetweenSpawns; set => _minTimeBetweenSpawns = value; }

    /// <summary>
    /// Minimum time to wait between plate spawnings
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _maxTimeBetweenSpawns = 3.0f;
    public float MaxTimeBetweenSpawns { get => _maxTimeBetweenSpawns; set => _maxTimeBetweenSpawns = value; }

    /// <summary>
    /// Al plates will be launched pointing to a point up the spawner,
    /// so they describe a parabolic trajectory 
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _heightToPoint = 10.0f;
    public float HeightToPoint { get => _heightToPoint; set => _heightToPoint = value; }

    /// <summary>
    /// Time to wait between checks to see if a plate is too far from the spawner, 
    /// so that it should be deleted
    /// </summary>
    private float _timeToWaitBetweenDistanceChecks = 0.2f;

    /// <summary>
    /// If should start spawning
    /// </summary>
    [SerializeField]
    private bool _startSpawning = false;

    /// <summary>
    /// Pooler object to manage plate instantiation 
    /// </summary>
    private PlatePooler _platePooler;

    /// <summary>
    /// Tells if the spawner is currently spawning 
    /// </summary>
    private bool _isSpawning = false;
    public bool IsSpawning { get => _isSpawning; }

    /// <summary>
    /// A plane, formed by its 3 basis, you can use them to 
    /// compute a point or vector inside the plane 
    /// </summary>
    private struct Plane
    {
        public Vector3 normal;
        public Vector3 tangent;
        public Vector3 binormal;

        public Vector3 Z { get => normal; }
        public Vector3 X { get => binormal; }
        public Vector3 Y { get => -tangent; }
    }

    /// <summary>
    /// corners of a plane
    /// </summary>
    private struct Corners
    {
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
    }


    // Start is called before the first frame update
    void Start()
    {
        // Init object pooler
        _platePooler = GetComponent<PlatePooler>();
        Invoke("StartSpawning",3f);
        // StartSpawning(); // DEBUG ONLY, SHOULD USE SIGNAL TO TRIGGER SPAWNING
    }

    private void OnDrawGizmos()
    {
        // Draw spawning area 
        DrawSpawnArea();
    }

    /// <summary>
    /// Start a spawning coroutine. This is idempotent
    /// </summary>
    public void StartSpawning() 
    {
        if (!_isSpawning)
        {
            _isSpawning = true;
            StartCoroutine(Spawning());
        }
    }
    
    /// <summary>
    /// Stop this spawner from spawning.
    /// </summary>
    public void StopSpawning()
    {
        if (!_isSpawning) return;

        _isSpawning = false;
        StopCoroutine("Spawning");
    }

    /// <summary>
    /// Coroutine to continously spawn a new plate every few seconds
    /// </summary>
    private IEnumerator Spawning()
    {
        while (true)
        {
            SpawnPlate();
            yield return new WaitForSeconds(UnityEngine.Random.Range(_minTimeBetweenSpawns, _maxTimeBetweenSpawns));
        }
    }

    /// <summary>
    /// Spawn a single plate at a random position within 
    /// the spawning area boundaries
    /// </summary>
    void SpawnPlate()
    {
        // position to spawn from 
        var pos = GetNextSpawnPosition();

        // plate to spawn 
        var plate = InitPlate(_platePooler.GetObject(), pos);

        LaunchPlate(plate, pos);

        // Start a coroutine to check if plate should be destroyed
        StartCoroutine(CheckPlateDistance(plate));

        // Report plate spawned
        PlateSpawned?.Invoke(plate);

    }

    /// <summary>
    /// Initialize a plate as you need it to be before using it
    /// </summary>
    /// <param name="plate">Plate to initialize</param>
    /// <returns>Same plate, but initialized</returns>
    private GameObject InitPlate(GameObject plate, Vector3 position)
    {
        
        // Update position
        plate.transform.position = position;
        var plateRbd = plate.GetComponent<Rigidbody>();

        // Stop object basically
        plateRbd.velocity = Vector3.zero;
        plateRbd.angularVelocity = Vector3.zero;

        return plate;
    }

    /// <summary>
    /// Launch plate into the air
    /// </summary>
    /// <param name="plate"> Plate to launch </param>
    private void LaunchPlate(GameObject plate, Vector3 startPos)
    {
        // Set direction
        var direction = GetSpawnPlane().normal;

        // Get rigid body and check that it is not null
        var rbdy = plate.GetComponent<Rigidbody>();
        Debug.Assert(rbdy != null, "Plate to launch should have a rigidbody object so it can behave physically accurate");

        // Launch plate into the air
        var force = direction * UnityEngine.Random.Range(_minLaunchForce, _maxLaunchForce);
        rbdy.AddForce(force);
        var torque = new Vector3(
               UnityEngine.Random.Range(_minLaunchForce, _maxLaunchForce),
               UnityEngine.Random.Range(_minLaunchForce, _maxLaunchForce),
               UnityEngine.Random.Range(_minLaunchForce, _maxLaunchForce)
               );
        rbdy.AddTorque(torque);
    }

    /// <summary>
    /// Get next position randomly inside our rect extetents
    /// </summary>
    /// <returns> Random position within spawn area </returns>
    private Vector3 GetNextSpawnPosition()
    {
        var p = GetSpawnPlane();

        // return random position inside spawning area

        var localPos = p.X * UnityEngine.Random.Range(-0.5f * _spawnRegionWidth, 0.5f * _spawnRegionWidth) +
                       p.Y * UnityEngine.Random.Range(-0.5f * _spawnRegionHeight, 0.5f * _spawnRegionHeight);

        return localPos + transform.position;
    }

    /// <summary>
    /// Coroutine to check plate distance to spawner. If too far, then "destroy" plate
    /// </summary>
    /// <param name="plate"> Plate to destroy </param>
    private IEnumerator CheckPlateDistance( GameObject plate )
    {
        while (true)
        {
            // Debug.Log($"Plate {plate} is at position {plate.transform.position}");
            var toPlate = plate.transform.position - transform.position;
            if (toPlate.sqrMagnitude > _maxDistanceToSpawner * _maxDistanceToSpawner) // compare to sqrMagnitude so you don't compute sqrroot
            {
                if (plate.activeSelf)
                    _platePooler.ReturnObject(plate);
                break;
            }

            yield return new WaitForSeconds(_timeToWaitBetweenDistanceChecks);
        }
    }

    /// <summary>
    /// Get a plane defined by its normal and its basis vectors, tangent and binormal
    /// </summary>
    /// <returns> Plane object </returns>
    private Plane GetSpawnPlane()
    {
        Plane p;

        p.normal = transform.localToWorldMatrix * new Vector4(0,1,0,0);     // Z direction 
        p.tangent = transform.localToWorldMatrix * new Vector4(0,0,1,0);    // inverted Y axis
        p.binormal = Vector3.Cross(p.normal, p.tangent);                    // X axis


        return p;
    }

    /// <summary>
    /// Get corners for this spawn area, useful for debug
    /// </summary>
    /// <returns> Corners object defining this spawn area </returns>
    private Corners GetPlaneCorners()
    {
        var p = GetSpawnPlane();

        Corners c;

        c.topLeft     = - 0.5f * _spawnRegionWidth * p.binormal - 0.5f * _spawnRegionHeight * p.tangent;
        c.topRight    =   0.5f * _spawnRegionWidth * p.binormal - 0.5f * _spawnRegionHeight * p.tangent;
        c.bottomLeft  = - 0.5f * _spawnRegionWidth * p.binormal + 0.5f * _spawnRegionHeight * p.tangent;
        c.bottomRight =   0.5f * _spawnRegionWidth * p.binormal + 0.5f * _spawnRegionHeight * p.tangent;

        c.topLeft     += transform.position;
        c.topRight    += transform.position;
        c.bottomLeft  += transform.position;
        c.bottomRight += transform.position;

        return c;
    }

    /// <summary>
    /// Draw spawn area
    /// </summary>
    private void DrawSpawnArea()
    {
        var corners = GetPlaneCorners();

        Gizmos.color = Color.red;

        // Draw square area
        Gizmos.DrawLine(corners.topLeft, corners.topRight);
        Gizmos.DrawLine(corners.topRight, corners.bottomRight);
        Gizmos.DrawLine(corners.bottomRight, corners.bottomLeft);
        Gizmos.DrawLine(corners.bottomLeft, corners.topLeft);

        // Draw up vector
        var p5 = transform.position;
        var p6 = p5 + _heightToPoint * GetSpawnPlane().normal;

        Gizmos.DrawLine(p5, p6);
    }

    /// <summary>
    /// Register a new handler function to call whenever a plate is spawned. Plate itself is 
    /// passed to the function
    /// </summary>
    /// <param name="handler"> Handler function to register </param>
    public void RegisterPlateSpawnedHandler(Action<GameObject> handler)
    {
        PlateSpawned += handler;
    }

    /// <summary>
    /// Unregister existent handler function related to this event
    /// </summary>
    /// <param name="handler"> handler function to remove </param>
    public void UnregisterPlateSpawnedHandler(Action<GameObject> handler)
    {
        PlateSpawned -= handler;
    }
}
