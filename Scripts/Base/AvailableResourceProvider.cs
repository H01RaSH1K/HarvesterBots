using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvailableResourceProvider : MonoBehaviour
{
    [SerializeField] private ResourceClaimer _resourceClaimer;

    [SerializeField] private ResourceScanner _resourceScanner;

    private HashSet<Resource> _resources;

    public event Action NewResourceAvailable;

    public bool IsInitialized => _resourceClaimer != null;

    private void Awake()
    {
        _resources = new HashSet<Resource>();
    }

    private void OnEnable()
    {
        if (IsInitialized == false)
            return;

        _resourceScanner.StopScanning();
        _resourceScanner.ResourceDetected += OnResourceDetected;
        _resourceScanner.StartScanning();
        _resourceClaimer.ResourceClaimed += RemoveResource;
    }

    private void OnDisable()
    {
        _resourceClaimer.ResourceClaimed -= RemoveResource;
        _resourceScanner.ResourceDetected -= OnResourceDetected;
        _resources.Clear();
    }

    public void Initialize(ResourceClaimer resourceClaimer)
    {
        if (IsInitialized)
            return;

        _resourceClaimer = resourceClaimer;
        OnEnable();
    }

    public bool TryClaimAvailableResource(out Resource resource)
    {
        resource = default;

        while (_resources.Count > 0)
        {
            resource = _resources.First();
            
            if (_resourceClaimer.TryClaimResource(resource))
                return true;

            _resources.Remove(resource);
        }

        return false;
    }

    private void OnResourceDetected(Resource resource)
    {
        if (_resourceClaimer.IsClaimed(resource))
            return;

        if (_resources.Add(resource) == false)
            return;

        resource.Expired += RemoveResource;
        NewResourceAvailable?.Invoke();
    }

    private void RemoveResource(Resource resource)
    {
        _resources.Remove(resource);
        resource.Expired -= RemoveResource;
    }
}
