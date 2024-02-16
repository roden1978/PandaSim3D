using UnityEngine;

namespace GameObjectsScripts.Timers
{
    public class ColdTimerView : ATimerView
    {
        [SerializeField] private Color _coldColor;
        [SerializeField] private Color _warmColor;

        public override void Construct(Timer timer, Color color)
        {
            Timer = timer;
            _indicator.color = _coldColor;
            Timer.UpdateTimerView += OnUpdateTimerView;
        }

        public override void OnUpdateTimerView(float value)
        {
            _indicator.fillAmount = 1 - value;
            Debug.Log($"Fill amount timer {Timer.TimerType.ToString()} value {1 - value}");
        }

        public void SetIndicatorColdColor()
        {
            _indicator.color = _coldColor;
        }

        public void SetIndicatorWarmColor()
        {
            _indicator.color = _warmColor;
        }
    }
}