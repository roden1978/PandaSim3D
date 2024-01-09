using System;
using System.Collections.Generic;
using Common;
using Infrastructure.Services;
using Zenject;

namespace Infrastructure
{
    public class GamesStateMachine : IGamesStateMachine, IInitializable
    {
        private Dictionary<Type, IUpdateableState> _states;
        private IUpdateableState _activeState;
        private ISceneLoader _sceneLoader;
        private readonly ServiceLocator _serviceLocator;

        public GamesStateMachine(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _serviceLocator = ServiceLocator.Container;
        }

        public void Initialize()
        {
            _states = new Dictionary<Type, IUpdateableState>
            {
                //InitializeServicesState - заменяется ServicesInstaller 
                //[typeof(InitializeServicesState)] = new InitializeServicesState(this, _serviceLocator),
                
                //Следующий этап загрузка настроек и прогресса игрока //DataLoader
                /*[typeof(LoadGameSettingsState)] = new LoadGameSettingsState(this, serviceLocator),
                [typeof(InitializeInputState)] = new InitializeInputState(this, serviceLocator),
                [typeof(LoadProgressState)] = new LoadProgressState(this, serviceLocator),
                
                //Загрузка игровой сцены //SceneLoader
                [typeof(LoadLevelState)] = new LoadLevelState(this, sceneLoader, serviceLocator),
                
                //Загрузка и инстацирование игровых объектов //GameObjectInstantiater
                [typeof(CreatePlayerState)] = new CreatePlayerState(this, serviceLocator),
                //[typeof(CreateCrowbarState)] = new CreateCrowbarState(this, serviceLocator, game),
                [typeof(SpawnEntitiesState)] = new SpawnEntitiesState(this, serviceLocator),
                
                //Загрузка и создание HUD //UIInstantiater
                [typeof(CreateHudState)] = new CreateHudState(this, serviceLocator),
                [typeof(CreateMediatorState)] = new CreateMediatorState(this, serviceLocator),
                
                //Обновление прогресса игрока //ProgressUpdater
                [typeof(UpdateProgressState)] = new UpdateProgressState(this,serviceLocator),
                
                [typeof(CreateGameMenuState)] = new CreateGameMenuState(serviceLocator)*/
            };
        }

        public void Enter<TState>() where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IUpdateableState
        {
            _activeState?.Exit();
            TState state = State<TState>();
            _activeState = state;
            return state;
        }

        private TState State<TState>() where TState : class, IUpdateableState
        {
            return _states[typeof(TState)] as TState;
        }
    }
}