using UnityEngine;

public sealed class StartBillboard : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private void Start()
    {
        transform.forward = _target.forward;
    }
}