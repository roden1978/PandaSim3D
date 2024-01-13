using UnityEngine;

namespace PlayerScripts
{
    public class SnowmanHead : MonoBehaviour, IStack
    {
        [SerializeField] private Transform _anchorPoint;
        public void Stack(Meal meal)
        {
            meal.gameObject.transform.position = _anchorPoint.position;
        }
    }
}