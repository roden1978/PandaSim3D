﻿using System.Collections.Generic;
using System.Linq;
using GameObjectsScripts.Timers;
using Services.SaveLoad.PlayerProgress;
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
    private ISaveLoadStorage _saveLoadStorage;
    private ISaveLoadService _saveLoadService;

    public MoodIndicator MoodIndicator => _moodIndicator;


    [Inject]
    private void Construct(ISaveLoadStorage saveLoadStorage, ISaveLoadService saveLoadService)
    {
        _saveLoadStorage = saveLoadStorage;

        foreach (SoTimer soTimer in _set.SoTimers)
        {
            Timer timer = new(soTimer);
            _timerSet.AddTimer(timer);
            _debugTimers.Add(timer.TimerType.ToString());
        }

        _moodIndicator = new MoodIndicator(_timerSet, saveLoadService);
        InstantiateMoodIndicatorView();
        _saveLoadStorage.RegisterInSaveLoadRepositories(_moodIndicator);
    }

    public void Initialize()
    {
        _moodIndicator.Initialize();
    }

    public void StartTimers()
    {
        foreach (Timer timer in _timerSet)
        {
            if (timer.CanStart)
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
        InstantiateTimersViews();
    }

    private void InstantiateTimersViews()
    {
        foreach (Timer timer in _timerSet)
        {
            SoTimer soTimer = _set.SoTimers
                .FirstOrDefault(x => x.TimerPrefab != null && x.Type == timer.TimerType);

            if (soTimer is not null)
            {
                GameObject prefab = Instantiate(soTimer.TimerPrefab, _parent);
                TimerView timerView = prefab.GetComponent<TimerView>();
                timerView.Construct(timer, soTimer.TimeColor, _saveLoadService);
            }
        }
    }

    private void InstantiateMoodIndicatorView()
    {
        MoodIndicatorView moodIndicatorView = Instantiate(_moodIndicatorView, _moodIndicatorParent);
        moodIndicatorView.Construct(_moodIndicator, _saveLoadStorage);
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
            TimerData timerData = playerProgress.TimersData.Timers
                .FirstOrDefault(x => x.Type == timer.TimerType);

            //if (timerData is null) continue;

            timer.UpdateTimerState(timerData);

            if (timer.Active)
                timer.Start();
            /*else
                timer.Stop();*/
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