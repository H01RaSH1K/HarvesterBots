using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Counter))]
public class Base : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactionRadius;
    [SerializeField] private UnitCreator _unitCreator;
    [SerializeField] private Transform _unitSpawnPoint;
    [SerializeField] private int _startUnitCount;
    [SerializeField] private AvailableResourceProvider _resourceProvider;

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

        _resourceProvider.NewResourceAvailable += OnNewResourceAvailable;
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

        _resourceProvider.NewResourceAvailable -= OnNewResourceAvailable;
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
        AssignResources();
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

    private void AddUnit()
    {
        Unit unit = _unitCreator.CreateUnit();
        unit.transform.position = _unitSpawnPoint.position;
        _units.Add(unit);
        _unassignedUnits.Enqueue(unit);
        unit.ResourceCollected += OnResourceCollectedByUnit;
    }
}
