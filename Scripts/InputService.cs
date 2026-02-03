using System;
using UnityEngine;

public class InputService : MonoBehaviour
{
    public event Action LeftMouseButtonDowned;
    public event Action RightMouseButtonDowned;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            LeftMouseButtonDowned?.Invoke();

        if (Input.GetMouseButtonDown(1))
            RightMouseButtonDowned?.Invoke();
    }
}
