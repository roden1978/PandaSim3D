using System;
using System.Collections.Generic;
using System.Linq;
using GameObjectsScripts.Timers;
using Services.SaveLoad.PlayerProgress;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class TimersPrincipal : MonoBehaviour, ISavedProgress, IInitializable
{
    [SerializeField] private SoTimersSet _set;
    [SerializeField] private Transform _parent;
    [SerializeField] private MoodIndicatorView _moodIndicatorView;
    [SerializeField] private Transform _moodIndicatorParent;
    [SerializeField] [Range(1, 5)] private int _saveStateInterval;

    [Header("Debug")] [ReadOnly] [SerializeField]
    private List<string> _debugTimers;

    private readonly TimerSet _timerSet = new();
    private MoodIndicator _moodIndicator;
    private ISaveLoadStorage _saveLoadStorage;
    private ISaveLoadService _saveLoadService;
    private int _currentGameStateValue;

    [Inject]
    private void Construct(ISaveLoadStorage saveLoadStorage, ISaveLoadService saveLoadService)
    {
        _saveLoadStorage = saveLoadStorage;

        foreach (SoTimer soTimer in _set.SoCommonTimers)
        {
            Timer timer = new(soTimer);
            _timerSet.AddTimer(timer);
            _debugTimers.Add(timer.TimerType.ToString());

            if (timer.TimerType == TimerType.GameState)
                timer.UpdateGameState += OnUpdateGameState;
        }

        _moodIndicator = new MoodIndicator(_timerSet, saveLoadService);
        InstantiateMoodIndicatorView();
        _saveLoadStorage.RegisterInSaveLoadRepositories(_moodIndicator);
    }

    private void OnUpdateGameState()
    {
        if (_currentGameStateValue < _saveStateInterval)
        {
            _currentGameStateValue++;
        }
        else
        {
            _currentGameStateValue = 0;
            _saveLoadService.SaveProgress();
        }
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
    
    public void AddTimersView(string roomName)
    {
        InstantiateTimersViews(roomName);
    }

    private void InstantiateTimersViews(string roomName)
    {
        foreach (Timer timer in _timerSet)
        {
            SoTimer soTimer = _set.SoCommonTimers
                .FirstOrDefault(x => x.TimerPrefab != null && x.Type == timer.TimerType);
            
            if (soTimer is not null)
            {
                CreateTimerView(soTimer, timer);
            }
        }

        foreach (SoTimer soTimer in GetRoomTimers(Enum.Parse<RoomsType>(roomName)))
        {
            Timer timer = new(soTimer);
            _timerSet.AddTimer(timer);
            CreateTimerView(soTimer, timer);
        }
    }

    private IEnumerable<SoTimer> GetRoomTimers(RoomsType type)
    {
        RoomTimers result = _set.SoRoomTimers.FirstOrDefault(x => x.RoomType == type);
        return result?.SoTimers;
    }

    private void CreateTimerView(SoTimer soTimer, Timer timer)
    {
        GameObject prefab = Instantiate(soTimer.TimerPrefab, _parent);
        TimerView timerView = prefab.GetComponent<TimerView>();
        timerView.Construct(timer, soTimer.TimeColor);
    }

    private void InstantiateMoodIndicatorView()
    {
        MoodIndicatorView moodIndicatorView = Instantiate(_moodIndicatorView, _moodIndicatorParent);
        moodIndicatorView.Construct(_moodIndicator, _saveLoadStorage);
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        if (playerProgress.PlayerState.FirstStartGame) return;
        string roomName = CurrentSceneName();
        AddTimersView(roomName);
        UpdateTimersProgress(playerProgress);
    }

    private static string CurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private void UpdateTimersProgress(PlayerProgress playerProgress)
    {
        foreach (Timer timer in _timerSet)
        {
            TimerData timerData = playerProgress.TimersData.Timers
                .FirstOrDefault(x => x.Type == timer.TimerType);

            timer.UpdateTimerState(timerData);

            if (timer.Active)
                timer.Start();
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