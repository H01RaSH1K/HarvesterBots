using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceClaimer : MonoBehaviour
{
    private HashSet<Resource> _claimedResources;

    public event Action<Resource> ResourceClaimed;

    private void Awake()
    {
        _claimedResources = new HashSet<Resource>();
    }

    private void OnDisable()
    {
        foreach (var resource in _claimedResources)
            resource.Expired -= OnResourceExpired;

        _claimedResources.Clear();
    }

    public void ClaimResource(Resource resource)
    {
        if (_claimedResources.Contains(resource))
        {
            Debug.LogWarning("Attempt to claim resource that already claimed");
            return;
        }

        _claimedResources.Add(resource);
        resource.Expired += OnResourceExpired;
        ResourceClaimed?.Invoke(resource);
    }

    public bool IsClaimed(Resource resource)
    {
        return _claimedResources.Contains(resource);
    }

    private void OnResourceExpired(Resource resource)
    {
        resource.Expired -= OnResourceExpired;
        _claimedResources.Remove(resource);
    }
}
