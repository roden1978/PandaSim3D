using UnityEngine.UI;

namespace GameObjectsScripts.Timers
{
    public class AdditionalThermStrategy : IBehaviourStrategy
    {
        public void UpdateIndicatorView(Image indicator, float value)
        {
            indicator.fillAmount = 1 - value;
        }
    }
}