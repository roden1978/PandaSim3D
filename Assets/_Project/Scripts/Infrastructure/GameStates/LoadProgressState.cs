using System;
using Common;
using Infrastructure.Services;
//using Services.PersistentProgress;

namespace Infrastructure.GameStates
{
    [Obsolete]
    public class LoadProgressState : IState
    {
        private const string SceneName = "City";
        private readonly GamesStateMachine _gamesStateMachine;
        private readonly ServiceLocator _serviceLocator;
        //private IPersistentProgressService _persistentProgressService;
        public LoadProgressState(GamesStateMachine gamesStateMachine, ServiceLocator serviceLocator)
        {
            _gamesStateMachine = gamesStateMachine;
            _serviceLocator = serviceLocator;
        }
        public void Enter() => 
            LoadProgress(NextState);
        private void NextState()
        {
            //string levelName = _persistentProgressService.PlayerProgress.WorldData.PositionOnLevel.SceneName;
            //_gamesStateMachine.Enter<LoadLevelState, string>(levelName);
        }
        private async void LoadProgress(Action callback)
        {
            /*ISaveLoadService saveLoadService = _serviceLocator.Single<ISaveLoadService>();
            _persistentProgressService = _serviceLocator.Single<IPersistentProgressService>();
            
            _persistentProgressService.PlayerProgress = await saveLoadService.LoadProgress() ?? CreatePlayerProgress();
            
            callback?.Invoke();*/
        }
        private PlayerProgress CreatePlayerProgress()
        {
            /*PlayerProgress playerProgress = new PlayerProgress(SceneName);

            playerProgress.PlayerState.CurrentHealth = playerProgress.StaticPlayerData.Health;
            playerProgress.PlayerState.CurrentCrystalsAmount = playerProgress.StaticPlayerData.CrystalsAmount;
            playerProgress.PlayerState.CurrentLivesAmount = playerProgress.StaticPlayerData.StartLivesAmount;
            playerProgress.PlayerState.CurrentFruitScoresAmount = playerProgress.StaticPlayerData.FruitScoresAmount;
            playerProgress.PlayerState.MaxHealth = playerProgress.StaticPlayerData.MaxHealth;
            playerProgress.PlayerState.MaxBonusLivesCount = playerProgress.StaticPlayerData.MaxBonusLivesCount;
            playerProgress.PlayerState.StartLivesAmount = playerProgress.StaticPlayerData.StartLivesAmount;*/
           
            //return playerProgress;
            return null;
        }
        public void Update(){}
        public void Exit(){}
    }
}