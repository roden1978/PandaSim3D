using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class MealEater : MonoBehaviour, IStack
    {
        private IWalletService _wallet;

        [Inject]
        public void Construct(IWalletService wallet)
        {
            _wallet = wallet;
        }
        public void Stack(Stuff stuff)
        {
            if(stuff.Item.Meal)
            {
                Debug.Log($"The pet ate the {stuff.Item.Name}");
                _wallet.AddAmount(CurrencyType.Coins, stuff.Item.Price);
                stuff.gameObject.SetActive(false);
            }
        }
        
        public void UnStack(Stuff stuff)
        {
            
        }
    }
}