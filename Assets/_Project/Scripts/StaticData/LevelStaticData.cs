using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace StaticData
{
    [CreateAssetMenu(fileName = "New LevelData", menuName = "StaticData/LevelData")]
    public class LevelStaticData : ScriptableObject
    {
        public string LevelKey;
        public Vector3 PlayerSpawnPoint;
        public Quaternion PlayerRotation;
        public List<EnvironmentObjectSpawnData> EnvironmentObjectsSpawnData;

        public EnvironmentObjectSpawnData GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId typeId)
        {
            return EnvironmentObjectsSpawnData.Find(x => x.GameObjectsTypeId == typeId);
        }
    }
}