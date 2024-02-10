using UnityEngine;
using UnityEngine.UI;

namespace GameObjectsScripts.Timers
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private Image _indicator;

        private Timer _timer;
        private ISaveLoadService _saveLoadService;

        public void Construct(Timer timer, Color color, ISaveLoadService saveLoadService)
        {
            _timer = timer;
            _indicator.color = color;
            _saveLoadService = saveLoadService;
            _timer.UpdateTimerView += OnUpdateTimerView;
        }
        
        private void OnDisable()
        {
            _timer.UpdateTimerView -= OnUpdateTimerView;
        }

        private void OnUpdateTimerView(float value)
        {
            _indicator.fillAmount = value;
            //TODO: Only for test and production
            //_saveLoadService.SaveProgress();
        }
    }
}