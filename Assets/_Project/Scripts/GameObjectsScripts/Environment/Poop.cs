using GameObjectsScripts;
using GameObjectsScripts.Timers;
using PlayerScripts;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class Poop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IShowable, IPositionAdapter, IInitializable
{
    [SerializeField] [Range(0, .2f)] private float _cleanReward;
    private TimersPrincipal _timersPrincipal;
    private Timer _timer;

    [Inject]
    public void Construct(TimersPrincipal timersPrincipal)
    {
        _timersPrincipal = timersPrincipal;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_timer.IndicatorValue < 1)
            _timer.SetTimerState(TimerState.Countdown);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer down");
        _timer.SetTimerState(TimerState.Revert);
        _timer.SetReward(_cleanReward);
        _timer.Active = true;
    }

    public void Initialize()
    {
        _timer ??= _timersPrincipal.GetTimerByType(TimerType.Poop);
    }
}