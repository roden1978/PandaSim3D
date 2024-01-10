using System.Collections.Generic;
using System.Linq;
using Infrastructure.AssetManagement;
using StaticData;

namespace Services.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private readonly IAssetProvider _assetProvider;
        
        private Dictionary<GameObjectsTypeId, EnvironmentObjectStaticData> _environmentObjectsStaticData;
        private Dictionary<string, LevelStaticData> _scenesStaticData;
        public StaticDataService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async void LoadEnvironmentObjectStaticData()
        {
            IList<EnvironmentObjectStaticData> result =
                await _assetProvider.LoadAllAsync<EnvironmentObjectStaticData>(AssetPaths.EnvironmentStaticDataLabel);
             _environmentObjectsStaticData = result.ToDictionary(x => x.GameObjectsTypeId, x => x);
        }

        public async void LoadLevelStaticData()
        {
            IList<LevelStaticData> result =
                await _assetProvider.LoadAllAsync<LevelStaticData>(AssetPaths.SceneStaticDataLabel);
             _scenesStaticData = result.ToDictionary(x => x.LevelKey, x => x);
        }

      public EnvironmentObjectStaticData GetEnvironmentObjectStaticData(GameObjectsTypeId typeId) =>
            _environmentObjectsStaticData.TryGetValue(typeId, out EnvironmentObjectStaticData pickableObjectStaticData)
                ? pickableObjectStaticData
                : null;

        public LevelStaticData GetLevelStaticData(string levelKey)
        {
            return _scenesStaticData.TryGetValue(levelKey, out LevelStaticData levelStaticData)
                ? levelStaticData
                : null;
        }
        
    }
}