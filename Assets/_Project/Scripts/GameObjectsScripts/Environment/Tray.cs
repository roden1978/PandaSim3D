using System;
using System.Linq;
using GameObjectsScripts.Timers;
using UnityEngine.SceneManagement;
using Zenject;

public class Tray : ISavedProgress, IInitializable
{
    public event Action ShowPoop;
    public event Action HidePoop;
    private readonly TimersPrincipal _timersPrincipal;
    private readonly ISaveLoadService _saveLoadService;
    private Timer _timer;
    private bool _isFull;

    public Tray(TimersPrincipal timersPrincipal, ISaveLoadService saveLoadService)
    {
        _timersPrincipal = timersPrincipal;
        _saveLoadService = saveLoadService;
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        if (playerProgress.PlayerState.FirstStartGame)
        {
            HidePoop?.Invoke();
            FillTray(false);
        }
        
        string sceneName = SceneManager.GetActiveScene().name;
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x => x.Name == sceneName);
        if (room is not null)
        {
            if (room.Poop)
            {
                ShowPoop?.Invoke();
                FillTray(true);
            }
            else
            {
                HidePoop?.Invoke();
                FillTray(false);
            }
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == "Room");
        if (room is not null)
            room.Poop = _isFull;
        else
            playerProgress.RoomsData.Rooms.Add(new RoomState
            {
                Poop = _isFull
            });
    }

    public void Initialize()
    {
        _timer = _timersPrincipal.GetTimerByType(TimerType.Poop);
        _timer.EndTimer += OnEndTimer;
        _timer.RestartTimer += OnRestartTimer;
    }

    private void OnRestartTimer()
    {
        HidePoop?.Invoke();
        FillTray(false);
        _saveLoadService.SaveProgress();
    }

    private void OnEndTimer()
    {
        ShowPoop?.Invoke();
        FillTray(true);
        _saveLoadService.SaveProgress();
    }

    public void Dispose()
    {
        _timer.EndTimer -= OnEndTimer;
        _timer.RestartTimer -= OnRestartTimer;
    }

    private void FillTray(bool value)
    {
        _isFull = value;
    }
}