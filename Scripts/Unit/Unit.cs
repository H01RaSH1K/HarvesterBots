using System;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
[RequireComponent(typeof(ResourceCarrier))]
public class Unit : MonoBehaviour
{
    public UnitMover UnitMover { get; private set; }
    public ResourceCarrier ResourceCarrier { get; private set; }

    public event Action<Unit> ResourceCollected;


    private void Awake()
    {
        UnitMover = GetComponent<UnitMover>();
        ResourceCarrier = GetComponent<ResourceCarrier>();
    }

    private void OnEnable()
    {
        UnitMover.InteractableReached += Interact;
    }

    private void OnDisable()
    {
        UnitMover.InteractableReached -= Interact;
    }

    public void MoveToInteract(IInteractable interactable)
    {
        UnitMover.MoveToInteractable(interactable);
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
