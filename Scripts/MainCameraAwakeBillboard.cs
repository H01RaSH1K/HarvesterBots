using UnityEngine;

public sealed class MainCameraAwakeBillboard : MonoBehaviour
{
    private void Awake()
    {
        transform.forward = Camera.main.transform.forward;
    }
}