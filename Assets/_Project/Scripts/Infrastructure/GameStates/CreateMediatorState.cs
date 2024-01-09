using System;
using Common;
using Infrastructure.Factories;
using Infrastructure.Services;
using PlayerScripts;
using UI;

namespace Infrastructure.GameStates
{
    public class CreateMediatorState : IState
    {
        private readonly GamesStateMachine _stateMachine;
        private readonly ServiceLocator _serviceLocator;
        public CreateMediatorState(GamesStateMachine stateMachine, ServiceLocator serviceLocator)
        {
            _stateMachine = stateMachine;
            _serviceLocator = serviceLocator;
        }
        public void Enter() => 
            CreateMediator(OnLoaded);
        private void OnLoaded() => 
            _stateMachine.Enter<SpawnEntitiesState>();
        public void Update(){}
        public void Exit(){}
        private void CreateMediator(Action onLoaded)
        {
            IGameFactory gameFactory = _serviceLocator.Single<IGameFactory>();
            Mediator mediator = gameFactory.CreateMediator();
            Player player = gameFactory.Player;
            InteractableObjectsCollector interactableObjectsCollector = player.GetComponent<InteractableObjectsCollector>();
            //Hud hud = gameFactory.Hud;
            //mediator.Construct(interactableObjectsCollector, hud, player);
            onLoaded?.Invoke();
        }
    }
}