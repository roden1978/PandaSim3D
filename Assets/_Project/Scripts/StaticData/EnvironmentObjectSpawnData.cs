using System;
using UnityEngine;

namespace StaticData
{
    [Serializable]
    public class EnvironmentObjectSpawnData
    {
        public string Id;
        public GameObjectsTypeId GameObjectsTypeId;
        public Vector3 Position;
        public Quaternion Rotation;

        public EnvironmentObjectSpawnData(string id, GameObjectsTypeId gameObjectsTypeId, Vector3 position, Quaternion rotation)
        {
            Id = id;
            GameObjectsTypeId = gameObjectsTypeId;
            Position = position;
            Rotation = rotation;
        }
    }
}