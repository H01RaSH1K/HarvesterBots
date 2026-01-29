using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceScanner : MonoBehaviour
{
    [SerializeField] private float _scanRadius;
    [SerializeField] private float _scanInterval;
    [SerializeField] private LayerMask _layerMask = new LayerMask();
    [SerializeField] private int _startBufferSize;

    private WaitForSeconds _waitForScan;
    private Coroutine _scanningCoroutine;

    private Collider[] _overlapResultsBuffer;
    private HashSet<Resource> _lastScanResults;
    private HashSet<Resource> _newScanResults;

    public event Action<Resource> ResourceFound;
    public event Action<Resource> ResourceLost;

    private void Awake()
    {
        _waitForScan = new WaitForSeconds(_scanInterval);
        _overlapResultsBuffer = new Collider[_startBufferSize];
        _lastScanResults = new HashSet<Resource>();
        _newScanResults = new HashSet<Resource>();
    }

    private void OnDisable()
    {
        StopScanning();
    }

    public void StartScanning()
    {
        StopScanning();
        _scanningCoroutine = StartCoroutine(ScanningCoroutine());
    }

    public void StopScanning()
    {
        if (_scanningCoroutine == null)
            return;

        StopCoroutine(_scanningCoroutine);
        _scanningCoroutine = null;

        foreach (Resource resource in _lastScanResults)
            OnResourceLost(resource);

        _lastScanResults.Clear();
    }

    public void Scan()
    {
        FillNewScanResults();

        foreach (Resource resource in _lastScanResults)
            if (_newScanResults.Contains(resource) == false)
                OnResourceLost(resource);

        foreach (Resource resource in _newScanResults)
            if (_lastScanResults.Contains(resource) == false)
                OnResourceFound(resource);

        HashSet<Resource> temp = _lastScanResults;
        _lastScanResults = _newScanResults;
        _newScanResults = temp;
    }

    private IEnumerator ScanningCoroutine()
    {
        while (enabled)
        {
            yield return _waitForScan;
            Scan();
        }

        _scanningCoroutine = null;
    }

    private void FillNewScanResults()
    {
        _newScanResults.Clear();
        int overlappedCount = OverlapSphere();

        for (int i = 0; i < overlappedCount; i++)
        {
            Collider collider = _overlapResultsBuffer[i];
                
            if (collider.TryGetComponent(out Resource resource))
                _newScanResults.Add(resource);
        }
    }

    private void ResizeBuffer(int count)
    {
        int size = Mathf.NextPowerOfTwo(++count);
        _overlapResultsBuffer = new Collider[size];
    }

    private int OverlapSphere()
    {
        int overlappedCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            _scanRadius,
            _overlapResultsBuffer,
            _layerMask
        );

        while (overlappedCount >= _overlapResultsBuffer.Length)
        {
            ResizeBuffer(_overlapResultsBuffer.Length);

            overlappedCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                _scanRadius,
                _overlapResultsBuffer,
                _layerMask
            );
        } 

        return overlappedCount;
    }

    private void OnResourceLost(Resource resource)
    {
        resource.Taken -= OnResourceLost;
        ResourceLost?.Invoke(resource);
    }

    private void OnResourceFound(Resource resource)
    {
        resource.Taken += OnResourceLost;
        ResourceFound?.Invoke(resource);
    }
}
