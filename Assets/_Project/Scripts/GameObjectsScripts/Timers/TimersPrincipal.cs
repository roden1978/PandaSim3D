using System.Collections;
using System.Collections.Generic;
using GameObjectsScripts.Timers;
using UnityEngine;
using Zenject;

public class TimersPrincipal : MonoBehaviour, IInitializable, ISavedProgress
{
    [SerializeField] private SoTimersSet _set;
    [SerializeField] private Transform _parent;

    private readonly TimerSet _timerSet = new();
    private Timer _moodTimer;

    private void SpawnTimers()
    {
        foreach (SoTimer soTimer in _set.SoTimers)
        {
            Timer timer = new(soTimer.Duration, soTimer.Type);
            GameObject prefab = Instantiate(soTimer.TimerPrefab, _parent);
            TimerView timerView = prefab.GetComponent<TimerView>();
            timerView.Construct(timer, soTimer.TimeColor);
            if (soTimer.Type == TimerType.Mood)
            {
                _moodTimer = timer;
                continue;
            }
            _timerSet.AddTimer(timer);
        }
    }

    public void Initialize()
    {
        SpawnTimers();
        StartTimers();
    }

    private void StartTimers()
    {
        foreach (Timer timer in _timerSet)
        {
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

    public void LoadProgress(PlayerProgress playerProgress)
    {
        //Implement load timers state
    }

    public void SaveProgress(PlayerProgress persistentPlayerProgress)
    {
        persistentPlayerProgress.TimersData.Timers.Clear();
        foreach (Timer timer in _timerSet)
        {
            persistentPlayerProgress.TimersData.Timers.Add(timer.SaveState());
        }
    }
}

public class TimerSet : IEnumerable<Timer>
{
    private readonly List<Timer> _timers = new();

    public void AddTimer(Timer timer)
    {
        _timers.Add(timer);
    }

    public IEnumerator<Timer> GetEnumerator()
    {
        return _timers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}