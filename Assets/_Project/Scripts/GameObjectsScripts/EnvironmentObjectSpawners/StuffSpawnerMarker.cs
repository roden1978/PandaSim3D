using StaticData;
using UnityEngine;
using UnityEngine.Serialization;

public class StuffSpawnerMarker : MonoBehaviour
{
    [SerializeField] private StuffStaticData _stuffStaticData;
    public StuffStaticData StuffStaticData => _stuffStaticData;
    
    public Color Color = new(255, 255, 255, 128);
    [Range(0, 1)] public float Size = 0.5f;

    public MarkerShapes Shape = MarkerShapes.Sphere;
}
