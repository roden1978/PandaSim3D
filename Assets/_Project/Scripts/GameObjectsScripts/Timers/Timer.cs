using System;
using UnityEngine;

namespace GameObjectsScripts.Timers
{
    public class Timer
    {
        public bool Active => _active;
        public float IndicatorValue => _indicatorValue;
        public TimerType TimerType => _type;
        public (float decrease, float increase) MoodValues => _moodValues;
        public event Action<float> UpdateTimerView;
        public event Action<Timer> EndTimer;
        public event Action<Timer> RestartTimer;
        
        private float _duration;
        private readonly (float decrease, float increase) _moodValues;
        private TimerType _type;
        private bool _active;
        private float _currentTime;
        private float _updateTime;
        private float _startTime;
        private float _endTime;
        private float _indicatorValue;
        private bool _increaseTimer;

        public Timer(float duration, Tuple<float, float> moodValues, TimerType type)
        {
            _duration = duration * TimeUtils.OneMinute;
            _currentTime = _duration;
            _moodValues = (moodValues.Item1, moodValues.Item2);
            _type = type;
        }

        public void Initialize()
        {
            _endTime = DateTime.Now.Second + _duration * TimeUtils.OneMinute;
            _startTime = DateTime.Now.Second;
        }

        public void Start()
        {
            _active = true;
        }

        public void Stop()
        {
            _active = false;
        }

        public void Tick()
        {
            if (_increaseTimer)
                IncreaseTimer();
            
            if (false == _active) return;

            if (_currentTime > 0)
            {
                _currentTime -= Time.unscaledDeltaTime;
                _updateTime += Time.unscaledDeltaTime;
            }
            else
            {
                Reset();
                Stop();
                UpdateTimerView?.Invoke(Single.Epsilon);
                EndTimer?.Invoke(this);
            }

            _indicatorValue = Convert.ToSingle(_currentTime / _duration);
            if (Convert.ToInt32(_updateTime) >= 1)
            {
                UpdateTimerView?.Invoke(_indicatorValue);
                _updateTime = 0;
            }

            
            //Debug.Log($"Update time {_updateTime} current time {_currentTime} indicator value {_indicatorValue} duration {_duration}");
        }

        private void Reset()
        {
            _currentTime = 0;
            _indicatorValue = 0;
        }

        private void Restart()
        {
            _currentTime = _duration;
            Start();
            RestartTimer?.Invoke(this);
        }

        public async void Synchronize()
        {
        }

        private void IncreaseTimer()
        {
            if (_indicatorValue < 1)
            {
                _indicatorValue += Time.unscaledDeltaTime;
                UpdateTimerView?.Invoke(_indicatorValue);
            }
            else
            {
                _indicatorValue = 1;
                _increaseTimer = false;
                Restart();
            }

            Debug.Log(
                $"Update time {_updateTime} current time {_currentTime} indicator value {_indicatorValue} duration {_duration}");
        }

        public void IncreaseSetActive(bool value)
        {
            _increaseTimer = value;
        }

        public void UpdateTimerState(TimerData timerData)
        {
            _type = timerData.Type;
            _duration = timerData.Duration;
            _startTime = timerData.StartTimerTimeInSeconds;
            _endTime = timerData.EndTimerTimeInSeconds;
            _currentTime = timerData.CurrentTime;
            _updateTime = timerData.UpdateTime;
            _indicatorValue = timerData.IndicatorValue;
            _active = timerData.Active;
            
            UpdateTimerView?.Invoke(_indicatorValue);
        }


        public TimerData SaveState()
        {
            return new TimerData()
            {
                Type = _type,
                Duration = _duration,
                StartTimerTimeInSeconds = _startTime,
                EndTimerTimeInSeconds = _endTime,
                CurrentTime = _currentTime,
                UpdateTime = _updateTime,
                IndicatorValue = _indicatorValue,
                Active = _active
            };
        }
    }
}