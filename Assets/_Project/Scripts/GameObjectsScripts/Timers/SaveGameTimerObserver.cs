using GameObjectsScripts.Timers;

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
        _saveLoadService.SaveProgress();
        timer.Restart();
    }
}