using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameObjectsScripts;
using GameObjectsScripts.Timers;
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

            LoadPrefab(typeof(MealDrawer), AssetPaths.MealDrawerPath);
            LoadPrefab(typeof(BackpackDrawer), AssetPaths.BackpackPath);
            LoadPrefab(typeof(Egg), AssetPaths.EggPath);
            LoadPrefab(typeof(Player), AssetPaths.PlayerPath);
            LoadPrefab(typeof(Snowman), AssetPaths.SnowmanPath);
            LoadPrefab(typeof(TrayView), AssetPaths.TrayPath);
            LoadPrefab(typeof(Poop), AssetPaths.PoopPath);
            LoadPrefab(typeof(ClothsDrawer), AssetPaths.ClothsDrawerPath);
            LoadPrefab(typeof(Carpet), AssetPaths.CarpetPath);
            LoadPrefab(typeof(ToysDrawer), AssetPaths.ToysDrawerPath);
        }


        private async void LoadPrefab(Type type, string path)
        {
            GameObject prefab = await _assetProvider.LoadAsync<GameObject>(path);
            _prefabsStorage.Register(type, prefab);
            
            _assetProvider.ReleaseAssetsByLabel(path);
        }

        private async void LoadPrefabs(string path)
        {
            List<string> list = await _assetProvider.GetAssetsListByLabel(path);
            GameObject[] prefabs = await _assetProvider.LoadAllAsync<GameObject>(list);
         
            
            foreach (GameObject gameObject in prefabs)
            {
                Debug.Log($"Registered game object {gameObject} name: {gameObject.name} type: {Type.GetType(gameObject.name)}");
                _prefabsStorage.Register(Type.GetType(gameObject.name), gameObject);
            }

            foreach (string label in list)
            {
                _assetProvider.ReleaseAssetsByLabel(label);
            }
        }
    }
}