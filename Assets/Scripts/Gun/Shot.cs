using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

/// <summary>
/// Behavior of a shot, will just move to the specified direction endlessly
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Shot : MonoBehaviour
{
    /// <summary>
    /// Speed for this shot to travel
    /// </summary>
    [SerializeField]
    [Min(0)]
    private float _speed = 10.0f;

    /// <summary>
    /// Time in seconds for this object to totally dissapear
    /// </summary>
    [Min(0)]
    [SerializeField]
    private float _timeToDissapear = 1.0f;

    public delegate void OnShotHit(GameObject shot, Collision col);

    private event OnShotHit ShotHit;

    /// <summary>
    /// Called when this shot just dissapears
    /// </summary>
    private event Action<GameObject> ShotDisappeared;

    /// <summary>
    /// Direction for this shot to travel to 
    /// </summary>
    private Vector3 _direction = Vector3.left;
    public Vector3 Direction { get => _direction; set => _direction = value; }

    /// <summary>
    /// Rigidbody controlling physics for this shot
    /// </summary>
    private Rigidbody _rigidbody;

    /// <summary>
    /// If currently dissapearing 
    /// </summary>
    private bool _dissapearing = false;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _rigidbody = GetComponent<Rigidbody>();
    }

    
    /// <summary>
    /// Shoot at given direction
    /// </summary>
    public void ShotAt(Vector3 dir)
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

        transform.LookAt(100 * dir + transform.position);
        _rigidbody.useGravity = false;
        _direction = dir;
        _rigidbody.velocity = _direction * _speed;
    }

    /// <summary>
    /// Make this shot smaller over time 
    /// </summary>
    private IEnumerator Dissapear()
    {
        var timeEllapsed = 0.0f;
        _dissapearing = true;
        var initialScale = transform.localScale;
        while (timeEllapsed <= _timeToDissapear)
        {
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, timeEllapsed / _timeToDissapear);

            timeEllapsed += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        _dissapearing = false;
    }

    private void OnEnable()
    {
        transform.localScale = _originalScale;

    }

    private void OnCollisionEnter(Collision collision)
    {
        // use gravity when shot hits something so it reacts properly
        if (!_dissapearing)
            StartCoroutine(Dissapear());

        // Check if game object was a plate to burn
        collision
            .collider
            .gameObject
            .GetComponent<Plate>()
            ?.BurnPlate();
		
		// add collision to count the hits
        if (collision.gameObject.tag != null)
        {
            if (collision.gameObject.tag == "plateTag")
            {
                Counter.Hits += 1;
            }
        }
    }


    public void RegisterShotDisappeared(Action<GameObject> handler)
    {
        ShotDisappeared += handler;
    }


    public void UnregisterShotDisappeared(Action<GameObject> handler)
    {
        ShotDisappeared -= handler;
    }

    private void StopGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ContinueGame()
    {
        Debug.Log(Counter.Hits);
        SceneManager.LoadScene(1);
    }
}
