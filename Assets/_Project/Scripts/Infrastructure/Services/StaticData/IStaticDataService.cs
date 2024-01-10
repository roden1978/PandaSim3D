using Infrastructure.Services;
using StaticData;

namespace Services.StaticData
{
    public interface IStaticDataService
    {
        EnvironmentObjectStaticData GetEnvironmentObjectStaticData(GameObjectsTypeId typeId);
        void LoadEnvironmentObjectStaticData();
        LevelStaticData GetLevelStaticData(string levelKey);
        void LoadLevelStaticData();
    }
}