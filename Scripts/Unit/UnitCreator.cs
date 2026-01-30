using UnityEngine;

public class UnitCreator : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;

    public Unit CreateUnit()
    {
        return Instantiate(_unitPrefab, transform);
    }
}
