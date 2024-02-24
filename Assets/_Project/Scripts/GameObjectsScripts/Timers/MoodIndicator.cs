using System;
using GameObjectsScripts.Timers;
using UI;
using UnityEngine;
using Zenject;

public class MoodIndicator : ISavedProgress, IInitializable
{
    public event Action<float> UpdateIndicatorValue;
    private readonly TimerSet _timers;
    private readonly ISaveLoadService _saveLoadService;
    private float _indicatorValue = 1;
    private Timer _gameOverTimer;
    private bool _isGameOverTimerEnabled;
    private readonly DialogManager _dialogManager;

    public MoodIndicator(TimerSet timers, ISaveLoadService saveLoadService, DialogManager dialogManager)
    {
        _timers = timers;
        _saveLoadService = saveLoadService;
        _dialogManager = dialogManager;
    }

    public void Initialize()
    {
        foreach (Timer timer in _timers)
        {
            timer.EndTimer += OnEndAnyTimer;
            timer.RestartTimer += OnRestartAnyTimer;

            if (timer.TimerType == TimerType.GameOver)
                _gameOverTimer = timer;
        }
    }

    private void OnRestartAnyTimer(float reward)
    {
        RevertIndicatorValue(reward);
    }

    private void OnEndAnyTimer(Timer timer)
    {
        DecreaseIndicatorValue(timer);
        ActivateGameOverTimer();
        GameOver(timer);
    }

    private void GameOver(Timer timer)
    {
        if (timer.TimerType == _gameOverTimer.TimerType)
        {
            Debug.Log("Game Over!");
            _dialogManager.ShowDialog<GameOverDialog>();
        }
    }

    private void ActivateGameOverTimer()
    {
        if (_indicatorValue <= 0 && _isGameOverTimerEnabled == false)
        {
            _gameOverTimer.Start();
            _isGameOverTimerEnabled = true;
        }
        else
        {
            _gameOverTimer.Stop();
            _gameOverTimer.Reset();
        }
    }

    private void RevertIndicatorValue(float reward)
    {
        _indicatorValue += reward;
        _indicatorValue = Clamp01();
        UpdateIndicatorValue?.Invoke(_indicatorValue);
        _saveLoadService.SaveProgress();
    }

    private void DecreaseIndicatorValue(Timer timer)
    {
        _indicatorValue -= timer.Decrease;
        _indicatorValue = Clamp01();
        UpdateIndicatorValue?.Invoke(_indicatorValue);
        _saveLoadService.SaveProgress();
    }

    private float Clamp01()
    {
        return Mathf.Clamp01(_indicatorValue);
    }

    public void Dispose()
    {
        foreach (Timer timer in _timers)
        {
            timer.EndTimer -= OnEndAnyTimer;
            timer.RestartTimer -= OnRestartAnyTimer;
        }
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        _indicatorValue = playerProgress.TimersData.MoodIndicatorValue;
        UpdateIndicatorValue?.Invoke(_indicatorValue);
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        playerProgress.TimersData.MoodIndicatorValue = _indicatorValue;
    }
}