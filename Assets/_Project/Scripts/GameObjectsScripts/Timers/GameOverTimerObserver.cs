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
        _timer.StopCountdownTimer += OnStopCountdownTimer;
    }

    private void OnStopCountdownTimer(Timer obj)
    {
        EndGameOverTimer?.Invoke();
    }

    public void Dispose()
    {
        _timer.StopCountdownTimer -= OnStopCountdownTimer;
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