﻿using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GameObjectsScripts.Timers;
using Infrastructure;
using Infrastructure.AssetManagement;
using StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PlayerScripts
{
    public class HatHolder : MonoBehaviour, IStack, ISavedProgress
    {
        [SerializeField] private Transform _anchorPoint;
        [SerializeField] private Vector3 _scale;
        private bool _tweenComplete;
        private Stuff _stuff;
        private ISaveLoadService _saveLoadService;
        private ItemType _itemType = ItemType.None;
        private IAssetProvider _assetProvider;
        private TimersPrincipal _timerPrincipal;
        private TimerObserver _timerObserver;
        private ISceneLoader _sceneLoader;
        private string _currentRoom;

        [Inject]
        public void Construct(ISaveLoadService saveLoadService, IAssetProvider assetProvider,
            TimersPrincipal timersPrincipal, ISceneLoader sceneLoader)
        {
            _saveLoadService = saveLoadService;
            _assetProvider = assetProvider;
            _timerPrincipal = timersPrincipal;
            _sceneLoader = sceneLoader;
            _timerObserver = new TimerObserver(timersPrincipal.GetTimerByType(TimerType.Cold),
                timersPrincipal.ColdTimerView);
            _timerObserver.Initialize();
        }

        public void Stack(Stuff stuff)
        {
            if (stuff.Item.StuffSpecies == StuffSpecies.Cloths)
            {
                stuff.StartPosition = stuff.Position = _anchorPoint.position;
                stuff.Rotation = transform.rotation;
                stuff.transform.parent = _anchorPoint;
                stuff.LastStack.UnStack();
                stuff.AddLastStack(this);
                _stuff = stuff;
                _itemType = stuff.Item.Type;
                TweenerCore<Vector3, Vector3, VectorOptions> result = _stuff.transform.DOScale(_scale, .5f);
                result.onComplete = SaveHatHolderState;
                Debug.Log($"Stack to hat holder");
            }
            else
                stuff.Position = stuff.StartPosition;
        }

        private void Callback()
        {
            _itemType = ItemType.None;
            _stuff = null;
            //Destroy(_stuff);
            SaveHatHolderState();
            //_stuff.Position = _stuff.StartPosition;
            //_stuff.SetCanDrag(false);
        }

        public void UnStack()
        {
            _itemType = ItemType.None;
            _stuff = null;
            //Destroy(_stuff);
            SaveHatHolderState();
            //TweenerCore<Vector3, Vector3, VectorOptions> result = _stuff.transform.DOScale(Vector3.one, .5f);
            //result.onComplete = Callback;
        }

        private void SaveHatHolderState()
        {
            _saveLoadService.SaveProgress();
        }

        public async void LoadProgress(PlayerProgress playerProgress)
        {
            if (playerProgress.PlayerState.PlayerDecor.Type == ItemType.None)
            {
                _currentRoom = SceneManager.GetActiveScene().name;
                var isWinterRoom = _currentRoom == AssetPaths.WinterRoomSceneName;
                var notHaveHat = playerProgress.PlayerState.PlayerDecor.Type == ItemType.None;

                if (isWinterRoom)
                    _timerObserver.SetIndicatorColdColor();
                else
                    _timerObserver.SetIndicatorWarmColor();
                
                if (isWinterRoom & notHaveHat)
                    _timerObserver.TimerStart();
                else
                {
                    _timerObserver.TimerStop();
                    //_timer.Reset();
                }
                
                if(!isWinterRoom & !notHaveHat)
                    _timerObserver.TimerStart();
                else
                    _timerObserver.TimerStop();
            }
            else
            {
                _itemType = playerProgress.PlayerState.PlayerDecor.Type;
                string clothName = Enum.GetName(typeof(ItemType), (int)_itemType);
                _stuff = await InstantiateItem(clothName);
                _stuff.transform.localScale = _scale;
                _stuff.Rotation = transform.rotation;
                _stuff.StartPosition = playerProgress.PlayerState.PlayerDecor.StartPosition
                    .Vector3DataToVector3();
                _stuff.AddLastStack(this);
            }
        }

        public void SaveProgress(PlayerProgress playerProgress)
        {
            playerProgress.PlayerState.PlayerDecor = _itemType == ItemType.None
                ? new PlayerDecor(ItemType.None, Vector3.zero.Vector3ToVector3Data())
                : new PlayerDecor(_itemType, _stuff.StartPosition.Vector3ToVector3Data());
        }

        private async UniTask<Stuff> InstantiateItem(string itemName)
        {
            UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(itemName);

            await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
            GameObject prefab = await result;

            Stuff stuff = Instantiate(prefab, _anchorPoint.position, Quaternion.identity, _anchorPoint)
                .GetComponent<Stuff>();

            stuff.Construct(this);

            _assetProvider.ReleaseAssetsByLabel(itemName);
            return stuff;
        }

        private void OnDestroy()
        {
            _timerObserver.Dispose();
        }
    }
}