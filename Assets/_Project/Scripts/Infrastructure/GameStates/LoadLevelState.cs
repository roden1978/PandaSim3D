using System;
using Common;
using Cysharp.Threading.Tasks;
using Infrastructure.Factories;
using Infrastructure.Services;

namespace Infrastructure.GameStates
{
    [Obsolete]
    public class LoadLevelState : IPayloadState<string>
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly ServiceLocator _serviceLocator;
        private readonly GamesStateMachine _stateMachine;
        public LoadLevelState(GamesStateMachine stateMachine, ISceneLoader sceneLoader, ServiceLocator serviceLocator)
        {
            _sceneLoader = sceneLoader;
            _serviceLocator = serviceLocator;
            _stateMachine = stateMachine;
        }
        public void Enter(string sceneName)
        {
            _serviceLocator.Single<IGameFactory>().CleanUp();
            LoadScene(sceneName);
        }
        public void Update(){}
        public void Exit(){}
        private async void LoadScene(string sceneName)
        {
            UniTask<bool> result = _sceneLoader.LoadScene(sceneName);
            while (result.Status != UniTaskStatus.Succeeded)
                await UniTask.Yield();
            
            _stateMachine.Enter<CreatePlayerState>();
        }
    }
}