using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class controls how colors of the gun model will change 
/// when the ammunition is changing
/// </summary>
[RequireComponent(typeof(Gun))]
public class GunColorController : MonoBehaviour
{
    /// <summary>
    /// Gun component attached to this component
    /// </summary>
    private Gun _gun;

    /// <summary>
    /// Color for the gun win ammo is at max capacity
    /// </summary>
    [SerializeField]
    private Color _maxCapacityColor = Color.green;

    /// <summary>
    /// Color for the gun when at lowest capacity
    /// </summary>
    [SerializeField]
    private Color _zeroCapactityColor = Color.red;

    /// <summary>
    /// Meshes whos material color will be changed
    /// </summary>
    [SerializeField]
    private MeshRenderer[] _meshesToChangeMaterialColor;

    // Start is called before the first frame update
    void Start()
    {
        // Sanity check
        _gun = GetComponent<Gun>();
        Debug.Assert(_gun != null, "Error in GunColorController: Gun component should not be null");

        // Set up gun component @TODO hay que configurar el manejador de eventos 
        // de disparo para que cambie el color 
        _gun.RegisterShotTriggeredHandler(OnGunShot);

        // Set array of meshes to empty array if null to keep us from weird null reference errors
        if (_meshesToChangeMaterialColor == null)
            _meshesToChangeMaterialColor = new MeshRenderer[0];

        UpdateColor();
    }

    private void Update()
    {
        UpdateColor();
    }

    /// <summary>
    /// Called whenever a shot is triggered, it changes the color of given parts in the gun 
    /// </summary>
    /// <param name="shot"></param>
    private void OnGunShot(GameObject shot)
    {
        UpdateColor();

        Debug.Log($"Changing color, current ammo is {_gun.CurrentAmmo}");
    }

    /// <summary>
    /// Update color of meshes to their corresponding one according to the ammo level
    /// </summary>
    private void UpdateColor()
    {
        float maxAmmo = _gun.MaxAmmo;
        float currAmmo = _gun.CurrentAmmo;

        Color minColor, maxColor;

        minColor = _zeroCapactityColor;
        maxColor = _maxCapacityColor;

        ChangeColor(Color.Lerp(minColor, maxColor, currAmmo / maxAmmo));
    }

    private void ChangeColor(Color color)
    {
        foreach( var mesh in _meshesToChangeMaterialColor)
        {
            mesh.material.SetVector("_Color", color);
        }    
    }

}
