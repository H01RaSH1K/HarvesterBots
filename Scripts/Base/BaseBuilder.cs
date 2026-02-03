using UnityEngine;

public class BaseBuilder : MonoBehaviour
{
    [SerializeField] private UnitCreator _unitCreator;
    [SerializeField] private ResourceClaimer _resourceClaimer;

    [SerializeField] private Base _basePrefab;

    public void BuildBase(Vector3 position, Unit builder)
    {
        Base newBase = Instantiate(_basePrefab, position, Quaternion.identity);
        newBase.Initialize(_unitCreator, _resourceClaimer, builder);
    }
}
