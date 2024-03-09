using GameObjectsScripts.Timers;
using UnityEngine;

public class SaveGameTimerObserver
{
    private readonly Timer _timer;
    private readonly ISaveLoadService _saveLoadService;

    public SaveGameTimerObserver(Timer timer, ISaveLoadService saveLoadService)
    {
        _timer = timer;
        _saveLoadService = saveLoadService;
    }

    public void Initialize()
    {
        _timer.StopCountdownTimer += OnStopTimer;
    }

    public void Dispose()
    {
        _timer.StopCountdownTimer -= OnStopTimer;
    }

    private void OnStopTimer(Timer timer)
    {
        timer.Restart();
        _saveLoadService.SaveProgress();
        Debug.Log($"<color=blue>Save game timer was stopped</color>");
    }
}