using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameObjectsScripts.Timers;
using Infrastructure.AssetManagement;
using StaticData;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PlayerScripts
{
    public class SnowmanHead : MonoBehaviour, IStack, ISavedProgress
    {
        [SerializeField] private Transform _anchorPoint;

        [SerializeField] [CustomReadOnly] private ItemType _debugType;
        private ItemType _type = ItemType.None;
        private Stuff _stuff;
        private IAssetProvider _assetProvider;
        private ISaveLoadService _saveLoadService;
        private TimersPrincipal _timersPrincipal;
        private Timer _timer;
        [Inject]
        public void Construct(IAssetProvider assetProvider, ISaveLoadService saveLoadService,
            TimersPrincipal timersPrincipal)
        {
            _assetProvider = assetProvider;
            _saveLoadService = saveLoadService;
            _timersPrincipal = timersPrincipal;
            _timer = timersPrincipal.GetTimerByType(TimerType.Carrot);
            _timer.StopCountdownTimer += OnStopCountdownCarrotTimer;
        }

        private void OnStopCountdownCarrotTimer(Timer obj)
        {
            _stuff.LastStack.UnStack();
            UnStack();
            Destroy(_stuff.gameObject);
            _saveLoadService.SaveProgress();
        }

        public void Stack(Stuff stuff)
        {
            if (stuff.Item.StuffSpecies == StuffSpecies.Decor && _type == ItemType.None)
            {
                stuff.StartPosition = stuff.Position = _anchorPoint.position;
                stuff.transform.parent = _anchorPoint;
                stuff.LastStack.UnStack();
                stuff.AddLastStack(this);
                stuff.GetComponentInChildren<Collider>().isTrigger = true;
                _stuff = stuff;
                _type = stuff.Item.Type;
                _debugType = stuff.Item.Type;
                _timer.SetReward(Extensions.DivideBy100ToFloat(stuff.Item.Price));
                _timer.Restart();
                _saveLoadService.SaveProgress();
            }
            else
            {
                stuff.Position = stuff.StartPosition;
            }
        }

        public void UnStack()
        {
            _type = ItemType.None;
            _debugType = ItemType.None;
        }


        public async void LoadProgress(PlayerProgress playerProgress)
        {
            string currentRoomName = SceneManager.GetActiveScene().name;
            RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
                x.Name == currentRoomName);

            if (roomState is null) return;

            ItemType decorType = roomState.SnowmanDecor.Type;

            if (decorType == ItemType.None) return;

            _type = decorType;
            string decorName = Enum.GetName(typeof(ItemType), (int)decorType);
            Stuff stuff = await InstantiateDecor(decorName);
            stuff.GetComponentInChildren<Collider>().isTrigger = true;
            _stuff = stuff;
            _timer.Start();
        }

        public void SaveProgress(PlayerProgress playerProgress)
        {
            string currentRoomName = SceneManager.GetActiveScene().name;
            RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
                x.Name == currentRoomName);
            if (room is not null)
            {
                room.SnowmanDecor ??= new SnowmanDecor
                {
                    Type = _type,
                };

                room.SnowmanDecor.Type = _type;
            }
            else
                playerProgress.RoomsData.Rooms.Add(new RoomState
                {
                    SnowmanDecor = new SnowmanDecor
                    {
                        Type = _type,
                    },
                    Name = currentRoomName
                });
        }

        private async UniTask<Stuff> InstantiateDecor(string decorName)
        {
            UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(decorName);

            await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
            GameObject prefab = await result;

            Vector3 position = _anchorPoint.position;
            
            Stuff stuff = Instantiate(prefab, position, Quaternion.identity,
                _anchorPoint).GetComponent<Stuff>();

            stuff.Construct(this, new PositionAdapter(position));

            _assetProvider.ReleaseAssetsByLabel(decorName);
            return stuff;
        }

        private void OnDestroy()
        {
            _timer.StopCountdownTimer -= OnStopCountdownCarrotTimer;
        }
    }
}