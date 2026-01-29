//using UnityEngine;

//[RequireComponent(typeof(ResourceCarrier))]
//public class Interactor : MonoBehaviour, IInteractor
//{
//    private ResourceCarrier _resourceCarrier;

//    private void Awake()
//    {
//        _resourceCarrier = GetComponent<ResourceCarrier>();
//    }

//    public void Interact(IInteractable interactable)
//    {
//        interactable.AcceptInteractor(this);
//    }

//    public void BuildBase()
//    {
//        throw new System.NotImplementedException();
//    }

//    public void CollectResourse(Resource resource)
//    {
//        _resourceCarrier.TakeResource(resource);
//    }

//    public void GiveResourses()
//    {
//        throw new System.NotImplementedException();
//    }
//}
