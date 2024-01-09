using GameObjectsScripts;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure.PickableObjectSpawners
{
    public class PlateSpawner : MonoBehaviour, ISavedProgress
    {
        private IEventBus _eventBus;
        private IPrefabsStorage _prefabsStorage;
        private DialogManager _dialogManager;

        [Inject]
        public void Construct(IEventBus eventBus, DialogManager dialogManager, IPrefabsStorage prefabsStorage)
        {
            Debug.Log("Construct plate spawner");
            _eventBus = eventBus;
            _prefabsStorage = prefabsStorage;
            _dialogManager = dialogManager;
        }
        
        public void LoadProgress(PlayerProgress playerProgress)
        {
            Spawn();
        }

        private void Spawn()
        {
            Debug.Log("Spawn plate from spawner");
            GameObject prefab = _prefabsStorage.Get(typeof(Plate));
            Plate plate = Instantiate(prefab,transform.position, Quaternion.identity).GetComponent<Plate>();
            plate.Contruct(_eventBus, _dialogManager);
        }

        public void SaveProgress(PlayerProgress persistentPlayerProgress)
        {
            
        }
    }
}