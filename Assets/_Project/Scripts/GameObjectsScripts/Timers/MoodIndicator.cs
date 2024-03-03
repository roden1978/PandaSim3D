using System;
using System.Linq;
using GameObjectsScripts.Timers;
using UnityEngine;
using Zenject;

public class MoodIndicator : ISavedProgress, IInitializable
{
    public event Action<float> UpdateIndicatorValue;
    public float MoodIndicatorValue => _indicatorValue;
    private readonly TimerSet _timers;
    private readonly ISaveLoadService _saveLoadService;
    private float _indicatorValue = 1;
    private Timer _moodTimer;

    public MoodIndicator(TimerSet timers, ISaveLoadService saveLoadService)
    {
        _timers = timers;
        _moodTimer = timers.First(x => x.TimerType == TimerType.Mood);
        _saveLoadService = saveLoadService;
    }

    public void Initialize()
    {
        foreach (Timer timer in _timers)
        {
            timer.StopCountdownTimer += OnStopCountdownAnyTimer;
            timer.RestartTimer += OnRestartAnyTimer;
        }

        _moodTimer.UpdateTimerView += OnUpdateMoodTimer;
    }

    private void OnUpdateMoodTimer(float value)
    {
        _indicatorValue = value;
        UpdateIndicatorViewValue();
    }

    private void OnRestartAnyTimer(float reward)
    {
        RevertIndicatorValue(reward);
        
        if(_moodTimer.Active)
        {
            _moodTimer.Stop();
            _moodTimer.Reset();
        }
    }

    private void OnStopCountdownAnyTimer(Timer timer)
    {
        DecreaseIndicatorValue(timer);
        WatchAllTimersEnd();
    }

    private void WatchAllTimersEnd()
    {
        int count = _timers.Count(x => x.BasicTimer & x.Active);
        
        if(count <= 0 & _indicatorValue > 0)
        {
            _moodTimer.UpdateDuration(_indicatorValue * TimeUtils.OneMinute);
            _moodTimer.Start();
            _saveLoadService.SaveProgress();
        }
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
            timer.StopCountdownTimer -= OnStopCountdownAnyTimer;
            timer.RestartTimer -= OnRestartAnyTimer;
        }
        _moodTimer.UpdateTimerView -= OnUpdateMoodTimer;
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        _indicatorValue = playerProgress.TimersData.MoodIndicatorValue;
        TimerData moodTimerData = playerProgress.TimersData.GetTimerDataByTimerType(TimerType.Mood);
        
        if (moodTimerData is { IndicatorValue: < 1 })
        {
            _moodTimer.UpdateDuration(moodTimerData.IndicatorValue * TimeUtils.OneMinute);
            _moodTimer.Start();
        }
        UpdateIndicatorValue?.Invoke(_indicatorValue);
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        playerProgress.TimersData.MoodIndicatorValue = _indicatorValue;
    }
}