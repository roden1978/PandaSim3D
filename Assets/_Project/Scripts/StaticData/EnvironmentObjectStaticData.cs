using UnityEngine;

namespace StaticData
{
    [CreateAssetMenu(fileName = "New EnvironmentObject Data", menuName = "StaticData/EnvironmentObjectData")]
    public class EnvironmentObjectStaticData : ScriptableObject
    {
        public GameObject Prefab;
        public GameObjectsTypeId GameObjectsTypeId;
    }

   
}
