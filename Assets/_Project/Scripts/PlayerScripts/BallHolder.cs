using Infrastructure.AssetManagement;
using StaticData;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class BallHolder : MonoBehaviour, IStack, ISavedProgress
    {
        [SerializeField] private Transform _anchorPoint;
        private Stuff _stuff;
        private ISaveLoadService _saveLoadService;
        private IAssetProvider _assetProvider;
        private ItemType _itemType = ItemType.None;

        [Inject]
        public void Construct(ISaveLoadService saveLoadService, IAssetProvider assetProvider)
        {
            _saveLoadService = saveLoadService;
            _assetProvider = assetProvider;
        }

        public void Stack(Stuff stuff)
        {
            /*if (stuff.Item.StuffSpecies == StuffSpecies.Toys)
            {
            }*/
            stuff.Position = stuff.StartPosition;
        }

        public void UnStack()
        {
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
        }

        public void SaveProgress(PlayerProgress playerProgress)
        {
        }
    }
}