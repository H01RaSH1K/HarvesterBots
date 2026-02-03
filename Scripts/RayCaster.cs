using UnityEngine;

public class RayCaster : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxDistance;

    public bool TryRayCastToMousePosition(out RaycastHit hit, LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(ray, out hit, _maxDistance, layerMask);
    }
}
