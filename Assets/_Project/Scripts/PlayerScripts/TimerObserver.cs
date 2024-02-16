using GameObjectsScripts.Timers;
using Infrastructure;
using Infrastructure.AssetManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace PlayerScripts
{
    public class TimerObserver : ISavedProgress
    {
        private Timer _timer;
        private readonly ColdTimerView _view;

        public TimerObserver(Timer timer, ColdTimerView view)
        {
            _timer = timer;
            _view = view;
        }

        public void Initialize()
        {
            _timer.EndTimer += OnEndTimer;
        }

        public void TimerStart()
        {
            _timer.Start();
        }

        public void TimerStop()
        {
            _timer.Stop();
        }

        public void Dispose()
        {
            _timer.EndTimer -= OnEndTimer;
        }

        private void OnEndTimer(Timer obj)
        {
            _timer.Stop();
            //_timer.Reset();
            //_sceneLoader.LoadScene(AssetPaths.RoomSceneName);
        }
        
        public void SetIndicatorColdColor()
        {
            _view.SetIndicatorColdColor();
        }

        public void SetIndicatorWarmColor()
        {
            _view.SetIndicatorWarmColor();
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
            
        }

        public void SaveProgress(PlayerProgress playerProgress)
        {
        }
    }
}