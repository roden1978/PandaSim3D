using System;
using UnityEngine;

namespace GameObjectsScripts.Timers
{
    public class Timer
    {
        public bool Active => _active;
        public float IndicatorValue => _indicatorValue;
        public float PassedTime => 1 - _indicatorValue;
        public TimerType TimerType => _type;
        public float Decrease => _decrease;
        public bool AwakeStart => _awakeStart;
        public bool BasicTimer => _basicTimer;
        public float CurrentTime => _currentTime;

        public event Action<float> UpdateTimerView;
        public event Action<Timer> StopCountdownTimer;
        public event Action<float> RestartTimer;
        public event Action UpdateGameState;
        public event Action StopRevertTimer;

        private readonly ITimerRevert _timerRevert;
        private readonly float _decrease;
        private float _duration;
        private TimerType _type;
        private bool _active;
        private float _currentTime;
        private float _updateTime;
        private float _startTime;
        private float _endTime;
        private float _indicatorValue;
        private bool _revertTimer;
        private float _reward;
        private bool _awakeStart;
        private float _saveStateInterval;
        private TimerState _timerState;
        private bool _basicTimer;

        public Timer(SoTimer soTimer, ITimerRevert timerRevert)
        {
            _duration = soTimer.Duration * TimeUtils.OneMinute;
            _currentTime = _duration;
            _decrease = soTimer.MoodDecrease;
            _type = soTimer.Type;
            _awakeStart = soTimer.AwakeStart;
            _timerRevert = timerRevert;
            _basicTimer = soTimer.BasicTimer;
        }

        public void Initialize()
        {
            _endTime = DateTime.Now.Second + _duration * TimeUtils.OneMinute;
            _startTime = DateTime.Now.Second;
        }

        public void Start() =>
            _active = true;

        public void Stop() =>
            _active = false;

        public void Tick()
        {
            RevertTimer();
            CountdownTimer();
        }

        private void CountdownTimer()
        {
            if (false == _active) return;

            if (_duration > 0 && _currentTime > 0)
            {
                _currentTime -= Time.unscaledDeltaTime;
                _updateTime += Time.unscaledDeltaTime;
            }
            else
            {
                Stop();
                UpdateTimerView?.Invoke(Single.Epsilon);
                StopCountdownTimer?.Invoke(this);
            }

            _indicatorValue = _currentTime / _duration;

            if (_updateTime >= .1f)
            {
                UpdateTimerView?.Invoke(_indicatorValue);
                _updateTime = 0;
            }
            
            if (_type == TimerType.GameOver)
                Debug.Log(
                    $"Update timer {_type.ToString()} time {_updateTime} current time {_currentTime} indicator value {_indicatorValue} duration {_duration}");
        }

        public void Reset()
        {
            _currentTime = _duration;
            _indicatorValue = 1;
            _updateTime = 0;

            UpdateTimerView?.Invoke(_indicatorValue);
        }

        public void Restart()
        {
            _currentTime = _duration;
            _indicatorValue = 1;
            _updateTime = 0;
            Start();
            RestartTimer?.Invoke(_reward);
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
            if (false == _revertTimer) return;

            _indicatorValue += _timerRevert.GetValue();

            if (_indicatorValue < 1)
            {
                UpdateTimerView?.Invoke(_indicatorValue);
            }
            else
            {
                StopRevertTimer?.Invoke();
                Restart();
                RevertSetActive(false);
            }

            /*Debug.Log(
                $"Update time {_updateTime} current time {_currentTime} indicator value {_indicatorValue} duration {_duration}");*/
        }

        public void RevertSetActive(bool value)
        {
            _revertTimer = value;
            _timerState = value ? TimerState.Revert : TimerState.Countdown;
        }

        public void UpdateTimerState(TimerData timerData)
        {
            _type = timerData.Type;
            _startTime = timerData.StartTimerTimeInSeconds;
            _endTime = timerData.EndTimerTimeInSeconds;
            _currentTime = timerData.CurrentTime;
            _updateTime = timerData.UpdateTime;
            _indicatorValue = timerData.IndicatorValue;
            _active = timerData.Active;
            _awakeStart = timerData.AwakeStart;

            UpdateTimerView?.Invoke(_indicatorValue);
        }

        public void UpdateDuration(float value)
        {
            _duration = value;
        }


        public TimerData SaveState()
        {
            return new TimerData()
            {
                Type = _type,
                StartTimerTimeInSeconds = _startTime,
                EndTimerTimeInSeconds = _endTime,
                CurrentTime = _currentTime,
                UpdateTime = _updateTime,
                IndicatorValue = _indicatorValue,
                Active = _active,
                AwakeStart = _awakeStart,
                State = _timerState,
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