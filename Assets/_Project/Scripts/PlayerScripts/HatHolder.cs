using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class HatHolder : MonoBehaviour, IStack
    {
        [SerializeField] private Transform _anchorPoint;
        [SerializeField] private Vector3 _scale;
        private IWalletService _wallet;

        [Inject]
        public void Construct(IWalletService wallet)
        {
            _wallet = wallet;
        }
        
        public void Stack(Stuff stuff)
        {
            var stuffTransform = stuff.gameObject.transform; 
            stuffTransform.position = _anchorPoint.position;
            stuffTransform.rotation = gameObject.transform.rotation;
            _wallet.AddAmount(CurrencyType.Coins, stuff.Item.Price);
        }
    }
}