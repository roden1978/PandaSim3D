using System;
using UnityEngine;

namespace GameObjectsScripts.Timers
{
    public class Timer
    {
        public bool Active { get; set; }
        public float IndicatorValue { get; private set; }
        public float PassedTime => 1 - IndicatorValue;
        public TimerType TimerType { get; private set; }
        public bool AwakeStart { get; private set; }
        public float Decrease { get; }
        public bool BasicTimer { get; private set; }
        public float CurrentTime { get; private set; }

        public event Action<float> UpdateTimerView;
        public event Action<Timer> StopCountdownTimer;
        public event Action<Timer, float> RestartTimer;
        public event Action UpdateGameState;
        public event Action StopRevertTimer;

        private readonly ITimerRevert _timerRevert;

        private string _name;
        private float _duration;
        private float _updateTime;
        private float _startTime;
        private float _endTime;
        private float _reward;
        private float _saveStateInterval;
        private TimerState _timerState;

        public Timer(SoTimer soTimer, ITimerRevert timerRevert)
        {
            _name = soTimer.Type.ToString();
            _duration = soTimer.Duration * TimeUtils.OneMinute;
            CurrentTime = _duration;
            _timerRevert = timerRevert;
            Decrease = soTimer.MoodDecrease;
            TimerType = soTimer.Type;
            AwakeStart = soTimer.AwakeStart;
            BasicTimer = soTimer.BasicTimer;
        }

        public void Initialize()
        {
            _endTime = DateTime.Now.Second + _duration * TimeUtils.OneMinute;
            _startTime = DateTime.Now.Second;
        }

        public void Start() =>
            Active = true;

        public void Stop()
        {
            if (TimerType == TimerType.Mood)
                Debug.Log(
                    $"Stop timer {_name}");
            Active = false;
        }

        public void Tick()
        {
            if (Active)
            {
                RevertTimer();
                CountdownTimer();
            }
        }

        private void CountdownTimer()
        {
            if (_timerState == TimerState.Revert) return;

            if (_duration > 0 && CurrentTime > 0)
            {
                CurrentTime -= Time.unscaledDeltaTime;
                _updateTime += Time.unscaledDeltaTime;
            }
            else
            {
                Stop();
                UpdateTimerView?.Invoke(0);
                StopCountdownTimer?.Invoke(this);
            }

            IndicatorValue = CurrentTime / _duration;

            if (_updateTime >= .1f)
            {
                UpdateTimerView?.Invoke(IndicatorValue);
                _updateTime = 0;
            }

            if (TimerType == TimerType.Mood)
                Debug.Log(
                    $"Update timer {TimerType.ToString()} time {_updateTime} current time {CurrentTime} indicator value {IndicatorValue} duration {_duration}");
            if (TimerType == TimerType.GameOver)
                Debug.Log(
                    $"Update timer {TimerType.ToString()} time {_updateTime} current time {CurrentTime} indicator value {IndicatorValue} duration {_duration}");
        }

        public void Reset()
        {
            ResetTimerData();
            UpdateTimerView?.Invoke(IndicatorValue);
        }

        private void ResetTimerData()
        {
            CurrentTime = _duration;
            IndicatorValue = 1;
            _updateTime = 0;
        }

        public void Restart()
        {
            ResetTimerData();
            Start();
            RestartTimer?.Invoke(this, _reward);
        }

        public void RestartWithOutReset()
        {
            Start();
            RestartTimer?.Invoke(this, _reward);
        }

        public void SetReward(float value)
        {
            _reward = value;
        }

        public async void Synchronize()
        {
        }

        private void RevertTimer()
        {
            if (_timerState == TimerState.Countdown) return;

            IndicatorValue += _timerRevert.GetValue();

            if (IndicatorValue < 1)
            {
                UpdateTimerView?.Invoke(IndicatorValue);
            }
            else
            {
                StopRevertTimer?.Invoke();
                Restart();
                SetTimerState(TimerState.Countdown);
            }

            /*if (TimerType == TimerType.Mood)
                Debug.Log(
                    $"Update timer {TimerType.ToString()} time {_updateTime} current time {CurrentTime} indicator value {IndicatorValue} duration {_duration}");*/
        }

        public void RestoreCurrentTime()
        {
            CurrentTime = IndicatorValue * _duration;
        }

        public void SetTimerState(TimerState value)
        {
            _timerState = value;
        }

        public void UpdateTimerState(TimerData timerData)
        {
            _name = timerData.Name;
            TimerType = timerData.Type;
            _startTime = timerData.StartTimerTimeInSeconds;
            _endTime = timerData.EndTimerTimeInSeconds;
            CurrentTime = timerData.CurrentTime;
            _updateTime = timerData.UpdateTime;
            IndicatorValue = timerData.IndicatorValue;
            Active = timerData.Active;
            AwakeStart = timerData.AwakeStart;
            _timerState = timerData.State;
            BasicTimer = timerData.BasicTimer;
            UpdateTimerView?.Invoke(IndicatorValue);
        }

        public void UpdateDuration(float value)
        {
            _duration = value;
        }
        public void UpdateCurrentTime(float indicatorValue)
        {
            CurrentTime = indicatorValue * _duration;
        }

        public TimerData SaveState()
        {
            return new TimerData()
            {
                Name = _name,
                Type = TimerType,
                StartTimerTimeInSeconds = _startTime,
                EndTimerTimeInSeconds = _endTime,
                CurrentTime = CurrentTime,
                UpdateTime = _updateTime,
                IndicatorValue = IndicatorValue,
                Active = Active,
                AwakeStart = AwakeStart,
                State = _timerState,
                BasicTimer = BasicTimer,
            };
        }
    }

    public interface ITimerRevert
    {
        float GetValue();
    }

    public class SimpleRevert : ITimerRevert
    {
        public float GetValue()
        {
            return Time.unscaledDeltaTime;
        }
    }

    public class DurationRevert : ITimerRevert
    {
        private readonly float _duration;

        public DurationRevert(float duration = 1)
        {
            _duration = duration;
        }

        public float GetValue()
        {
            return Time.unscaledDeltaTime / (_duration * TimeUtils.OneMinute);
        }
    }

    public enum TimerState
    {
        None = 0,
        Countdown = 1,
        Revert = 2,
    }
}