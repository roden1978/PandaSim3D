using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerScripts
{
    public class Player : MonoBehaviour, IPositionAdapter, IRotationAdapter, IPointerClickHandler
    {
        public event Action WakeUp;
        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        [SerializeField] private BoxCollider _collider;

        public void SetActiveTriggerCollider(bool value)
        {
            _collider.isTrigger = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            WakeUp?.Invoke();
        }
    }

    public enum PlayerState
    {
        None = 0,
        Sleep = 1,
        Awake = 2,
    }
}