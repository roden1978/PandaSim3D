using System;
using Common;
using Infrastructure.Factories;
using Infrastructure.Services;
using PlayerScripts;

namespace Infrastructure.GameStates
{
    public class CreatePlayerState : IState
    {
        private readonly GamesStateMachine _stateMachine;
        private readonly ServiceLocator _serviceLocator;
        public CreatePlayerState(GamesStateMachine stateMachine, ServiceLocator serviceLocator)
        {
            _stateMachine = stateMachine;
            _serviceLocator = serviceLocator;
        }
        private void CreatePlayer(Action onLoaded)
        {
            //PoolService pool = _serviceLocator.Single<IGameFactory>().CreatePool();
            //Crosshair crosshair = _serviceLocator.Single<IGameFactory>().CreateCrosshair();
            Player player = _serviceLocator.Single<IGameFactory>().CreatePlayer();

            
            onLoaded?.Invoke();
        }
        public void Enter() => 
            CreatePlayer(OnLoaded);
        public void Update(){}
        public void Exit(){}
        private void OnLoaded() => 
            _stateMachine.Enter<CreateCrowbarState>();
    }
}