using PlayerScripts;
using UnityEngine;

public class Tray : MonoBehaviour, IPositionAdapter, ISavedProgress
{
    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        
    }

    public void SaveProgress(PlayerProgress persistentPlayerProgress)
    {
        
    }
}