using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        private IAssetProvider _assetProvider;
        private ISaveLoadService _saveLoadService;
        private TimersPrincipal _timersPrincipal;

        [Inject]
        public void Construct(IAssetProvider assetProvider, ISaveLoadService saveLoadService, TimersPrincipal timersPrincipal)
        {
            _assetProvider = assetProvider;
            _saveLoadService = saveLoadService;
            _timersPrincipal = timersPrincipal;
        }

        public void Stack(Stuff stuff)
        {
            if (stuff.Item.StuffSpecies == StuffSpecies.Decor && _type == ItemType.None)
            {
                stuff.StartPosition = stuff.Position = _anchorPoint.position;
                stuff.transform.parent = _anchorPoint;
                stuff.LastStack.UnStack();
                stuff.AddLastStack(this);
                _type = stuff.Item.Type;
                _debugType = stuff.Item.Type;
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
            Debug.Log($"Unstack carrot");
        }


        //TODO: Implement spawn decor after state loading
        public async void LoadProgress(PlayerProgress playerProgress)
        {
            //TODO: Implement snowman data load;
            string currentRoomName = SceneManager.GetActiveScene().name;
            RoomState roomState = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
                x.Name == currentRoomName);

            if (roomState is not null)
            {
                ItemType melaType = roomState.SnowmanDecor.Type;

                if (melaType.Equals(ItemType.None)) return;
                _type = melaType;
                string mealName = Enum.GetName(typeof(ItemType), (int)melaType);
                await InstantiateDecor(mealName);
            }
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

        private async Task InstantiateDecor(string decorName)
        {
            UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(decorName);

            await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
            GameObject prefab = await result;

            Stuff stuff = Instantiate(prefab, _anchorPoint.position, Quaternion.identity,
                    _anchorPoint).GetComponent<Stuff>();

            stuff.Construct(this);

            _assetProvider.ReleaseAssetsByLabel(decorName);
        }
    }
}