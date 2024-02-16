using UnityEngine;

namespace PlayerScripts
{
    public class Player : MonoBehaviour, IPositionAdapter, IRotationAdapter
    {
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
        
    }
}