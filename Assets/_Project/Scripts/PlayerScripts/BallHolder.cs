using GameObjectsScripts.Timers;
using StaticData;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class BallHolder : MonoBehaviour, IStack
    {
        private Stuff _stuff;
        private ItemType _itemType = ItemType.None;
        private Timer _timer;

        [Inject]
        public void Construct(TimersPrincipal timersPrincipal)
        {
            _timer = timersPrincipal.GetTimerByType(TimerType.Fun);
        }

        public void Stack(Stuff stuff)
        {
            if (stuff.Item.StuffSpecies == StuffSpecies.Toys)
            {
                Reward(stuff);
            }

            stuff.Position = stuff.StartPosition;
        }

        public void UnStack()
        {
        }

        private void Reward(Stuff stuff)
        {
            float value = Extensions.DivideBy100ToFloat(stuff.Item.Price);
            float rewardValue = _timer.IndicatorValue <= 0
                ? value * .1f
                : value * Extensions.DivideBy100ToFloat(_timer.PassedTime);
            _timer.SetReward(rewardValue);
            _timer.Stop();
            _timer.IncreaseSetActive(true);

            Debug.Log($"Reward value {rewardValue}");
        }
    }
}