using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Infrastructure.Services;
using UnityEngine.AddressableAssets;

namespace Infrastructure.AssetManagement
{
    public interface IAssetProvider : IService
    {
        void Initialize();
        UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference) where TAsset : class;
        UniTask<TAsset> LoadAsync<TAsset>(string key) where TAsset : class;
        UniTask<TAsset[]> LoadAllAsync<TAsset>(List<string> keys) where TAsset : class;
        UniTask<IList<TAsset>> LoadAllAsync<TAsset>(string key) where TAsset : class;
        UniTask<List<string>> GetAssetsListByLabel<TAsset>(string label);
        UniTask<List<string>> GetAssetsListByLabel(string label, Type type = null);
        UniTask WarmupAssetsByLabel(string label);
        UniTask ReleaseAssetsByLabel(string label);
        void Cleanup();
    }
}