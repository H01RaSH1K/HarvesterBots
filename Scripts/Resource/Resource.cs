using System;
using UnityEngine;

public class Resource : MonoBehaviour, IInteractable
{
    [SerializeField] private float _interactionRadiusSquared;

    public event Action<Resource> Expired;

    public float InteractionRadiusSquared => _interactionRadiusSquared;
    public Vector3 InteractionPosition => transform.position;

    public void Interact(Unit unit)
    {
        unit.CollectResourse(this);
    }

    public void Expire()
    {
        Expired?.Invoke(this);
    }
}
