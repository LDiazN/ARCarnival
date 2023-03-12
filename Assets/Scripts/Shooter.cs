using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class Shooter : MonoBehaviour
{
    public GameObject arCamera;
    // public GameObject smoke;

    // Update is called once per frame
    public void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(arCamera.transform.position, arCamera.transform.forward, out hit))
        {
            if (hit.transform.name == "Plate1(Clone)" || hit.transform.name == "Plate2(Clone)" || hit.transform.name == "Plate1(Clone)")
            {
                Destroy(hit.transform.gameObject);
                // Instantiate(smoke, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}
