using System.Collections.Generic;
using System.Linq;
using GameObjectsScripts.Timers;
using TriInspector;
using UnityEngine;
using Zenject;

public class TimersPrincipal : MonoBehaviour, ISavedProgress, IInitializable
{
    [SerializeField] private SoTimersSet _set;
    [SerializeField] private Transform _parent;
    [SerializeField] private MoodIndicatorView _moodIndicatorView;
    [SerializeField] private Transform _moodIndicatorParent;

    [Header("Debug")] [ReadOnly] [SerializeField]
    private List<string> _debugTimers;
    
    private readonly TimerSet _timerSet = new();
    private MoodIndicator _moodIndicator;


    [Inject]
    private void Construct() //private void InitializeTimers()
    {
        _moodIndicator = new MoodIndicator();
        
        foreach (SoTimer soTimer in _set.SoTimers)
        {
            Timer timer = new(soTimer.Duration, soTimer.Type);
            _timerSet.AddTimer(timer);
            _debugTimers.Add(timer.TimerType.ToString());
        }
    }


    public void Initialize()
    {
        //InitializeTimers();
    }

    public void StartTimers()
    {
        foreach (Timer timer in _timerSet)
        {
            //Watch this for cold timer!!!!!
            if(timer.TimerType == TimerType.Cold) continue;
            
            timer.Start();
        }
    }

    private void Update()
    {
        foreach (Timer timer in _timerSet)
        {
            timer.Tick();
        }
    }

    public Timer GetTimerByType(TimerType type)
    {
        return _timerSet.FirstOrDefault(x => x.TimerType == type);
    }

    public void AddTimersView()
    {
        MoodIndicatorView moodIndicatorView = Instantiate(_moodIndicatorView, _moodIndicatorParent);
        moodIndicatorView.Construct(_moodIndicator);
        
        foreach (Timer timer in _timerSet)
        {
            SoTimer soTimer = _set.SoTimers.FirstOrDefault(x => x.Type == timer.TimerType);

            if (soTimer is not null)
            {
                GameObject prefab = Instantiate(soTimer.TimerPrefab, _parent);
                TimerView timerView = prefab.GetComponent<TimerView>();
                timerView.Construct(timer, soTimer.TimeColor);
            }
        }
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        if (playerProgress.PlayerState.FirstStartGame) return;
        AddTimersView();
        UpdateTimersProgress(playerProgress);
    }

    private void UpdateTimersProgress(PlayerProgress playerProgress)
    {
        foreach (Timer timer in _timerSet)
        {
            TimerData timerData = playerProgress.TimersData.Timers.FirstOrDefault(x => x.Type == timer.TimerType);

            if (timerData is null) continue;

            timer.UpdateTimerState(timerData);

            if (timer.Active)
                timer.Start();
            else
                timer.Stop();
        }
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        playerProgress.TimersData.Timers.Clear();
        foreach (Timer timer in _timerSet)
        {
            playerProgress.TimersData.Timers.Add(timer.SaveState());
        }
    }
}