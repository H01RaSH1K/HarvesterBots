using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Counter))]
public class Base : MonoBehaviour, IInteractable
{
    [SerializeField] private UnitCreator _unitCreator;

    [SerializeField] private AvailableResourceProvider _resourceProvider;
    [SerializeField] private float _interactionRadius;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private int _startUnitCount = 0;
    [SerializeField] private int _unitCost = 3;
    [SerializeField] private int _baseCost = 5;
    [SerializeField] private int _minUnitsCountToBuild = 2;

    private Counter _resourceCounter;

    private float _interactionRadiusSquared;

    private List<Unit> _units;
    private Queue<Unit> _unassignedUnits;
    private Func<bool> _trySpendResourcesDelegate;

    public float InteractionRadiusSquared => _interactionRadiusSquared;
    public Vector3 InteractionPosition => transform.position;

    private void Awake()
    {
        _resourceCounter = GetComponent<Counter>();
        _interactionRadiusSquared = _interactionRadius * _interactionRadius;
        _units = new List<Unit>();
        _unassignedUnits = new Queue<Unit>();
        SetPriorityToCreateUnits();
    }

    private void OnEnable()
    {
        foreach (Unit unit in _units)
            unit.ResourceCollected += OnResourceCollectedByUnit;

        _resourceProvider.NewResourceAvailable += OnNewResourceAvailable;
    }

    private void Start()
    {
        for (int i = 0; i < _startUnitCount; i++)
            CreateUnit();
    }

    private void OnDisable()
    {
        foreach (Unit unit in _units)
            unit.ResourceCollected -= OnResourceCollectedByUnit;

        _resourceProvider.NewResourceAvailable -= OnNewResourceAvailable;
    }

    public void Initialize(UnitCreator unitCreator, ResourceClaimer resourceClaimer, Unit unit)
    {
        _unitCreator = unitCreator;
        _resourceProvider.Initialize(resourceClaimer);
        AddUnit(unit);
    }

    public void Interact(Unit unit)
    {
        _unassignedUnits.Enqueue(unit);
        Resource resource = unit.ResourceCarrier.DropResource();

        if (resource != null)
        {
            _resourceCounter.Increment();
            resource.Expire();
            _trySpendResourcesDelegate();
        }

        AssignResources();
    }

    public bool TrySetPriorityToBuild(IInteractable flag)
    {
        if (_units.Count < _minUnitsCountToBuild)
            return false;

        _trySpendResourcesDelegate = GetBuildBaseDelegate(flag);
        return true;
    }

    public void SetPriorityToCreateUnits()
    {
        _trySpendResourcesDelegate = TryBuyUnit;
    }

    private void OnNewResourceAvailable()
    {
        AssignResources();
    }

    private void OnResourceCollectedByUnit(Unit unit)
    {
        unit.MoveToInteract(this);
    }

    private void AssignResources()
    {
        Unit unit;

        while (_unassignedUnits.TryDequeue(out unit))
        {
            if (_resourceProvider.TryClaimAvailableResource(out Resource resource) == false)
            {
                _unassignedUnits.Enqueue(unit);
                return;
            }

            unit.MoveToInteract(resource);
        }
    }

    private void AddUnit(Unit unit)
    {
        _units.Add(unit);
        _unassignedUnits.Enqueue(unit);
        unit.ResourceCollected += OnResourceCollectedByUnit;
    }

    private void CreateUnit()
    {
        Unit unit = _unitCreator.CreateUnit();
        unit.transform.position = _unitSpawnPoint.position;
        AddUnit(unit);
    }

    private bool TryBuyUnit()
    {
        if (_resourceCounter.Count < _unitCost)
            return false;

        _resourceCounter.Substract(_unitCost);
        CreateUnit();
        return true;
    }

    private Func<bool> GetBuildBaseDelegate(IInteractable flag)
    {
        bool tryBuildBase()
        {
            if (_resourceCounter.Count < _baseCost)
                return false;

            if (_unassignedUnits.TryDequeue(out Unit unit) == false)
                return false;

            _resourceCounter.Substract(_baseCost);
            unit.MoveToInteract(flag);
            _units.Remove(unit);
            SetPriorityToCreateUnits();
            return true;
        }

        return tryBuildBase;
    }
}
