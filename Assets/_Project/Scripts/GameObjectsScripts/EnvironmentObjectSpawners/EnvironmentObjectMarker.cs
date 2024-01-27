using StaticData;
using UnityEngine;

public class EnvironmentObjectMarker : MonoBehaviour
{
    [field: SerializeField] public EnvironmentObjectStaticData EnvironmentObjectStaticData { get; private set; }

    public Color Color = new(255, 255, 255, 128);
    [Range(0, 1)] public float Size = 0.5f;
}