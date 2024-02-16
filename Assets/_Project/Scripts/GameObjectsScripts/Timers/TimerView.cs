using UnityEngine;

namespace GameObjectsScripts.Timers
{
    public class TimerView : ATimerView
    {
        public override void Construct(Timer timer, Color color)
        {
            Timer = timer;
            _indicator.color = color;
            Timer.UpdateTimerView += OnUpdateTimerView;
        }
        public override void OnUpdateTimerView(float value)
        {
            _indicator.fillAmount = value;
        }
    }
}