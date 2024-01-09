using System.Collections.Generic;
using Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Services.Pool
{
    public class PoolService : IPoolService, IInitializable
    {
        private readonly IAssetProvider _assetProvider;

        private readonly Dictionary<PooledObjectTypes, Pool> _poolsRepository;

        private int _index;

        public PoolService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
            _poolsRepository = new Dictionary<PooledObjectTypes, Pool>();
        }
        public async void Initialize()
        {
            IList<PoolData> result = await _assetProvider.LoadAllAsync<PoolData>(AssetPaths.PoolsDataLabel);

            foreach (PoolData poolData in result)
            {
                _poolsRepository.Add(poolData.Type,
                    new Pool(poolData.Type, poolData.PooledObject, poolData.Capacity));
            }

            Debug.Log($"Pool count: {_poolsRepository.Count}");
        }

        public GameObject GetPooledObject(PooledObjectTypes type)
        {
            GameObject pooledObject = null;

            if (_poolsRepository.TryGetValue(type, out Pool pool))
            {
                if (pool.Count == 0)
                {
                    _index = 0;
                    for (int i = 0; i < pool.Capacity; i++)
                    {
                        pooledObject = Object.Instantiate(pool.PooledObject);
                        pooledObject.name = $"{type.ToString()}({i})";
                        pooledObject.SetActive(false);
                        pool.SetPooledObject(pooledObject);
                        _index = i;
                    }
                }

                pooledObject = pool.GetFirst();

                if (pooledObject.activeInHierarchy)
                {
                    _index = pool.Count - 1;
                    GameObject additional = Object.Instantiate(pool.PooledObject);
                    additional.name = $"{type.ToString()}({++_index})";
                    pool.SetPooledObject(additional);
                    return additional;
                }

                pooledObject = pool.GetPooledObject();
                pooledObject.SetActive(true);
                pool.SetPooledObject(pooledObject);
            }


            return pooledObject;
        }

        public void AddPool(PooledObjectTypes type, GameObject pooledObject, int capacity)
        {
            _poolsRepository.Add(type, new Pool(type, pooledObject, capacity));
        }

        public void ClearPools()
        {
            _poolsRepository.Clear();
        }

        public void ClearPool(PooledObjectTypes pooledObjectType)
        {
            if (_poolsRepository.TryGetValue(pooledObjectType, out Pool pool))
            {
                pool.ClearPool();
            }
        }

        public void ReturnToPool(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }
    }
}