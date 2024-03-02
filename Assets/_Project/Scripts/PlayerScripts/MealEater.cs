using System;
using GameObjectsScripts.Timers;
using StaticData;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class MealEater : MonoBehaviour, IStack, IInitializable
    {
        private IWalletService _wallet;
        private TimersPrincipal _timerPrincipal;
        private Timer _timer;

        [Inject]
        public void Construct(IWalletService wallet, TimersPrincipal timersPrincipal)
        {
            _wallet = wallet;
            _timerPrincipal = timersPrincipal;
            _timer = _timerPrincipal.GetTimerByType(TimerType.Meal);
        }
        public void Stack(Stuff stuff)
        {
            EatMeal(stuff);
        }

        private void EatMeal(Stuff stuff)
        {
            if (stuff.Item.StuffSpecies == StuffSpecies.Meal)
            {
                Debug.Log($"The pet ate the {stuff.Item.Name}");
                _wallet.AddAmount(CurrencyType.Coins, Mathf.FloorToInt(Convert.ToSingle(stuff.Item.Price) / 2));
                stuff.Position = stuff.StartPosition;
                stuff.LastStack.UnStack();
                //stuff.gameObject.SetActive(false);
                Reward(stuff);
                Destroy(stuff.gameObject);
            }
            else
            {
                stuff.Position = stuff.StartPosition;
            }
        }

        private void Reward(Stuff stuff)
        {
            float value = Extensions.DivideBy100ToFloat(stuff.Item.Price);
            float rewardValue = value * _timer.PassedTime;
            _timer.SetReward(rewardValue);
            _timer.Stop();
            _timer.SetTimerState(TimerState.Revert);
            _timer.Active = true;
            Debug.Log($"Reward value {rewardValue}");
        }

        public void UnStack()
        {
            
        }

        public void Initialize()
        {
           
        }
    }
}