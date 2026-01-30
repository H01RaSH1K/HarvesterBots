using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Counter))]
public class Base : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactionRadius;
    [SerializeField] private UnitPool _unitPool;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private int _startUnitCount;
    [SerializeField] private int _unitCost;
    [SerializeField] private ResourceScanner _resourceScanner;

    private Counter _resourceCounter;

    private float _interactionRadiusSquared;

    private List<Unit> _unassignedUnits;
    private List<Resource> _unassignedResources;
    private Dictionary<Resource, Unit> _unitsByAssignedResource;
    private Func<bool> _trySpendResourcesDelegate;

    public float InteractionRadiusSquared => _interactionRadiusSquared;

    public Vector3 InteractionPosition => transform.position;

    private void Awake()
    {
        _resourceCounter = GetComponent<Counter>();
        _interactionRadiusSquared = _interactionRadius * _interactionRadius;
        _unassignedUnits = new List<Unit>();
        _unassignedResources = new List<Resource>();
        _unitsByAssignedResource = new Dictionary<Resource, Unit>();
        _trySpendResourcesDelegate = TryAddUnit;
    }

    private void OnEnable()
    {
        _resourceScanner.StopScanning();
        _resourceScanner.ResourceLost += OnResourceLost;
        _resourceScanner.ResourceFound += OnResourceFound;
        _resourceScanner.StartScanning();
    }

    private void Start()
    {
        for (int i = 0; i < _startUnitCount; i++)
            AddUnit();
    }

    private void OnDisable()
    {
        _resourceScanner.StopScanning();
        _resourceScanner.ResourceLost -= OnResourceLost;
        _resourceScanner.ResourceFound -= OnResourceFound;
    }

    public void BeInteracted(Unit unit)
    {
        Resource resource = unit.ResourceCarrier.DropResource();

        if (resource != null)
        {
            _resourceCounter.Increment();
            resource.Expire();
            _trySpendResourcesDelegate();
        }

        _unassignedUnits.Add(unit);
        AssignResources();
    }

    private void OnResourceFound(Resource resource)
    {
        _unassignedResources.Add(resource);
        AssignResources();
    }

    private void OnResourceLost(Resource resource)
    {
        if (_unassignedResources.Contains(resource))
            _unassignedResources.Remove(resource);

        if (_unitsByAssignedResource.TryGetValue(resource, out Unit assignedUnit))
        {
            assignedUnit.MoveToInteract(this);
            _unitsByAssignedResource.Remove(resource);
        }
    }

    private void AssignResources()
    {
        int minCount = Math.Min(_unassignedResources.Count, _unassignedUnits.Count);

        for (int i = 0; i < minCount; i++)
        {
            Unit unit = _unassignedUnits[i];
            Resource resource = _unassignedResources[i];
            _unassignedUnits.RemoveAt(i);
            _unassignedResources.RemoveAt(i);
            _unitsByAssignedResource[resource] = unit;
            unit.MoveToInteract(resource);
        }
    }

    private void AddUnit()
    {
        Unit unit = _unitPool.GetObject();
        unit.transform.position = _unitSpawnPoint.transform.position;
        _unassignedUnits.Add(unit);
    }

    private bool TryAddUnit()
    {
        if (_resourceCounter.Count < _unitCost)
            return false;

        _resourceCounter.Remove(_unitCost);
        AddUnit();
        return true;
    }
}
