using UnityEngine;

namespace Infrastructure.PickableObjectSpawners
{
    public class StuffSpawner : MonoBehaviour
    {
        /*private bool _pickedUp;
        private string _id;
        private IGameFactory _gameFactory;
        private GameObjectsTypeId _gameObjectsTypeId;
        public void Construct(GameFactory gameFactory, string spawnerId, GameObjectsTypeId gameObjectsTypeId)
        {
            _id = spawnerId;
            _gameObjectsTypeId = gameObjectsTypeId;
            _gameFactory = gameFactory;
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
            /*if (playerProgress.PickableObjectData.ClearedSpawners.Contains(_id))
                _pickedUp = true;
            else
                Spawn();#1#
        }

        private void Spawn()
        {
            /*GameObject pickableObject = _gameFactory.CreatePickableObject(_pickableObjectTypeId, transform);
            _pickableObject = pickableObject.GetComponent<PickableObject>();
            _pickableObject.PickUp += OnPickUp;
            Disable();#1#
        }*/
    }
}