using System;
using GameObjectsScripts.Timers;
using UnityEngine;

public class GameOverTimerObserver : ISavedProgress
{
    public event Action EndGameOverTimer;
    private readonly Timer _timer;
    private readonly MoodIndicator _moodIndicator;
    private readonly DateTime _timeOrigin;

    public GameOverTimerObserver(Timer timer, MoodIndicator moodIndicator, DateTime timeOrigin)
    {
        _timer = timer;
        _moodIndicator = moodIndicator;
        _timeOrigin = timeOrigin;
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
        Debug.Log($"<color=red>Game over timer was started</color>");
        _timer.Start();
    }

    public void StopGameOverTimer()
    {
        Debug.Log($"<color=blue>Game over timer was stopped</color>");
        _timer.Stop();
        _timer.Reset();
    }

    public void ResetGameOverTimer()
    {
        _timer.Reset();
    }

    private double CalculateSecondsLastGameSave(double seconds)
    {
        var result = DateTime.Now.Subtract(_timeOrigin).TotalSeconds - seconds;
        Debug.Log($"<color=red>Seconds count {result} </color>");
        return result;
    }

    private float UpdateCurrentTime(float currentTime, double timeDelta) =>
        currentTime - timeDelta <= 0 ? 0 : (float)(currentTime - timeDelta);

    private double GetCurrentWorldTimeInSeconds(PlayerProgress playerProgress) =>
        playerProgress.TimersData.CurrentWorldTimeInSeconds;

    public void LoadProgress(PlayerProgress playerProgress)
    {
        if (playerProgress.PlayerState.FirstStartGame) return;

        TimerData gameOverTimer = playerProgress.TimersData.GetTimerDataByTimerType(TimerType.GameOver);

        if (_moodIndicator.MoodIndicatorValue <= 0)
        {
            double currentWorldTimeInSeconds = GetCurrentWorldTimeInSeconds(playerProgress);
            double delta = CalculateSecondsLastGameSave(currentWorldTimeInSeconds);
            float newCurrentTime = UpdateCurrentTime(gameOverTimer.CurrentTime, delta);
            _timer.UpdateTimerCurrentTime(newCurrentTime);
            _timer.Start();
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
    }
}