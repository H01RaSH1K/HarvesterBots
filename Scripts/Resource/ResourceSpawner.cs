using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private ResourcePool _resourcePool;

    [SerializeField] private float _spawnRadius = 10f;
    [SerializeField] private int _maximumActiveResources = 25;
    [SerializeField] private float _spawnIntervalMinInclusive = 5f;
    [SerializeField] private float _spawnIntervlMaxInclusive = 15f;
    [SerializeField] private int _spawnIntervalSamples = 10;

    private readonly List<Resource> _activeResources = new List<Resource>();

    private List<WaitForSeconds> _waitsForSpawn;
    private Coroutine _spawningCoroutine;

    private void Awake()
    {
        InitializeWaits();
    }

    private void OnEnable()
    {
        _spawningCoroutine = StartCoroutine(SpawningCoroutine());
    }

    private void OnDisable()
    {
        if (_spawningCoroutine == null)
            return;

        StopCoroutine(_spawningCoroutine);
        _spawningCoroutine = null;

        while (_activeResources.Count > 0)
            _activeResources[0].Expire();
    }

    private IEnumerator SpawningCoroutine()
    {
        while (enabled)
        {
            yield return GetRandomWait();
            TrySpawn();
        }

        _spawningCoroutine = null;
    }

    private void TrySpawn()
    {
        if (_activeResources.Count >= _maximumActiveResources)
            return;

        Resource resource = _resourcePool.GetObject();

        if (resource == null)
            return;

        resource.transform.position = GetRandomPositionInRadius();
        resource.Expired += OnResourceExpired;
        _activeResources.Add(resource);
    }

    private void OnResourceExpired(Resource resource)
    {
        _activeResources.Remove(resource);
        resource.Expired -= OnResourceExpired;
        _resourcePool.Release(resource);
    }

    private Vector3 GetRandomPositionInRadius()
    {
        Vector2 offset = Random.insideUnitCircle * _spawnRadius;
        Vector3 center = transform.position;

        return new Vector3(center.x + offset.x, center.y, center.z + offset.y);
    }

    private WaitForSeconds GetRandomWait()
    {
        int randomIndex = Random.Range(0, _waitsForSpawn.Count);
        return _waitsForSpawn[randomIndex];
    }

    private void InitializeWaits()
    {
        _waitsForSpawn = new List<WaitForSeconds>();

        for (int i = 0; i < _spawnIntervalSamples; i++)
            _waitsForSpawn.Add(new WaitForSeconds(Random.Range(_spawnIntervalMinInclusive, _spawnIntervlMaxInclusive)));
    }
}
