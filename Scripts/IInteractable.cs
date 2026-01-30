using UnityEngine;

public interface IInteractable
{
    public abstract float InteractionRadiusSquared { get; }
    public abstract Vector3 InteractionPosition { get; }
    public abstract void BeInteracted(Unit unit);
}
