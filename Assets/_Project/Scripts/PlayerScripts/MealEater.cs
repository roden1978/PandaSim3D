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
                stuff.gameObject.transform.position = stuff.StartPosition;
                stuff.gameObject.SetActive(false);
                
                Reward(stuff);
            }
        }

        private void Reward(Stuff stuff)
        {
            float value = Convert.ToSingle(stuff.Item.Price) / 100;
            Debug.Log($"Reward value {value}");
            _timer.SetReward(value);
            _timer.Restart();
        }

        public void UnStack(Stuff stuff)
        {
            
        }

        public void Initialize()
        {
           
        }
    }
}