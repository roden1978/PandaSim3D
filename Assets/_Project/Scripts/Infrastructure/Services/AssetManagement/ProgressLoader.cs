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

        private async void LoadSettings()
        {
            _persistentProgress.Settings = await _saveLoadService.LoadSettings() ?? CreateSettings();
        }

        private async void LoadPlayerProgress()
        {
            _persistentProgress.PlayerProgress = await _saveLoadService.LoadProgress() ?? CreatePlayerProgress();
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
            PlayerProgress playerProgress = new();

            playerProgress.PlayerState.SceneName = AssetPaths.RoomSceneName;
            playerProgress.PlayerState.CurrentHealth = playerProgress.StaticPlayerData.Health;
            playerProgress.PlayerState.MaxHealth = playerProgress.StaticPlayerData.MaxHealth;
            playerProgress.PlayerState.Dream = playerProgress.StaticPlayerData.Dream;
            playerProgress.PlayerState.Meal = playerProgress.StaticPlayerData.Meal;
            playerProgress.PlayerState.Mood = playerProgress.StaticPlayerData.Mood;
            playerProgress.PlayerState.Toilet = playerProgress.StaticPlayerData.Toilet;

            return playerProgress;
        }
    }
}