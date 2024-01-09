using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameObjectsScripts
{
    public class Plate : MonoBehaviour, IPointerClickHandler
    {
        private IEventBus _eventBus;
        private DialogManager _dialogManager;

        [Inject]
        public void Contruct(IEventBus eventBus, DialogManager dialogManager)
        {
            _eventBus = eventBus;
            _dialogManager = dialogManager;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Click on plate");
            _dialogManager.ShowDialog<InventoryDialog>();
            _eventBus.Invoke(new UpdateInventoryView());
        }
    }
}