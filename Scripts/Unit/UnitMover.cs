using System;
using System.Collections;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 3.5f;

    public event Action<IInteractable> InteractableReached;

    private Coroutine _movingCoroutine;

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
            float maxStepDistanceSquared = maxStepDistance * maxStepDistance;

            if (remainingDistanceSquared <= maxStepDistanceSquared)
            {
                transform.position = targetPosition;
                break;
            }

            Vector3 direction = toTarget.normalized;
            transform.position = currentPosition + direction * maxStepDistance;
            yield return null;
        }

        if (interactable != null)
            InteractableReached?.Invoke(interactable);

        _movingCoroutine = null;
    }
}
