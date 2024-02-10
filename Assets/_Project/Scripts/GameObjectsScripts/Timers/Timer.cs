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
        public bool CanStart => _canStart;

        public event Action<float> UpdateTimerView;
        public event Action<Timer> EndTimer;
        public event Action<float> RestartTimer;

        private readonly float _decrease;
        private float _duration;
        private TimerType _type;
        private bool _active;
        private float _currentTime;
        private float _updateTime;
        private float _startTime;
        private float _endTime;
        private float _indicatorValue;
        private bool _increaseTimer;
        private float _reward;
        private bool _canStart;

        public Timer(SoTimer soTimer)
        {
            _duration = soTimer.Duration * TimeUtils.OneMinute;
            _currentTime = _duration;
            _decrease = soTimer.MoodDecrease;
            _type = soTimer.Type;
            _canStart = soTimer.CanStart;
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


            if (_type == TimerType.Carrot)
                Debug.Log(
                    $"Update timer {_type.ToString()} time {_updateTime} current time {_currentTime} indicator value {_indicatorValue} duration {_duration}");
        }

        private void Reset()
        {
            _currentTime = 0;
            _indicatorValue = 0;
            _updateTime = 0;
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

        private void IncreaseTimer()
        {
            if (_indicatorValue < 1)
            {
                _indicatorValue += Time.unscaledDeltaTime;
                UpdateTimerView?.Invoke(_indicatorValue);
            }
            else
            {
                Restart();
                IncreaseSetActive(false);
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
            _canStart = timerData.CanStart;

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
                Active = _active,
                CanStart = _canStart
            };
        }
    }
}