using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameObjectsScripts;
using Infrastructure.PickableObjectSpawners;
using PlayerScripts;
using UnityEngine;
using Zenject;

namespace Infrastructure.AssetManagement
{
    public class AssetLoader : IInitializable
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IPrefabsStorage _prefabsStorage;

        public AssetLoader(IAssetProvider assetProvider, IPrefabsStorage prefabsStorage)
        {
            _assetProvider = assetProvider;
            _prefabsStorage = prefabsStorage;
        }

        public void Initialize()
        {
            Debug.Log($"Asset loader init");

            LoadPrefabs(AssetPaths.UILabel);
            LoadPrefab(typeof(ProgressUpdater), AssetPaths.ProgressUpdater);

            //Methods below can move to another loader
            LoadPrefab(typeof(Plate), AssetPaths.PlatePath);
            LoadPrefab(typeof(Crate), AssetPaths.CratePath);
            LoadPrefab(typeof(Egg), AssetPaths.EggPath);
            LoadPrefab(typeof(Player), AssetPaths.PlayerPath);
            LoadPrefab(typeof(Snowman), AssetPaths.SnowmanPath);
            LoadPrefab(typeof(Hat), AssetPaths.HatPath);
            LoadPrefab(typeof(Tray), AssetPaths.TrayPath);
        }


        private async void LoadPrefab(Type type, string path)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(path);
            _prefabsStorage.Register(type, prefab);
            
            _assetProvider.ReleaseAssetsByLabel(path);
        }

        private async void LoadPrefabs(string path)
        {
            IList<string> list = await _assetProvider.GetAssetsListByLabel(path);
            IList<GameObject> prefabs = await _assetProvider.LoadAllAsync<GameObject>(path);

            foreach (GameObject gameObject in prefabs)
            {
                _prefabsStorage.Register(Type.GetType(gameObject.name), gameObject);
            }

            foreach (string label in list)
            {
                _assetProvider.ReleaseAssetsByLabel(label);
            }
        }
    }
}