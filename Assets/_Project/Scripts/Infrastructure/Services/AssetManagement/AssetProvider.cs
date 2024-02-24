using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Zenject;

namespace Infrastructure.AssetManagement
{
    public class AssetProvider : IAssetProvider, IInitializable
    {
        private readonly Dictionary<string, AsyncOperationHandle> assetRequests = new();

        public async void Initialize() =>
            await Addressables.InitializeAsync();

        public async UniTask<TAsset> LoadAsync<TAsset>(string key) where TAsset : class
        {
            if (!assetRequests.TryGetValue(key, out AsyncOperationHandle handle))
            {
                handle = Addressables.LoadAssetAsync<TAsset>(key);
                assetRequests.Add(key, handle);
            }

            await handle.ToUniTask();
            
            handle.Completed += _ => Debug.Log($"Was loaded {handle.DebugName}");

            return handle.Result as TAsset;
        }

        public async UniTask<IList<TAsset>> LoadAllAsync<TAsset>(string key) where TAsset : class
        {
            AsyncOperationHandle<IList<TAsset>> handle =
                Addressables.LoadAssetsAsync<TAsset>(key, x => Debug.Log($"Was loaded {x.GetType()}"));
            await handle.ToUniTask();

            return handle.Result;
        }

        public async UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference) where TAsset : class =>
            await LoadAsync<TAsset>(assetReference.AssetGUID);

        public async UniTask<TAsset[]> LoadAllAsync<TAsset>(List<string> keys) where TAsset : class
        {
            List<UniTask<TAsset>> tasks = new(keys.Count);
            tasks.AddRange(Enumerable.Select(keys, LoadAsync<TAsset>));

            return await UniTask.WhenAll(tasks);
        }

        public async UniTask<List<string>> GetAssetsListByLabel<TAsset>(string label) =>
            await GetAssetsListByLabel(label, typeof(TAsset));

        public async UniTask<List<string>> GetAssetsListByLabel(string label, Type type = null)
        {
            AsyncOperationHandle<IList<IResourceLocation>> operationHandle =
                Addressables.LoadResourceLocationsAsync(label, type);

            IList<IResourceLocation> locations = await operationHandle.ToUniTask();

            List<string> assetKeys = new(locations.Count);

            foreach (IResourceLocation location in locations)
                assetKeys.Add(location.PrimaryKey);

            Addressables.Release(operationHandle);
            foreach (string key in assetKeys)
            {
                Debug.Log($"Key: {key}");
            }

            return assetKeys;
        }

        public async UniTask WarmupAssetsByLabel(string label)
        {
            List<string> assetsList = await GetAssetsListByLabel(label);
            await LoadAllAsync<object>(assetsList);
        }

        public async UniTask ReleaseAssetsByLabel(string label)
        {
            List<string> assetsList = await GetAssetsListByLabel(label);

            foreach (string assetKey in assetsList)
                if (assetRequests.TryGetValue(assetKey, out AsyncOperationHandle handler))
                {
                    Addressables.Release(handler);
                    assetRequests.Remove(assetKey);
                }
        }

        public void Cleanup()
        {
            foreach (KeyValuePair<string, AsyncOperationHandle> assetRequest in assetRequests)
                Addressables.Release(assetRequest.Value);

            assetRequests.Clear();
        }
    }
}