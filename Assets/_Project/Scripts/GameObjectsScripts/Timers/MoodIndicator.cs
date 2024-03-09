﻿using System;
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
    private readonly Timer _moodTimer;

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
        if (value <= 0) _indicatorValue = 0;
        _indicatorValue = value;
        UpdateIndicatorViewValue();
    }

    private void OnRestartAnyTimer(Timer timer, float reward)
    {
        if (timer.TimerType == TimerType.Carrot)
            RevertIndicatorValue(reward);
        
        if (false == timer.BasicTimer) return;

        RevertIndicatorValue(reward);

        if (_moodTimer.Active && timer.BasicTimer)
        {
            _moodTimer.Stop();
        }
    }

    private void OnStopCountdownAnyTimer(Timer timer)
    {
        DecreaseIndicatorValue(timer);
        WatchAllTimersEnd(timer);
    }

    private void WatchAllTimersEnd(Timer timer)
    {
        if (false == timer.BasicTimer) return;

        int count = _timers.Count(x => x.BasicTimer & x.Active);

        if (count <= 0 & _indicatorValue > 0)
        {
            //_moodTimer.Reset();
            _moodTimer.UpdateDuration(_indicatorValue * TimeUtils.OneMinute);
            _moodTimer.UpdateCurrentTime(_indicatorValue);
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
        
        UpdateIndicatorValue?.Invoke(_indicatorValue);

        if (_indicatorValue > 0 && moodTimerData is {Active: true})
        {
            _moodTimer.UpdateDuration(moodTimerData.IndicatorValue * TimeUtils.OneMinute);
            _moodTimer.UpdateCurrentTime(moodTimerData.IndicatorValue);
            _moodTimer.Start();
        }

    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        playerProgress.TimersData.MoodIndicatorValue = _indicatorValue;
    }
}