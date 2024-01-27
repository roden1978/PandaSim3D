using Services.SaveLoad.PlayerProgress;
using UnityEngine;
using Zenject;

public class ProgressUpdater : MonoBehaviour// ,IInitializable
{
    private const float Delay = 3;
    private ISaveLoadStorage _saveLoadStorage;
    private IPersistentProgress _persistentProgress;

    [Inject]
    public void Construct(ISaveLoadStorage saveLoadStorage, IPersistentProgress persistentProgress)
    {
        _saveLoadStorage = saveLoadStorage;
        _persistentProgress = persistentProgress;
    }

    private void Start()//public void Initialize()
    {
        foreach (ISavedProgress progressWriter in _saveLoadStorage.Savers)
        {
            Debug.Log($"Update object {progressWriter}");
            progressWriter.LoadProgress(_persistentProgress.PlayerProgress);
        }
        
        Destroy(this, Delay);
    }
}