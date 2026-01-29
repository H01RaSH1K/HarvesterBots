using System;
using UnityEngine;

public class Resource : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactionRadiusSquared;

    public float InteractionRadiusSquared => _interactionRadiusSquared;
    public Vector3 InteractionPosition => transform.position;
    public bool IsTaken { get; private set; }

    public event Action<Resource> Taken;
    public event Action<Resource> Expired;

    private void OnEnable()
    {
        IsTaken = false;
    }

    public void BeInteracted(Unit unit)
    {
        unit.ResourceCarrier.TakeResource(this);
        IsTaken = true;
        Taken?.Invoke(this);
    }

    public void Expire()
    {
        Expired?.Invoke(this);
    }
}
