using System;
using Common;
using Infrastructure.Factories;
using Infrastructure.Services;

namespace Infrastructure.GameStates
{
    public class UpdateProgressState : IState
    {
        private readonly GamesStateMachine _stateMachine;
        private readonly ServiceLocator _serviceLocator;
        
        public UpdateProgressState(GamesStateMachine stateMachine, ServiceLocator serviceLocator)
        {
            _stateMachine = stateMachine;
            _serviceLocator = serviceLocator;
        }
        public void Update(){}
        public void Exit(){}
        public void Enter()
        {
            UpdatePlayerProgress(HideFader);
            LoadGameMenu();
        }
        private void LoadGameMenu() => 
            _stateMachine.Enter<CreateGameMenuState>();
       
        private void HideFader() {}
        private void UpdatePlayerProgress(Action callback)
        {
            /*//IPersistentProgress persistentProgressService = _serviceLocator.Single<IPersistentProgress>();
            IGameFactory gameFactory = _serviceLocator.Single<IGameFactory>();
            
            foreach (ISavedProgressReader readers in gameFactory.ProgressReaders)
            {
                readers.LoadProgress(persistentProgressService.PlayerProgress);
            }
            callback?.Invoke();*/
        }
    }
}