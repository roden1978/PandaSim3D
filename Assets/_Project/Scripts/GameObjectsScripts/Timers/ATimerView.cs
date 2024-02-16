using UnityEngine;
using UnityEngine.UI;

namespace GameObjectsScripts.Timers
{
    public abstract class ATimerView : MonoBehaviour, ITimerView
    {
        [SerializeField] protected Image _indicator;
        protected Timer Timer;

        public abstract void Construct(Timer timer, Color color);
        private void OnDisable()
        {
            Timer.UpdateTimerView -= OnUpdateTimerView;
        }
        public abstract void OnUpdateTimerView(float value);
    }
}