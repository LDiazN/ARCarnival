using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This simple script will ensure that spawned plates in the 
/// main menu will dissapear after a few seconds after they're spawned
/// so we don't clutter the main menu with a lot of plates
/// </summary>
[RequireComponent(typeof(PlateSpawner))]
public class MainMenuPlateSpawner : MonoBehaviour
{
    [Tooltip("Time in seconds to wait before a spawned plate can be burned")]
    [SerializeField]
    private float _timeToBurnAPlate = 30;

    // 
    private PlateSpawner _spawner;
    // Start is called before the first frame update
    void Start()
    {
        // Sanity checks
        _spawner = GetComponent<PlateSpawner>();
        Debug.Assert(_spawner != null, "This game object should be a PlateSpawner with a PlateSpawner component attached");

        // Wire things up
        _spawner.RegisterPlateSpawnedHandler(StartBurningCoroutine);
    }

    /// <summary>
    /// Just a wrapper function to start a burning coroutine when a new plate is spawned
    /// </summary>
    /// <param name="plate"> Spawned plate to burn </param>
    private void StartBurningCoroutine(GameObject plate)
    {
        StartCoroutine(BurnPlateAfterSeconds(plate, _timeToBurnAPlate));
    }

    /// <summary>
    /// Coroutine to burn a plate after the specified amount of seconds 
    /// </summary>
    /// <param name="plate">Plate to burn</param>
    /// <param name="SecondsToWait">Seconds to wait to burn the plate</param>
    private IEnumerator BurnPlateAfterSeconds(GameObject plate, float SecondsToWait)
    {
        // Wait
        yield return new WaitForSeconds(SecondsToWait);

        // And then burn
        Plate plateComponent = plate.GetComponent<Plate>();
        Debug.Assert(plateComponent != null, "Game object should be a plate"); // Sanity check never hurts
        plateComponent.BurnPlate();
    }
}
