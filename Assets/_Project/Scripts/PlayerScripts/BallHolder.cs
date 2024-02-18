using GameObjectsScripts.Timers;
using StaticData;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class BallHolder : MonoBehaviour, IStack
    {
        private Stuff _stuff;
        private TimersPrincipal _timerPrincipal;

        [Inject]
        public void Construct(TimersPrincipal timersPrincipal)
        {
            _timerPrincipal = timersPrincipal;
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
            Timer timer = _timerPrincipal.GetTimerByType(TimerType.Fun);
            
            float value = Extensions.DivideBy100ToFloat(stuff.Item.Price);
            float rewardValue = timer.IndicatorValue <= 0
                ? value * .1f
                : value * Extensions.DivideBy100ToFloat(timer.PassedTime);
            timer.SetReward(rewardValue);
            timer.Stop();
            timer.IncreaseSetActive(true);

            Debug.Log($"Reward value {rewardValue}");
        }
    }
}