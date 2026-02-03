using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagPlacer : MonoBehaviour
{
    [SerializeField] private InputService _inputService;
    [SerializeField] private BaseBuilder _baseBuilder;
    [SerializeField] private RayCaster _rayCaster;
    [SerializeField] private FlagMover _flagMover;

    [SerializeField] private LayerMask _flagLayerMask;
    [SerializeField] private LayerMask _baseLayerMask;
    [SerializeField] private float _minDistanceToBase;

    private HashSet<Flag> _placedFlags;
    private Flag _flagToPlace = null;

    private void Awake()
    {
        _placedFlags = new HashSet<Flag>();
    }

    private void OnEnable()
    {
        _inputService.LeftMouseButtonDowned += OnLeftMouseButtonDowned;   
    }

    private void OnDisable()
    {
        _inputService.LeftMouseButtonDowned -= OnLeftMouseButtonDowned;
        AbandonFlag();
    }

    private void OnLeftMouseButtonDowned()
    {
        if (_flagToPlace == null)
            TryGetFlagToPlace();
        else
            TryPlaceFlag();
    }

    private void ResetFlag()
    {
        if (_flagToPlace == null)
            return;

        _flagToPlace.Reset();
        AbandonFlag();
    }

    private void AbandonFlag()
    {
        _flagMover.StopMoveFlagToMouse();
        _inputService.RightMouseButtonDowned -= ResetFlag;
        _flagToPlace = null;
    }

    private void TryGetFlagToPlace()
    {
        if (_rayCaster.TryRayCastToMousePosition(out RaycastHit hit, _flagLayerMask) == false)
            return;

        if (hit.collider.TryGetComponent(out Flag flag) == false) 
            return;

        if (_placedFlags.Contains(flag))
        {
            RemoveFlagFromPlaced(flag);
            flag.Base.SetPriorityToCreateUnits();
        }

        _flagToPlace = flag;
        _flagMover.StartMoveFlagToMouse(_flagToPlace);
        _inputService.RightMouseButtonDowned += ResetFlag;
    }

    private void TryPlaceFlag()
    {
        if (Physics.CheckSphere(_flagToPlace.transform.position, _minDistanceToBase, _baseLayerMask))
            return;

        if (_flagToPlace.Base.TrySetPriorityToBuild(_flagToPlace) == false)
            return;

        _placedFlags.Add(_flagToPlace);
        _flagToPlace.Interacted += BuildBaseOnFlag;
        AbandonFlag();
    }

    private void BuildBaseOnFlag(Flag flag, Unit builder)
    {
        _baseBuilder.BuildBase(flag.transform.position, builder);
        RemoveFlagFromPlaced(flag);
    }

    private void RemoveFlagFromPlaced(Flag flag)
    {
        flag.Reset();
        flag.Interacted -= BuildBaseOnFlag;
        _placedFlags.Remove(flag);
    }
}
