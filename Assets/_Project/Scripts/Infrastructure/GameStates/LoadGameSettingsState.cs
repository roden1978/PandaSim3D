using System;
using Common;
using Infrastructure.Services;
//using Services.PersistentProgress;

namespace Infrastructure.GameStates
{
    [Obsolete]
    public class LoadGameSettingsState : IState
    {
        private readonly GamesStateMachine _gamesStateMachine;
        private readonly ServiceLocator _serviceLocator;
        //private IPersistentProgressService _persistentProgressService;
        public LoadGameSettingsState(GamesStateMachine gamesStateMachine, ServiceLocator serviceLocator)
        {
            _gamesStateMachine = gamesStateMachine;
            _serviceLocator = serviceLocator;
        }

        public void Enter() => 
            LoadSettings(NextState);
        
        private void NextState() => 
            _gamesStateMachine.Enter<InitializeInputState>();

        private async void LoadSettings(Action callback)
        {
            /*ISaveLoadService saveLoadService = _serviceLocator.Single<ISaveLoadService>();
            _persistentProgressService = _serviceLocator.Single<IPersistentProgressService>();
            _persistentProgressService.Settings = await saveLoadService.LoadSettings() ?? CreateSettings();
            callback?.Invoke();*/
        }

        private Settings CreateSettings()
        {
            Settings settings = new();
            settings.SoundSettings.Mute = settings.StaticSoundSettings.Mute;
            settings.SoundSettings.Volume = settings.StaticSoundSettings.Volume;
                
            return settings;
        }

        public void Update(){}

        public void Exit(){}
    }
}