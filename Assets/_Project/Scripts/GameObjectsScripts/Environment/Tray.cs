using System.Linq;
using GameObjectsScripts;
using GameObjectsScripts.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Tray : MonoBehaviour, ISavedProgress, IInitializable
{
    private TimersPrincipal _timerPrincipal;
    private IShowable _poop;
    private Timer _timer;
    private ISaveLoadService _saveLoadService;
    private bool _isFull;

    [Inject]
    public void Construct(TimersPrincipal timersPrincipal, IShowable poop, ISaveLoadService saveLoadService)
    {
        _timerPrincipal = timersPrincipal;
        _poop = poop;
        _saveLoadService = saveLoadService;
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        if (playerProgress.PlayerState.FirstStartGame)
        {
            _poop.Hide();
            FillTray(false);
        }
        
        string sceneName = SceneManager.GetActiveScene().name;
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x => x.Name == sceneName);
        if (room is not null)
        {
            if (room.Poop)
            {
                _poop.Show();
                FillTray(true);
            }
            else
            {
                _poop.Hide();
                FillTray(false);
            }
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        string currentRoomName = SceneManager.GetActiveScene().name;
        RoomState room = playerProgress.RoomsData.Rooms.FirstOrDefault(x =>
            x.Name == currentRoomName);
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
        _timer = _timerPrincipal.GetTimerByType(TimerType.Poop);
        _timer.EndTimer += OnEndTimer;
        _timer.RestartTimer += OnRestartTimer;
    }

    private void OnRestartTimer()
    {
        _poop.Hide();
        FillTray(false);
        _saveLoadService.SaveProgress();
    }

    private void OnEndTimer()
    {
        _poop.Show();
        FillTray(true);
        _saveLoadService.SaveProgress();
    }

    private void OnDisable()
    {
        _timer.EndTimer -= OnEndTimer;
        _timer.RestartTimer -= OnRestartTimer;
    }

    private void FillTray(bool value)
    {
        _isFull = value;
    }
}