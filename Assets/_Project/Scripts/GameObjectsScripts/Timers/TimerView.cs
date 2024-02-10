using UnityEngine;
using UnityEngine.UI;

namespace GameObjectsScripts.Timers
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private Image _indicator;

        private Timer _timer;

        public void Construct(Timer timer, Color color)
        {
            _timer = timer;
            _indicator.color = color;
            _timer.UpdateTimerView += OnUpdateTimerView;
        }
        
        private void OnDisable()
        {
            _timer.UpdateTimerView -= OnUpdateTimerView;
        }

        private void OnUpdateTimerView(float value)
        {
            _indicator.fillAmount = value;
           
        }
    }
}