using System;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
[RequireComponent(typeof(ResourceCarrier))]
public class Unit : MonoBehaviour
{
    public UnitMover Mover { get; private set; }
    public ResourceCarrier ResourceCarrier { get; private set; }

    public event Action<Unit> ResourceCollected;

    private void Awake()
    {
        Mover = GetComponent<UnitMover>();
        ResourceCarrier = GetComponent<ResourceCarrier>();
    }

    private void OnEnable()
    {
        Mover.InteractableReached += Interact;
    }

    private void OnDisable()
    {
        Mover.InteractableReached -= Interact;
    }

    public void MoveToInteract(IInteractable interactable)
    {
        Mover.MoveToInteractable(interactable);
    }

    public void CollectResourse(Resource resource)
    {
        ResourceCarrier.TakeResource(resource);
        ResourceCollected?.Invoke(this);
    }

    private void Interact(IInteractable interactable)
    {
        interactable.Interact(this);
    }
}
