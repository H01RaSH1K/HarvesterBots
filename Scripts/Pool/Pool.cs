using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Pool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T _objectPrefab;
    [SerializeField] private int _poolCapacity = 10;
    [SerializeField] private int _poolMaxSize = 50;

    private ObjectPool<T> _pool;

    protected void Awake()
    {
        _pool = new ObjectPool<T>(
            createFunc: CreateObject,
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleaseObject,
            actionOnDestroy: OnDestroyObject,
            collectionCheck: true,
            defaultCapacity: _poolCapacity,
            maxSize: _poolMaxSize
        );
    }

    public T GetObject()
    {
        T result = _pool.Get();
        result.transform.SetParent(transform);
        return result;
    }

    public void Release(T poolObject)
    {
        _pool.Release(poolObject);
    }

    private void OnGetObject(T poolObject)
    {
        poolObject.gameObject.SetActive(true);
    }

    private T CreateObject()
    {
        return Instantiate(_objectPrefab);
    }

    private void OnDestroyObject(T poolObject)
    {
        Destroy(poolObject);
    }

    private void OnReleaseObject(T poolObject)
    {
        poolObject.gameObject.SetActive(false);
    }
}
