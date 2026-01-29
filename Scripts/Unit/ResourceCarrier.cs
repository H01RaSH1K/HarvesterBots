using UnityEngine;

public class ResourceCarrier : MonoBehaviour
{
    [SerializeField] private Transform _carryAnchor;
    [SerializeField] private Vector3 _carryOffset;

    public Resource CarriedResource { get; private set; }

    public void TakeResource(Resource resource)
    {
        if (CarriedResource != null)
        {
            Debug.LogWarning("Attempt to take new resource while another resource is already held");
            return;
        }

        resource.transform.SetParent(_carryAnchor);
        resource.transform.localPosition = _carryOffset;
        CarriedResource = resource;
    }

    public Resource DropResource()
    {
        Resource resource = CarriedResource;
        CarriedResource = null;
        return resource;
    }
}
