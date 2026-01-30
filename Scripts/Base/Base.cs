using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Counter))]
public class Base : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactionRadius;
    [SerializeField] private UnitCreator _unitCreator;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private int _startUnitCount;
    [SerializeField] private ResourceClaimer _resourceClaimer;
    [SerializeField] private ResourceScanner _resourceScanner;

    private Counter _resourceCounter;

    private float _interactionRadiusSquared;

    private List<Unit> _units;
    private List<Unit> _unassignedUnits;
    private List<Resource> _unassignedResources;

    public float InteractionRadiusSquared => _interactionRadiusSquared;

    public Vector3 InteractionPosition => transform.position;

    private void Awake()
    {
        _resourceCounter = GetComponent<Counter>();
        _interactionRadiusSquared = _interactionRadius * _interactionRadius;
        _units = new List<Unit>();
        _unassignedUnits = new List<Unit>();
        _unassignedResources = new List<Resource>();
    }

    private void OnEnable()
    {
        foreach (Unit unit in _units)
            unit.ResourceCollected += OnResourceCollectedByUnit;

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
        foreach (Unit unit in _units)
            unit.ResourceCollected -= OnResourceCollectedByUnit;

        _resourceScanner.StopScanning();
        _resourceScanner.ResourceLost -= OnResourceLost;
        _resourceScanner.ResourceFound -= OnResourceFound;
    }

    public void Interact(Unit unit)
    {
        Resource resource = unit.ResourceCarrier.DropResource();

        if (resource != null)
        {
            _resourceCounter.Increment();
            resource.Expire();
        }

        _unassignedUnits.Add(unit);
        AssignResources();
    }

    private void OnResourceFound(Resource resource)
    {
        _unassignedResources.Add(resource);
        AssignResources();
    }

    private void OnResourceCollectedByUnit(Unit unit)
    {
        unit.MoveToInteract(this);
    }

    private void OnResourceLost(Resource resource)
    {
        if (_unassignedResources.Contains(resource))
            _unassignedResources.Remove(resource);
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
            _resourceClaimer.ClaimResource(resource);
            unit.MoveToInteract(resource);
        }
    }

    private void AddUnit()
    {
        Unit unit = _unitCreator.CreateUnit();
        unit.transform.position = _unitSpawnPoint.position;
        _units.Add(unit);
        _unassignedUnits.Add(unit);
        unit.ResourceCollected += OnResourceCollectedByUnit;
    }
}
