using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Infrastructure.Factories;
using Infrastructure.Services;
using Services.StaticData;
using StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Infrastructure.GameStates
{
    public class SpawnEntitiesState : IState
    {
        private const string EnemiesParentObjectName = "[Enemies]"; 
        private const string PickableParentObjectName = "[PickableObjects]";
        private const string SavePointsParentObjectName = "[SaveProgressPoints]";
        
        private readonly GamesStateMachine _stateMachine;
        private readonly ServiceLocator _serviceLocator;
        
        private IGameFactory _gameFactory;
        //private List<EnemySpawnMarker> _enemySpawnMarkers;
        private List<EnvironmentObjectMarker> _pickableObjectMarkers;
        //private List<SaveProgressPointMarker> _saveProgressPointMarkers;
        public SpawnEntitiesState(GamesStateMachine stateMachine, ServiceLocator serviceLocator)
        {
            _stateMachine = stateMachine;
            _serviceLocator = serviceLocator;
        }
        public void Enter() => 
            SpawnGameEntities(OnLoaded);
        private void SpawnGameEntities(Action callback)
        {
            InitSpawners();
            FindAllMarkersInScene();
            DestroyEnemyMarkers();
            DestroyPickableObjectsMarkers();
            DestroySavePointMarkers();
            ClearMarkersLists();
            callback?.Invoke();
        }

        private void ClearMarkersLists()
        {
            //_enemySpawnMarkers.Clear();
            _pickableObjectMarkers.Clear();
            //_saveProgressPointMarkers.Clear();
        }

        public void Update(){}
        public void Exit(){}
        private void OnLoaded() => 
            _stateMachine.Enter<UpdateProgressState>();
        private void InitSpawners()
        {
            _gameFactory = _serviceLocator.Single<IGameFactory>();
            string levelKey = SceneManager.GetActiveScene().name;
            LevelStaticData levelStaticData = _serviceLocator.Single<IStaticDataService>().GetLevelStaticData(levelKey);
            //SpawnEnemies(levelStaticData.EnemySpawners);
            SpawnPickableObjects(levelStaticData.EnvironmentObjectSpawners);
            //SpawnSaveProgressPoints(levelStaticData.SaveProgressPointSpawners);
        }

        private void FindAllMarkersInScene()
        {
            //_enemySpawnMarkers = Object.FindObjectsOfType<EnemySpawnMarker>().ToList();
            _pickableObjectMarkers = Object.FindObjectsOfType<EnvironmentObjectMarker>().ToList();
            //_saveProgressPointMarkers = Object.FindObjectsOfType<SaveProgressPointMarker>().ToList();
        }

        private void DestroyEnemyMarkers()
        {
            /*foreach (EnemySpawnMarker marker in _enemySpawnMarkers)
            {
                Object.Destroy(marker.gameObject);
            }    */        
        }

        private void DestroyPickableObjectsMarkers()
        {
            foreach (EnvironmentObjectMarker marker in _pickableObjectMarkers)
            {
                Object.Destroy(marker.gameObject);
            }
        }

        private void DestroySavePointMarkers()
        {
            /*foreach (SaveProgressPointMarker marker in _saveProgressPointMarkers)
            {
                Object.Destroy(marker.gameObject);
            }*/
        }

        /*private void SpawnEnemies(IEnumerable<EnemySpawnerData> enemySpawners)
        {
            GameObject parentObject = new GameObject(EnemiesParentObjectName);
            foreach (EnemySpawnerData spawnerData in enemySpawners)
            {
                //_gameFactory.CreateEnemySpawner(spawnerData.Id, spawnerData.EnemyTypeId, spawnerData.Position, parentObject.transform);
            }
        }*/

        private void SpawnPickableObjects(IEnumerable<EnvironmentObjectSpawnData> pickableObjectSpawners)
        {
            GameObject parentObject = new GameObject(PickableParentObjectName);
            foreach (EnvironmentObjectSpawnData spawnerData in pickableObjectSpawners)
            {
                //_gameFactory.CreatePickableObjectSpawner(spawnerData.Id, spawnerData.PickableObjectTypeId,
                //    spawnerData.Position, parentObject.transform);
            }
        }

        /*private void SpawnSaveProgressPoints(IEnumerable<SaveProgressPointSpawnData> saveProgressPointSpawners)
        {
            GameObject parentObject = new GameObject(SavePointsParentObjectName);
            foreach (SaveProgressPointSpawnData spawnerData in saveProgressPointSpawners)
            {
                //_gameFactory.CreateSaveProgressPointSpawner(spawnerData.Id, spawnerData.SaveProgressPointTypeId,
                //    spawnerData.Width, spawnerData.Height, spawnerData.Position, parentObject.transform);
            }
        }*/
    }
}