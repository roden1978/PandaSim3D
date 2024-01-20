using UnityEngine;

namespace PlayerScripts
{
    public class SnowmanHead : MonoBehaviour, IStack
    {
        [SerializeField] private Transform _anchorPoint;
        public void Stack(Stuff stuff)
        {
            stuff.gameObject.transform.position = _anchorPoint.position;
        }
        
        public void UnStack(Stuff stuff)
        {
            
        }
    }
}