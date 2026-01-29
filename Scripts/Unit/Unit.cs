using System;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
[RequireComponent(typeof(ResourceCarrier))]
public class Unit : MonoBehaviour
{
    public UnitMover UnitMover { get; private set; }
    public ResourceCarrier ResourceCarrier { get; private set; }


    private void Awake()
    {
        UnitMover = GetComponent<UnitMover>();
        ResourceCarrier = GetComponent<ResourceCarrier>();
    }

    private void OnEnable()
    {
        UnitMover.InteractableReached += OnInteractableReached;
    }

    private void OnDisable()
    {
        UnitMover.InteractableReached -= OnInteractableReached;
    }

    public void MoveToInteract(IInteractable interactable)
    {
        UnitMover.MoveToInteractable(interactable);
    }

    private void OnInteractableReached(IInteractable interactable)
    {
        Interact(interactable);
    }

    public void BuildBase()
    {
        throw new System.NotImplementedException();
    }

    public void CollectResourse(Resource resource)
    {
    }

    public void GiveResourses()
    {
        throw new System.NotImplementedException();
    }

    private void Interact(IInteractable interactable)
    {
        interactable.BeInteracted(this);
    }
}
