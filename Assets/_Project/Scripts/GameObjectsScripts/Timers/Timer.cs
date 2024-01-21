using System;
using UnityEngine;

namespace GameObjectsScripts.Timers
{
    public class Timer
    {
        public bool Active => _active;
        public float IndicatorValue => _indicatorValue;

        public event Action<float> UpdateTimerView;

        private readonly int _duration;
        private TimerType _type;
        private bool _active;
        private float _currentTime;
        private float _updateTime;
        private int _startTime;
        private int _endTime;
        private float _indicatorValue;

        public Timer(int duration, TimerType type)
        {
            _duration = duration * TimeUtils.OneMinute;
            _currentTime = _duration;
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
        }

        public void Restart()
        {
            _currentTime = _duration;
            Start();
        }

        public async void Synchronize()
        {
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