using System;
using UnityEngine;

public class Flag : MonoBehaviour, IInteractable
{
    [SerializeField] private Base _base;
    [SerializeField] private float _interactionRadius;

    private float _interactionRadiusSquared;
    private Vector3 _initialLocalPosition;

    public event Action<Flag, Unit> Interacted;

    public Base Base => _base;
    public float InteractionRadiusSquared => _interactionRadiusSquared;
    public Vector3 InteractionPosition => transform.position;

    private void Awake()
    {
        _interactionRadiusSquared = _interactionRadius * _interactionRadius;
        _initialLocalPosition = transform.localPosition;
    }

    public void Reset()
    {
        transform.localPosition = _initialLocalPosition;
    }

    public void Interact(Unit unit)
    {
        Interacted?.Invoke(this, unit);
    }
}
