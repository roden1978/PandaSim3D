using DG.Tweening;
using GameObjectsScripts.Timers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Carpet : MonoBehaviour, IPositionAdapter, IPointerClickHandler
{
    [SerializeField] private Transform _playerSleepPoint;
    private IPositionAdapter _playerPosition;
    private Transform _playerTransform;
    private Timer _timer;
    private TimersPrincipal _timersPrincipal;
    private Vector3 _playerStayPoint;
    private Player _player;
    private float _gotoSleepTimerPassedTime;
    private ISaveLoadService _saveLoadService;

    [Inject]
    public void Construct(Player player, TimersPrincipal timersPrincipal, ISaveLoadService saveLoadService)
    {
        _saveLoadService = saveLoadService;
        _playerPosition = player;
        _player = player;
        _playerTransform = player.transform;
        _playerStayPoint = _playerPosition.Position;
        _timersPrincipal = timersPrincipal;
        _timer = timersPrincipal.GetTimerByType(TimerType.Sleep);
        _timer.StopRevertTimer += OnStopRevertTimer;
        _player.ChangePlayerState += OnChangePlayerState;
    }

    private void OnStopRevertTimer()
    {
        SetSleepTimerReward();
        ReturnToStayPoint();
    }

    private void OnChangePlayerState(State state)
    {
        SetSleepTimerReward();
        ReturnToStayPoint();
    }

    private float CalculateReward() =>
        .25f * (_gotoSleepTimerPassedTime - _timer.PassedTime);

    private void SetSleepTimerReward() =>
        _timer.SetReward(CalculateReward());

    public void OnPointerClick(PointerEventData eventData)
    {
        GotoSleep();
    }

    private void GotoSleep()
    {
        _player.SetActiveColliders(false);
        _gotoSleepTimerPassedTime = _timer.PassedTime;
        _playerTransform.DOMove(_playerSleepPoint.position, .5f).onComplete = () =>
        {
            _timersPrincipal.SetActiveTimersBySleepPlayerState(false);
            _player.SetState(State.Sleep);
            _timer.SetTimerState(TimerState.Revert);
            _timer.Active = true;
            _saveLoadService.SaveProgress();
        };
    }

    private void ReturnToStayPoint()
    {
        _player.SetActiveColliders(true);
        
        _playerTransform.DOMove(_playerStayPoint, .5f).onComplete = () =>
        {
            _timersPrincipal.SetActiveTimersBySleepPlayerState(true);
            _player.SetState(State.Awake);
            _timer.SetTimerState(TimerState.Countdown);
            _timer.RestoreCurrentTime();
            _timer.RestartWithOutReset();
            _timer.Active = true;
            _saveLoadService.SaveProgress();
        };
    }

    private void OnDisable()
    {
        _player.ChangePlayerState -= OnChangePlayerState;
        _timer.StopRevertTimer -= OnStopRevertTimer;
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }
}