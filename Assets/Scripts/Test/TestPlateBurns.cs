using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlateBurns : MonoBehaviour
{
    public GameObject plateModel;
    private bool done = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!done)
        {
            var idk = GetComponent<Plate>();
            idk.BurnPlate();
            done = true;
        }
    }
}
