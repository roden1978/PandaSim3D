using System;
using GameObjectsScripts.Timers;
using Zenject;

public class MoodIndicator: ISavedProgress, IInitializable
{
    public event Action<float> UpdateIndicatorValue; 
    private readonly TimerSet _timers;
    private float _indicatorValue = 1;
    public MoodIndicator(TimerSet timers)
    {
        _timers = timers;
    }

    public void Initialize()
    {
        foreach (Timer timer in _timers)
        {
            timer.EndTimer += OnEndAnyTimer;
            timer.RestartTimer += OnRestartAnyTimer;
        }
    }

    private void OnRestartAnyTimer(Timer timer)
    {
        _indicatorValue += timer.MoodValues.increase;
        UpdateIndicatorValue?.Invoke(_indicatorValue);
    }

    private void OnEndAnyTimer(Timer timer)
    {
        _indicatorValue -= timer.MoodValues.decrease;
        UpdateIndicatorValue?.Invoke(_indicatorValue);
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
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
    }
}