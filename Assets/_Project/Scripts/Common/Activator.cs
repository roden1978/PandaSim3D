using Infrastructure.PickableObjectSpawners;
using UnityEngine;

namespace Common
{
    public class Activator : MonoBehaviour
    {
        //[SerializeField] private TriggerObserver _triggerObserver;

        private void OnEnable()
        {
            //_triggerObserver.TriggerEnter += Enable;
            //_triggerObserver.TriggerExit += Disable;
        }

        private void OnDisable()
        {
           // _triggerObserver.TriggerEnter -= Enable;
           // _triggerObserver.TriggerExit -= Disable;
        }

        private void Disable(Collider2D obj)
        {
            if(obj.TryGetComponent(out IActivator activator))
            {
                activator.Disable();
            }
        }

        private void Enable(Collider2D obj)
        {
            if(obj.TryGetComponent(out IActivator activator))
            {
                activator.Enable();
            }
        }
        
    }
}
