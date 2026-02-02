using System;
using System.Collections;
using UnityEngine;

public class ResourceScanner : MonoBehaviour
{
    [SerializeField] private float _scanRadius;
    [SerializeField] private float _scanInterval;
    [SerializeField] private LayerMask _layerMask = new LayerMask();
    [SerializeField] private int _bufferSize;

    private WaitForSeconds _waitForScan;
    private Coroutine _scanningCoroutine;

    private Collider[] _overlapResultsBuffer;

    public event Action<Resource> ResourceDetected;

    private void Awake()
    {
        _waitForScan = new WaitForSeconds(_scanInterval);
        _overlapResultsBuffer = new Collider[_bufferSize];
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
    }

    public void Scan()
    {
        int overlappedCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            _scanRadius,
            _overlapResultsBuffer,
            _layerMask
        );

        for (int i = 0; i < overlappedCount; i++)
        {
            Collider collider = _overlapResultsBuffer[i];

            if (collider.TryGetComponent(out Resource resource))
                ResourceDetected?.Invoke(resource);
        }
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
}
