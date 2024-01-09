using UnityEngine;

namespace Services.Pool
{
    [CreateAssetMenu(menuName = "Pool/PoolData", fileName = "New PoolData", order = 51)]
    public class PoolData : ScriptableObject
    {
        public PooledObjectTypes Type;
        public GameObject PooledObject;
        public int Capacity;
    }
}