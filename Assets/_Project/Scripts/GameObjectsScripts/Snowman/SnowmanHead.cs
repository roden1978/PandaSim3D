using UnityEngine;

namespace PlayerScripts
{
    public class SnowmanHead : MonoBehaviour, IStack, ISavedProgress
    {
        [SerializeField] private Transform _anchorPoint;
        public ItemType DecorType;
        private ItemType _type = ItemType.None;
        public void Stack(Stuff stuff)
        {
            stuff.gameObject.transform.position = _anchorPoint.position;
            _type = stuff.Item.Type;
        }
        
        public void UnStack(Stuff stuff)
        {
            _type = ItemType.None;
        }
        
        //TODO: Implement spawn decor after state loading
        public void LoadProgress(PlayerProgress playerProgress)
        {
            throw new System.NotImplementedException();
        }

        public void SaveProgress(PlayerProgress persistentPlayerProgress)
        {
            throw new System.NotImplementedException();
        }
    }
}