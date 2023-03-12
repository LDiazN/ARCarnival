using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlatesSpawnAndBurn : MonoBehaviour
{

    public PlateSpawner spawner;
    private LinkedList<GameObject> plates;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(spawner != null, "no le asignaste el spawner en el editor");
        plates = new LinkedList<GameObject>();

        spawner.RegisterPlateSpawnedHandler(AddPlateToList);
    }

    public void AddPlateToList(GameObject plate)
    {
        plates.AddLast(plate);
    }

    public void BurnPlate()
    {
        var plate = plates.First.Value;

        Debug.Log($"burning plate {plate}");

        plates.RemoveFirst();

        plate?.GetComponent<Plate>()?.BurnPlate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            BurnPlate();
    }
}
