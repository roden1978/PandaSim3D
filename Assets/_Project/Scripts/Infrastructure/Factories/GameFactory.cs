using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using PlayerScripts;
using Services.StaticData;
using UI;
using UnityEngine;
using Zenject;

namespace Infrastructure.Factories
{
    public class GameFactory : IGameFactory
    {
        public Player Player { get; private set; }
        public Hud Hud { get; set; }
        public GameObject UIRoot;
        private GameMenu _gameMenu;

        private List<ISavedProgressReader> _progressReaders { get; }

        private List<ISavedProgress> _progressWriters { get; }

        private readonly IPrefabsStorage _prefabsStorage;
        
        private readonly IStaticDataService _staticDataService;
        private readonly DiContainer _container;

        public GameFactory(IPrefabsStorage prefabsStorage, IStaticDataService staticDataService, DiContainer container)
        {
            _prefabsStorage = prefabsStorage;
            _staticDataService = staticDataService;
            _container = container;
            _progressReaders = new List<ISavedProgressReader>();
            _progressWriters = new List<ISavedProgress>();
        }

        public void StartFactory()
        {
            Debug.Log("Start factory");
            //CreateUIRoot();
            //CreateHud();
        }

        private void CreateHud()
        {
            
            GameObject hud = Object.Instantiate(_prefabsStorage.Get(typeof(Hud)), _container.ResolveId<Transform>("UIRoot"));
            Debug.Log($"Instantiate hud start {hud}");
            _container.Bind<Hud>().FromComponentInNewPrefab(hud).UnderTransformGroup("UIRoot").AsSingle()
                .NonLazy();
            Debug.Log($"Instantiate hud end {hud}");
        }

        private void CreateUIRoot()
        {
                /*GameObject uiRoot = Object.Instantiate(_assetContainer.Get(typeof(UIRoot))); 
                _container
                    .Bind<Transform>()
                    .FromComponentInNewPrefab(uiRoot)
                    .WithGameObjectName("UIRoot")
                    .AsSingle()
                    .NonLazy();*/
            //RegisterInSaveLoadRepositories(uiRoot);
        }

       
        public void CreateGameMenu()
        {
           //_gameMenu = _assetProvider.InstantiateGameMenu();
        }

        public Player CreatePlayer()
        {
            //Player = _assetProvider.InstantiatePlayer();
            RegisterInSaveLoadRepositories(Player.gameObject);
            return Player;
        }
        
        public Mediator CreateMediator()
        {
            //Mediator mediator = _assetProvider.InstantiateMediator();
           // RegisterInSaveLoadRepositories(mediator.gameObject);
            //return mediator;
            return null;
        }

       private void RegisterInSaveLoadRepositories(GameObject registeredGameObject)
        {
            foreach (ISavedProgressReader progressReader in registeredGameObject
                .GetComponentsInChildren<ISavedProgressReader>())
            {
                if (progressReader is ISavedProgress progressWriter)
                    AddProgressWriter(progressWriter);
                AddProgressReader(progressReader);
            }
        }

        private void AddProgressReader(ISavedProgressReader progressReader)
        {
            _progressReaders.Add(progressReader);
        }

        private void AddProgressWriter(ISavedProgress progressWriter)
        {
            _progressWriters.Add(progressWriter);
        }

        public void CleanUp()
        {
            _progressReaders.Clear();
            _progressWriters.Clear();
        }
    }
}