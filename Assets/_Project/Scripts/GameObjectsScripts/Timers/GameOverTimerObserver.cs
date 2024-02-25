using System;
using GameObjectsScripts.Timers;

public class GameOverTimerObserver
{
    public event Action EndGameOverTimer;
    private readonly Timer _timer;

    public GameOverTimerObserver(Timer timer)
    {
        _timer = timer;
    }

    public void Initialize()
    {
        _timer.EndTimer += OnEndTimer;
    }

    private void OnEndTimer(Timer obj)
    {
        EndGameOverTimer?.Invoke();
    }

    public void Dispose()
    {
        _timer.EndTimer -= OnEndTimer;
    }

    public void StartGameOverTimer()
    {
        _timer.Start();
    }

    public void StopGameOverTimer()
    {
        _timer.Stop();
        _timer.Reset();
    }

    public void ResetGameOverTimer()
    {
        _timer.Reset();
    }
}