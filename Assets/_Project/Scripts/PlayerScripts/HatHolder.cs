using DG.Tweening;
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
            Transform stuffTransform = stuff.gameObject.transform; 
            stuffTransform.position = _anchorPoint.position;
            stuffTransform.rotation = gameObject.transform.rotation;
            _wallet.AddAmount(CurrencyType.Coins, stuff.Item.Price);
            
            DoHatScale(stuffTransform, _scale, .5f);
        }

        private void DoHatScale(Transform hatTransform, Vector3 scale, float duration)
        {
            hatTransform.DOScale(scale, duration);
        }
        
        public void UnStack(Stuff stuff)
        {
            DoHatScale(stuff.transform, Vector3.one, .5f);
        }
    }
}