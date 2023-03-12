using UnityEngine;

public class AutoShooter : MonoBehaviour
{
    public GameObject SpawnPrefab;
    public float newSpawnDuration = 0.1f;
    public float forceMultiplier = 2;

    private Vector3 SpawnPos;
    private Vector3 SpawnScreenPos;
    
    private GameObject CurrentSpawn; 
    private bool SpawnReady = false;
    private GameObject SpawnParent;
    
    private void Start()
    {
        SpawnPos = transform.position;
        SpawnScreenPos = Camera.main.WorldToScreenPoint(SpawnPos);
        SpawnParent = new GameObject("SpawnParent");
        SpawnNewObject();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            AutoShoot();
        }
    }
    
    void AutoShoot()
    {
        if (SpawnReady)
        {
            Shoot();
        }
    }
    
    void SpawnNewObject()
    {
       CurrentSpawn = Instantiate(SpawnPrefab, SpawnPos, Quaternion.identity, SpawnParent.transform);
       SpawnReady = true;
    }

    void Shoot()
    {
        Vector3 Force =  Input.mousePosition - SpawnScreenPos;
        CurrentSpawn.GetComponent<Rigidbody>().AddForce(new Vector3(Force.x,Force.y,Force.y) * forceMultiplier);
            
        SpawnReady = false;
        
        Invoke("SpawnNewObject", newSpawnDuration);
    }
}
