using UnityEngine;

namespace PlayerScripts
{
    public interface IRotationAdapter
    {
        Quaternion Rotation { get; set; }
    }
}