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
        private Timer _timer;

        [Inject]
        public void Construct(TimersPrincipal timersPrincipal)
        {
            _timerPrincipal = timersPrincipal;
            _timer = _timerPrincipal.GetTimerByType(TimerType.Fun);
        }

        public void Stack(Stuff stuff)
        {
            if (stuff.Item.StuffSpecies == StuffSpecies.Toys)
                Reward();

            stuff.Position = stuff.StartPosition;
        }

        public void UnStack()
        {
        }

        private void Reward()
        {
            float value = _timer.Decrease;
            float rewardValue = _timer.IndicatorValue <= 0
                ? value * value
                : value * _timer.IndicatorValue;

            _timer.SetReward(value * _timer.IndicatorValue);
            _timer.Stop();
            _timer.SetTimerState(TimerState.Revert);
            _timer.Active = true;
            Debug.Log($"Reward value {rewardValue}");
        }
    }
}