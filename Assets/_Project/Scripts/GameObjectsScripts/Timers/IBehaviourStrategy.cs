using UnityEngine.UI;

namespace GameObjectsScripts.Timers
{
    public interface IBehaviourStrategy
    {
        void UpdateIndicatorView(Image indicator, float value);
    }
}