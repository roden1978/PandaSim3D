using System;
using System.Linq;
using GameObjectsScripts.Timers;
using UnityEngine;
using Zenject;

public class MoodIndicator : ISavedProgress, IInitializable
{
    private const int MoodScale = 10; //10 minutes mood timer scale
    public event Action<float> UpdateIndicatorValue;
    public float MoodIndicatorValue => _indicatorValue;
    private readonly TimerSet _timers;
    private readonly ISaveLoadService _saveLoadService;
    private float _indicatorValue = 1;
    private readonly Timer _moodTimer;
    private readonly DateTime _timeOrigin;

    public MoodIndicator(TimerSet timers, ISaveLoadService saveLoadService, DateTime timeOrigin)
    {
        _timers = timers;
        _moodTimer = timers.First(x => x.TimerType == TimerType.Mood);
        _saveLoadService = saveLoadService;
        _timeOrigin = timeOrigin;
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
        //if (value <= 0) _indicatorValue = 0;
        _indicatorValue = value;
        UpdateIndicatorViewValue();
    }

    private void OnRestartAnyTimer(Timer timer, float reward)
    {
        if (timer.HasRole(TimerRoles.Rewardable))
        {
            RevertIndicatorValue(reward);

            if (_moodTimer.Active)
                _moodTimer.Stop();
        }
    }

    private void OnStopCountdownAnyTimer(Timer timer)
    {
        DecreaseIndicatorValue(timer);
        WatchAllTimersEnd();
    }

    private void WatchAllTimersEnd()
    {
        int count = _timers.Count(x => x.HasRole(TimerRoles.Basic) & x.Active);

        if (count <= 0 & _indicatorValue > 0)
        {
            _moodTimer.UpdateTimerDuration(_indicatorValue * TimeUtils.OneMinute * MoodScale);
            _moodTimer.UpdateTimerCurrentTime(_indicatorValue);
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
        //Debug.Log($"<color=red>Decrease timer {timer.TimerType.ToString()}</color>");

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
        TimerData moodTimer = playerProgress.TimersData.GetTimerDataByTimerType(TimerType.Mood);

        if (_indicatorValue > 0 && moodTimer is {Active: true})
        {
            float duration = CalculateDuration();
            float currentTime = CalculateCurrentTime(duration);
            double currentWorldTimeInSeconds = GetCurrentWorldTimeInSeconds(playerProgress);
            double delta = CalculateSecondsLastGameSave(currentWorldTimeInSeconds);
            float newCurrentTime = UpdateCurrentTime(currentTime, delta);
            _indicatorValue = newCurrentTime / duration;
            float newDuration = _indicatorValue * TimeUtils.OneMinute * MoodScale;
            _moodTimer.UpdateTimerDuration(newDuration);
            _moodTimer.UpdateTimerCurrentTime(newCurrentTime / newDuration);
            _moodTimer.Start();
        }

        UpdateIndicatorValue?.Invoke(_indicatorValue);
    }

    private float CalculateCurrentTime(float duration) =>
        _indicatorValue * duration;

    private float CalculateDuration() =>
        _indicatorValue * TimeUtils.OneMinute * MoodScale;

    private double GetCurrentWorldTimeInSeconds(PlayerProgress playerProgress) =>
        playerProgress.TimersData.CurrentWorldTimeInSeconds;

    private double CalculateSecondsLastGameSave(double seconds)
    {
        var result = DateTime.Now.Subtract(_timeOrigin).TotalSeconds - seconds;
        Debug.Log($"<color=red>Seconds count {result} </color>");
        return result;
    }

    private float UpdateCurrentTime(float currentTime, double timeDelta) =>
        currentTime - timeDelta <= 0 ? 0 : (float)(currentTime - timeDelta);

    public void SaveProgress(PlayerProgress playerProgress)
    {
        playerProgress.TimersData.MoodIndicatorValue = _indicatorValue;
    }
}