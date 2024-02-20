using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameObjectsScripts.Timers
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] protected Image _indicator;
        private Timer _timer;
        private IBehaviourStrategy _behaviourStrategy;

        public void Construct(Timer timer, Color color, IBehaviourStrategy behaviourStrategy)
        {
            _timer = timer;
            _indicator.color = color;
            _behaviourStrategy = behaviourStrategy;
            _timer.UpdateTimerView += OnUpdateTimerView;
        }
        private void OnDisable()
        {
            _timer.UpdateTimerView -= OnUpdateTimerView;
        }
        private void OnUpdateTimerView(float value)
        {
            _behaviourStrategy.UpdateIndicatorView(_indicator, value);
        }
    }
}