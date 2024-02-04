using PlayerScripts;
using UnityEngine;

public class Carpet : MonoBehaviour, IPositionAdapter
{
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
}