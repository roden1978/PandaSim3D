using GameObjectsScripts.Timers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Carpet : MonoBehaviour, IPositionAdapter, IPointerClickHandler, IStack
{
    private IPositionAdapter _playerPosition;
    private Timer _timer;
    private TimersPrincipal _timersPrincipal;

    [Inject]
    public void Construct(Player player, TimersPrincipal timersPrincipal)
    {
        _playerPosition = player;
        _timersPrincipal = timersPrincipal;
        _timer = timersPrincipal.GetTimerByType(TimerType.Sleep);
        _timer.EndTimer += OnEndTimer;
    }

    private void OnEndTimer(Timer obj)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _timer.RevertSetActive(true);
    }

    public void Stack(Stuff stuff)
    {
        
    }

    public void UnStack()
    {
        
    }

    private void OnDisable()
    {
        _timer.EndTimer -= OnEndTimer;
    }

    public Vector3 Position { get => transform.position; set => transform.position = value; }
}