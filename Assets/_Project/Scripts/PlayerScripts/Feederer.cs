using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerScripts
{
    public class Feederer : MonoBehaviour, IEndDragHandler, IPointerEnterHandler
    {
        public void OnEndDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Its head!");
        }
    }
}