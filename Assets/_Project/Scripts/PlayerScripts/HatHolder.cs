using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using StaticData;
using UnityEngine;
using Zenject;

namespace PlayerScripts
{
    public class HatHolder : MonoBehaviour, IStack
    {
        [SerializeField] private Transform _anchorPoint;
        [SerializeField] private Vector3 _scale;
        private IWalletService _wallet;
        private IInventory _inventory;
        private bool _tweenComplete;
        private Stuff _stuff;

        [Inject]
        public void Construct(IWalletService wallet, IInventory inventory)
        {
            _wallet = wallet;
            _inventory = inventory;
        }

        public void Stack(Stuff stuff)
        {
            if (stuff.Item.StuffSpecies == StuffSpecies.Cloths)
            {
                Transform stuffTransform = stuff.gameObject.transform;
                stuffTransform.position = _anchorPoint.position;
                stuffTransform.rotation = gameObject.transform.rotation;
                _wallet.AddAmount(CurrencyType.Coins, stuff.Item.Price);
                _stuff = stuff;
                stuffTransform.DOScale(_scale, .5f);
            }
        }

        /*private TweenCallback DoHatScale(Transform hatTransform, Vector3 scale, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> result = hatTransform.DOScale(scale, duration);
            return result.onComplete = Callback;
        }*/

        private void Callback()
        {
            _inventory.TryAddItem(this, _stuff.Item, Extensions.OneItem);
            _stuff.gameObject.transform.position = _stuff.StartPosition;
            _stuff.gameObject.SetActive(false);
            _stuff = null;
        }

        public void UnStack(Stuff stuff)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> result = stuff.transform.DOScale(Vector3.one, .5f);
            result.onComplete = Callback;
        }
    }
}