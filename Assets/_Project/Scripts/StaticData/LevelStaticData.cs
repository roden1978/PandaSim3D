using System.Collections.Generic;
using UnityEngine;

namespace StaticData
{
    [CreateAssetMenu(fileName = "New LevelData", menuName = "StaticData/LevelData")]
    public class LevelStaticData : ScriptableObject
    {
        [ReadOnly]
        public string LevelKey;
        [ReadOnly]
        public Vector3 PlayerSpawnPoint;
        [ReadOnly]
        public Quaternion PlayerRotation;
        [ReadOnly]
        public List<EnvironmentObjectSpawnData> EnvironmentObjectsSpawnData;

        public EnvironmentObjectSpawnData GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId typeId)
        {
            return EnvironmentObjectsSpawnData.Find(x => x.GameObjectsTypeId == typeId);
        }
    }
}