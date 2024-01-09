using Common;
using Infrastructure.Factories;
using Infrastructure.Services;

namespace Infrastructure.GameStates
{
    public class LoadControlsPanelState : IState
    {
        private readonly ServiceLocator _serviceLocator;
        public LoadControlsPanelState(ServiceLocator serviceLocator) => 
            _serviceLocator = serviceLocator;
        public void Update(){}
        public void Exit(){}
        public void Enter() {}
    }
}