using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Operations : MonoBehaviour
{
    void Start()
    {

    }

    /// <summary>
    /// Counts the collisions in a plate
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        Counter.Hits += 1;
    }
}
