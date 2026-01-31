using System;
using System.Collections.Generic;
using System.Linq;
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
    private Queue<Unit> _unassignedUnits;

    public float InteractionRadiusSquared => _interactionRadiusSquared;

    public Vector3 InteractionPosition => transform.position;

    private void Awake()
    {
        _resourceCounter = GetComponent<Counter>();
        _interactionRadiusSquared = _interactionRadius * _interactionRadius;
        _units = new List<Unit>();
        _unassignedUnits = new Queue<Unit>();
    }

    private void OnEnable()
    {
        foreach (Unit unit in _units)
            unit.ResourceCollected += OnResourceCollectedByUnit;

        _resourceScanner.StopScanning();
        _resourceScanner.ResourceDetected += OnResourcesDetected;
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
        _resourceScanner.ResourceDetected -= OnResourcesDetected;
    }

    public void Interact(Unit unit)
    {
        Resource resource = unit.ResourceCarrier.DropResource();

        if (resource != null)
        {
            _resourceCounter.Increment();
            resource.Expire();
        }

        _unassignedUnits.Enqueue(unit);
    }

    private void OnResourcesDetected(Resource resource)
    {
        TryAssignResource(resource);
    }

    private void OnResourceCollectedByUnit(Unit unit)
    {
        unit.MoveToInteract(this);
    }

    private bool TryAssignResource(Resource resource)
    {
        if (_resourceClaimer.IsClaimed(resource))
            return false;

        if (_unassignedUnits.TryDequeue(out Unit unit) == false) 
            return false;

        _resourceClaimer.ClaimResource(resource);
        unit.MoveToInteract(resource);
        return true;
    }

    private void AddUnit()
    {
        Unit unit = _unitCreator.CreateUnit();
        unit.transform.position = _unitSpawnPoint.position;
        _units.Add(unit);
        _unassignedUnits.Enqueue(unit);
        unit.ResourceCollected += OnResourceCollectedByUnit;
    }
}
