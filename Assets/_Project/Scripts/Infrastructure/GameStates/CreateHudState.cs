using System;
using Common;
using Infrastructure.Factories;
using Infrastructure.Services;

namespace Infrastructure.GameStates
{
    public class CreateHudState : IState
    {
        private readonly GamesStateMachine _stateMachine;
        private readonly ServiceLocator _serviceLocator;
        public CreateHudState(GamesStateMachine stateMachine, ServiceLocator serviceLocator)
        {
            _stateMachine = stateMachine;
            _serviceLocator = serviceLocator;
        }
        public void Enter() => 
            CreateHud(OnLoaded);
        public void Update(){}
        public void Exit(){}
        private void CreateHud(Action onLoaded)
        {
            //_serviceLocator.Single<IGameFactory>().CreateHud();
            onLoaded?.Invoke();
        }
        private void OnLoaded() => 
            _stateMachine.Enter<CreateMediatorState>();
    }
}