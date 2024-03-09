using System;
using System.Linq;
using GameObjectsScripts.Timers;
using Infrastructure.AssetManagement;
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
        RoomState room =
            playerProgress.RoomsData.Rooms.FirstOrDefault(x => x.Name == AssetPaths.RoomSceneName.ToString());

        if (room is not null)
        {
            FillTray(room.Poop);
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == AssetPaths.RoomSceneName.ToString());
        if (room is not null)
            room.Poop = _isFull;
        else
            playerProgress.RoomsData.Rooms.Add(new RoomState
            {
                Poop = _isFull,
                Name = AssetPaths.RoomSceneName.ToString()
            });
    }

    public void Initialize()
    {
        _timer = _timersPrincipal.GetTimerByType(TimerType.Poop);
        _timer.StopCountdownTimer += OnStopCountdownTimer;
        _timer.RestartTimer += OnRestartTimer;
    }

    private void OnRestartTimer(Timer timer, float reward)
    {
        FillTray(false);
        _saveLoadService.SaveProgress();
    }

    private void OnStopCountdownTimer(Timer timer)
    {
        FillTray(true);
        _saveLoadService.SaveProgress();
    }

    public void Dispose()
    {
        _timer.StopCountdownTimer -= OnStopCountdownTimer;
        _timer.RestartTimer -= OnRestartTimer;
    }

    private void FillTray(bool value)
    {
        if (value)
            ShowPoop?.Invoke();
        else
            HidePoop?.Invoke();

        _isFull = value;
    }
}