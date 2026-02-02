using System;
using System.Collections;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 3.5f;

    private Coroutine _movingCoroutine;

    public event Action<IInteractable> InteractableReached;

    public void MoveToInteractable(IInteractable interactable)
    {
        StopMoving();
        _movingCoroutine = StartCoroutine(MovingToInteractableCoroutine(interactable));
    }

    public void StopMoving()
    {
        if (_movingCoroutine == null)
            return;

        StopCoroutine(_movingCoroutine);
        _movingCoroutine = null;
    }

    private IEnumerator MovingToInteractableCoroutine(IInteractable interactable)
    {
        while (interactable != null)
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = interactable.InteractionPosition;
            targetPosition.y = currentPosition.y;
            Vector3 toTarget = targetPosition - currentPosition;
            float remainingDistanceSquared = toTarget.sqrMagnitude;

            if (remainingDistanceSquared <= interactable.InteractionRadiusSquared)
                break;

            float maxStepDistance = _movementSpeed * Time.deltaTime;
            Vector3 nextPosition = Vector3.MoveTowards(currentPosition, targetPosition, maxStepDistance);
            transform.position = nextPosition;
            yield return null;
        }

        if (interactable != null)
            InteractableReached?.Invoke(interactable);

        _movingCoroutine = null;
    }
}
