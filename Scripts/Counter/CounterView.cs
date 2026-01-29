using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CounterView : MonoBehaviour
{
    [SerializeField] private Counter _counter;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        _counter.CountChanged += OnCountChanged;
        OnCountChanged();
    }

    private void OnDisable()
    {

        _counter.CountChanged -= OnCountChanged;
    }

    private void OnCountChanged()
    {
        _text.SetText(_counter.Count.ToString());
    }
}
