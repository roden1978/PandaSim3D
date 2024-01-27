using System.Collections.Generic;
using TriInspector;
using UnityEngine;

namespace StaticData
{
    [CreateAssetMenu(fileName = "New LevelData", menuName = "StaticData/LevelData")]
    public class LevelStaticData : ScriptableObject
    {
        [CustomReadOnly]
        public string LevelKey;
        [CustomReadOnly]
        public Vector3 PlayerSpawnPoint;
        [CustomReadOnly]
        public Quaternion PlayerRotation;
        [ReadOnly]
        public List<EnvironmentObjectSpawnData> EnvironmentObjectsSpawnData;
        [ReadOnly]
        public List<StuffSpawnData> StuffSpawnData;

        public EnvironmentObjectSpawnData GetEnvironmentObjectSpawnDataByTypeId(GameObjectsTypeId typeId)
        {
            return EnvironmentObjectsSpawnData.Find(x => x.GameObjectsTypeId == typeId);
        }

        public StuffSpawnData GetStuffSpawnDataBySpecies(StuffSpecies species)
        {
            return StuffSpawnData.Find(x => x.StuffSpecies == species);
        }
    }
}