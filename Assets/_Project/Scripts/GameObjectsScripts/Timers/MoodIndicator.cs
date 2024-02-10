using System;
using GameObjectsScripts.Timers;
using UnityEngine;
using Zenject;

public class MoodIndicator: ISavedProgress, IInitializable
{
    public event Action<float> UpdateIndicatorValue; 
    private readonly TimerSet _timers;
    private float _indicatorValue = 1;
    private readonly ISaveLoadService _saveLoadService;

    public MoodIndicator(TimerSet timers, ISaveLoadService saveLoadService)
    {
        _timers = timers;
        _saveLoadService = saveLoadService;
    }

    public void Initialize()
    {
        foreach (Timer timer in _timers)
        {
            timer.EndTimer += OnEndAnyTimer;
            timer.RestartTimer += OnRestartAnyTimer;
        }
    }

    private void OnRestartAnyTimer(float reward)
    {
        IncreaseIndicatorValue(reward);
    }

    private void OnEndAnyTimer(Timer timer)
    {
        DecreaseIndicatorValue(timer);
    }

    public void IncreaseIndicatorValue(float reward)
    {
        _indicatorValue += reward;
        _indicatorValue = Clamp01();
        UpdateIndicatorValue?.Invoke(_indicatorValue);
        _saveLoadService.SaveProgress();
    }

    public void DecreaseIndicatorValue(Timer timer)
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