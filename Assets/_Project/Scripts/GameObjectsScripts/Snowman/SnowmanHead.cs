using System;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PlayerScripts
{
    public class SnowmanHead : MonoBehaviour, IStack, ISavedProgress
    {
        [SerializeField] private Transform _anchorPoint;
        private ItemType _type = ItemType.None;
        private IAssetProvider _assetProvider;

        [Inject]
        public void Construct(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }
        public void Stack(Stuff stuff)
        {
            stuff.gameObject.transform.position = _anchorPoint.position;
            _type = stuff.Item.Type;
        }
        
        public void UnStack(Stuff stuff)
        {
            _type = ItemType.None;
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
                
                    if(melaType.Equals(ItemType.None)) return;
                    _type = melaType;
                    string mealName = Enum.GetName(typeof(ItemType), (int)melaType);
                    await InstantiateMeal(mealName);
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
            
            private async Task InstantiateMeal(string mealName)
            {
                UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(mealName);
        
                await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
                GameObject prefab = await result;
        
                Stuff stuff = Instantiate(prefab, _anchorPoint.position, Quaternion.identity,
                        _anchorPoint)
                    .GetComponent<Stuff>();
                
                stuff.Construct(this);
        
                _assetProvider.ReleaseAssetsByLabel(mealName);
            }
    }
}