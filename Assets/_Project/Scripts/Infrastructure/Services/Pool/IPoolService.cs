using Cysharp.Threading.Tasks;
using Infrastructure.Services;
using UnityEngine;

namespace Services.Pool
{
    public interface IPoolService : IService
    {
        GameObject GetPooledObject(PooledObjectTypes type);
        void AddPool(PooledObjectTypes type, GameObject pooledObject, int capacity);
        void ClearPools();
        void ClearPool(PooledObjectTypes type);
        void ReturnToPool(GameObject gameObject);
    }
}