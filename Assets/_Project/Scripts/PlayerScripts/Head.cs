using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class Head : MonoBehaviour
    {
        private IWalletService _wallet;

        [Inject]
        public void Construct(IWalletService wallet)
        {
            _wallet = wallet;
        }
        public void Feed(Meal meal)
        {
            Debug.Log($"The pet ate the {meal.Item.Name}");
            _wallet.AddAmount(CurrencyType.Coins, meal.Item.Price);
        }
    }
}