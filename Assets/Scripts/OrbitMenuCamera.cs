using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitMenuCamera : MonoBehaviour
{
    // Camera used to rotate around main target
    private Camera _camera;

    // Object in scene to rotate around
    [Tooltip("Object to rotate around")]
    [SerializeField]
    private GameObject _objectToOrbit;

    [Tooltip("Speed to rotate around the object")]
    [SerializeField]
    private float _orbitSpeed = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        // Sanity checks
        Debug.Assert(_objectToOrbit != null, "Object to orbit not specified in Orbit Camera");

        _camera = GetComponent<Camera>();
        Debug.Assert(_camera != null, "Missing camera component in orbital camera Game Object");
    }

    // Update is called once per frame
    void Update()
    {
        _camera.transform.LookAt(_objectToOrbit.transform);
        _camera.transform.RotateAround(_objectToOrbit.transform.position, Vector3.up, Time.deltaTime * _orbitSpeed);
    }
}
