using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotPooler : ObjectPooler
{

    /// <summary>
    /// Register spawned plates with a function to return them to pool whenever they dissapear
    /// </summary>
    /// <returns> A plate game object with a return to pool method registered for its "PlateGone" method </returns>
    protected override GameObject SpawnNewObject()
    {
        var obj = base.SpawnNewObject();

        var plateComponent = obj.GetComponent<Shot>();

        Debug.Assert(plateComponent != null, "PlatePooler not properly configured: This game object is not a plate, as it doesn't have a plate component");

        plateComponent.RegisterShotDisappeared(ReturnObject);

        return obj;
    }
}
