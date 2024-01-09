namespace Infrastructure.GameStates
{
    public class LoadGameSettings
    {
        private readonly ISaveLoadService _saveLoadService;
        private readonly IPersistentProgress _persistentProgressService;

        public LoadGameSettings(ISaveLoadService saveLoadService, IPersistentProgress persistentProgressService)
        {
            _saveLoadService = saveLoadService;
            _persistentProgressService = persistentProgressService;
        }
        
        public async void LoadSettings()
        {
            _persistentProgressService.Settings = await _saveLoadService.LoadSettings() ?? CreateSettings();
        }

        private Settings CreateSettings()
        {
            Settings settings = new();
            settings.SoundSettings.Mute = settings.StaticSoundSettings.Mute;
            settings.SoundSettings.Volume = settings.StaticSoundSettings.Volume;
                
            return settings;
        }
    }
}