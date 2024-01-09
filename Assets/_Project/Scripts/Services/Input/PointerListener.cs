using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public sealed class PointerListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public event Action<PointerEventData> Down;
    public event Action<PointerEventData> Up;
    public event Action<PointerEventData> Click;
    public bool IsActive { get; set; } = true;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(IsActive)
            Down?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(IsActive)
            Up?.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(IsActive)
        {
            Debug.Log("Click");
            Click?.Invoke(eventData);
        }
    }
}
