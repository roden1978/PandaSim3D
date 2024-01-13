using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public interface IStack
    {
        void Stack(Meal meal);
    }
    public class Head : MonoBehaviour, IStack
    {
        private IWalletService _wallet;

        [Inject]
        public void Construct(IWalletService wallet)
        {
            _wallet = wallet;
        }
        public void Stack(Meal meal)
        {
            Debug.Log($"The pet ate the {meal.Item.Name}");
            _wallet.AddAmount(CurrencyType.Coins, meal.Item.Price);
            meal.gameObject.SetActive(false);
        }
    }
}