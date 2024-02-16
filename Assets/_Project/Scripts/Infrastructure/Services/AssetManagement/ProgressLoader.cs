using Services.StaticData;
using Zenject;

namespace Infrastructure.AssetManagement
{
    public class ProgressLoader : IInitializable
    {
        private readonly IStaticDataService _staticDataService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly IPersistentProgress _persistentProgress;

        public ProgressLoader(IStaticDataService staticDataService, ISaveLoadService saveLoadService,
            IPersistentProgress persistentProgress)
        {
            _staticDataService = staticDataService;
            _saveLoadService = saveLoadService;
            _persistentProgress = persistentProgress;
        }

        public void Initialize()
        {
            LoadStaticData();

            LoadSettings();
            LoadPlayerProgress();
        }

        private void LoadStaticData()
        {
            _staticDataService.LoadEnvironmentObjectStaticData();
            _staticDataService.LoadLevelStaticData();
        }

        private void LoadSettings()
        {
            _persistentProgress.Settings = _saveLoadService.LoadSettings() ?? CreateSettings();
        }

        private void LoadPlayerProgress()
        {
            _persistentProgress.PlayerProgress = _saveLoadService.LoadProgress() ?? CreatePlayerProgress();
        }

        private Settings CreateSettings()
        {
            Settings settings = new();
            settings.SoundSettings.Mute = settings.StaticSoundSettings.Mute;
            settings.SoundSettings.Volume = settings.StaticSoundSettings.Volume;

            return settings;
        }

        private PlayerProgress CreatePlayerProgress()
        {
            PlayerProgress playerProgress = new()
            {
                PlayerState =
                {
                    SceneName = AssetPaths.RoomSceneName.ToString()
                }
            };

            return playerProgress;
        }
    }
}