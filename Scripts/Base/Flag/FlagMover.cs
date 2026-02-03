using System.Collections;
using UnityEngine;

public class FlagMover : MonoBehaviour
{
    [SerializeField] private RayCaster _rayCaster;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private Vector3 _placementHitOffset;

    private Coroutine _moveFlagCoroutine;

    public void StartMoveFlagToMouse(Flag flag)
    {
        StopMoveFlagToMouse();
        _moveFlagCoroutine = StartCoroutine(MoveFlagToMouseCoroutine(flag));
    }

    public void StopMoveFlagToMouse()
    {
        if (_moveFlagCoroutine == null)
            return;

        StopCoroutine(_moveFlagCoroutine);
        _moveFlagCoroutine = null;
    }

    private IEnumerator MoveFlagToMouseCoroutine(Flag flag)
    {
        Vector3 previousMousePosition = default;

        while (flag != null)
        {
            Vector3 currentMouseScreenPosition = Input.mousePosition;

            if (currentMouseScreenPosition != previousMousePosition)
            {
                if (_rayCaster.TryRayCastToMousePosition(out RaycastHit raycastHit, _groundLayerMask))
                {
                    flag.transform.position = raycastHit.point + _placementHitOffset;
                    previousMousePosition = currentMouseScreenPosition;
                }
            }

            yield return null;
        }

        _moveFlagCoroutine = null;
    }
}
