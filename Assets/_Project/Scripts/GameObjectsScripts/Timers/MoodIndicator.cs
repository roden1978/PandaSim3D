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
        RevertIndicatorValue(reward);
    }

    private void OnEndAnyTimer(Timer timer)
    {
        DecreaseIndicatorValue(timer);
    }

    private void RevertIndicatorValue(float reward)
    {
       _indicatorValue += reward;
        UpdateIndicatorViewValue();
        SaveProgress();
    }

    private void DecreaseIndicatorValue(Timer timer)
    {
        _indicatorValue -= timer.Decrease;
        UpdateIndicatorViewValue();
        SaveProgress();
    }

    private void UpdateIndicatorViewValue()
    {
        _indicatorValue = Clamp01();
        UpdateIndicatorValue?.Invoke(_indicatorValue);
    }

    public void ResetMoodIndicator()
    {
        _indicatorValue = 1;
        UpdateIndicatorViewValue();
    }

    private void SaveProgress() => 
        _saveLoadService.SaveProgress();

    private float Clamp01() => 
        Mathf.Clamp01(_indicatorValue);

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