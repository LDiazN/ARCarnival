using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this class to manage multiple spawning of objects that may appear and disappear 
/// </summary>
public class ObjectPooler : MonoBehaviour
{

    /// <summary>
    /// How many objects to pool as the game starts
    /// </summary>
    [Min(0)]
    [SerializeField]
    private int _initialPoolSize;
    public int InitialPoolSize { get => _initialPoolSize; }

    /// <summary>
    /// Object to manage in pool
    /// </summary>
    [SerializeField]
    private GameObject _objectToPool;
    public GameObject ObjectToPool { get => _objectToPool; }

    /// <summary>
    /// List of stored objects
    /// </summary>
    private Stack<GameObject> _pool;  

    private void Awake()
    {
        // Check for consistency 
        Debug.Assert(_objectToPool != null, "Object pool not properly configured: Missing object to pool");
        
        // Init object buffer:
        _pool = new Stack<GameObject>(_initialPoolSize);
        for(var i = 0;  i < _initialPoolSize; i++)
        {
            // Create new object
            var obj = SpawnNewObject();

            // Deactivate object
            obj.SetActive(false);

            // Store object 
            _pool.Push(obj);
        }

    }


    /// <summary>
    /// Return a game object stored in this pool. If not 
    /// enough objects in pool, create a new one
    /// </summary>
    /// <returns></returns>
    public GameObject GetObject()
    {
        GameObject obj;
        if (_pool.Count == 0)
            obj = SpawnNewObject();
        else
        {
            obj = _pool.Pop();
            obj.SetActive(true);
        }

        return obj;
    }
    
    /// <summary>
    /// Return a game object to the pull. Note that this function won't check if the object
    /// is the same as the stored template one, so be carefull to store the correct object
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Push(obj);
    }

    /// <summary>
    /// Return a new clone of the pooled game object
    /// </summary>
    /// <returns> New non-pooled object </returns>
    protected virtual GameObject SpawnNewObject()
    {
        return Instantiate(_objectToPool);
    }
}
