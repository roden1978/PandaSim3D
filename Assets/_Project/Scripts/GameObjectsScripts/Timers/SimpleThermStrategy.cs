using UnityEngine.UI;

namespace GameObjectsScripts.Timers
{
    public class SimpleThermStrategy : IBehaviourStrategy
    {
        public void UpdateIndicatorView(Image indicator, float value)
        {
            indicator.fillAmount = value;
        }
    }
}