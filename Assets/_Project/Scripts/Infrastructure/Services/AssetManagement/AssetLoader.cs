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

        private readonly IPersistentProgress _persistentProgress;
        private Dictionary<string, UniTask<GameObject>> _handles;

        public AssetLoader(IAssetProvider assetProvider, IPrefabsStorage prefabsStorage,
            IPersistentProgress persistentProgress)
        {
            _assetProvider = assetProvider;
            _prefabsStorage = prefabsStorage;
            _persistentProgress = persistentProgress;
            _handles = new Dictionary<string, UniTask<GameObject>>();
        }

        public void Initialize()
        {
            Debug.Log($"Asset loader init");

            LoadUI();
            LoadProgressUpdater();

            //Methods below can move to another loader
            LoadPlatePrefab();
            LoadCratePrefab();
            LoadPlayerPrefab();
            LoadEggPrefab();
            LoadSnowmanPrefab();
            //////////////////////////////////////
            foreach (var gameObject in _prefabsStorage.GetAll())
            {
                Debug.Log($"Prefab in storage {gameObject.name}");
            }
        }

        private async void LoadUI()
        {
            IList<GameObject> assets = await _assetProvider.LoadAllAsync<GameObject>(AssetPaths.UILabel);

            foreach (GameObject gameObject in assets)
            {
                _prefabsStorage.Register(Type.GetType(gameObject.name), gameObject);
            }
        }

        private async void LoadProgressUpdater()
        {
            GameObject progressUpdater = await _assetProvider.LoadAsync<GameObject>(AssetPaths.ProgressUpdater);
            _prefabsStorage.Register(typeof(ProgressUpdater), progressUpdater);
        }

        private async void LoadPlatePrefab()
        {
            GameObject platePrefab = await _assetProvider.LoadAsync<GameObject>(AssetPaths.PlatePath);
            _prefabsStorage.Register(typeof(Plate), platePrefab);
        }
        
        private async void LoadCratePrefab()
        {
            GameObject cratePrefab = await _assetProvider.LoadAsync<GameObject>(AssetPaths.CratePath);
            _prefabsStorage.Register(typeof(Crate), cratePrefab);
        }

        private async void LoadEggPrefab()
        {
            GameObject eggPrefab = await _assetProvider.LoadAsync<GameObject>(AssetPaths.EggPath);
            _prefabsStorage.Register(typeof(Egg), eggPrefab);
        }
        private async void LoadSnowmanPrefab()
        {
            GameObject snowman = await _assetProvider.LoadAsync<GameObject>(AssetPaths.SnowmanPath);
            _prefabsStorage.Register(typeof(Snowman), snowman);
        }

        private async void LoadPlayerPrefab()
        {
            GameObject playerPrefab = await _assetProvider.LoadAsync<GameObject>(AssetPaths.PlayerPath);
            _prefabsStorage.Register(typeof(Player), playerPrefab);
        }


        /*private async void LoadUIExample2()
        {
            List<string> listByLabel = await _assetProvider.GetAssetsListByLabel<GameObject>(AssetPaths.UILabel);
            foreach (string name in listByLabel)
            {
                Debug.Log($"Label: {name.ParseAssetPath()}");
                _handles[name.ParseAssetPath()] = _assetProvider.LoadAsync<GameObject>(name);
            }

            GameObject[] assets = await UniTask.WhenAll(_handles.Values);

            foreach (GameObject gameObject in assets)
            {
                _prefabsStorage.Register(Type.GetType(gameObject.name), gameObject);
            }

            _handles.Clear();
        }

        private async void LoadUIExample3()
        {
            List<string> listByLabel = await _assetProvider.GetAssetsListByLabel<GameObject>(AssetPaths.UILabel);
            GameObject[] assets = await _assetProvider.LoadAllAsync<GameObject>(listByLabel);

            foreach (GameObject gameObject in assets)
            {
                _prefabsStorage.Register(Type.GetType(gameObject.name), gameObject);
            }
        }*/
    }
}