using System;
using UnityEngine;

public class Counter : MonoBehaviour
{
    public int Count { get; private set; }

    public event Action CountChanged;

    public void Increment()
    {
        ChangeCount(1);
    }

    public void Decrement()
    {
        ChangeCount(-1);
    }

    public void Add(int count)
    {
        if (count > 0)
            ChangeCount(count);
    }

    public void Remove(int count)
    {
        if (count > 0)
            ChangeCount(-count);
    }

    private void ChangeCount(int delta)
    {
        if (Count + delta < 0)
            return;

        Count += delta;
        CountChanged?.Invoke();
    }
}
