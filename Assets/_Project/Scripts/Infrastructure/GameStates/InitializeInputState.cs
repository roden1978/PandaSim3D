using System;
using Common;
using Infrastructure.Services;
using Services.Input;
using UnityEngine;

namespace Infrastructure.GameStates
{
    public class InitializeInputState : IState
    {
        private readonly GamesStateMachine _stateMachine;
        private readonly ServiceLocator _serviceLocator;
        public InitializeInputState(GamesStateMachine stateMachine, ServiceLocator serviceLocator)
        {
            _stateMachine = stateMachine;
            _serviceLocator = serviceLocator;
        }
        public void Enter() => 
            RegisterInputService(NextState);
        public void Update(){}
        public void Exit(){}
        private void RegisterInputService(Action callback = null)
        {
            _serviceLocator.RegisterSingle(InputService());
            callback?.Invoke();
        }
        private IInputService InputService()
        {
            if (Application.isEditor)
               return new KeyboardInputService();

            return new SwipeService();
        }
        private void NextState()
        {
            //_stateMachine.Enter<LoadProgressState>();
        }
    }
}