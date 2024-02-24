using System;
using System.Collections.Generic;
using System.Linq;
using GameObjectsScripts.Timers;
using Services.SaveLoad.PlayerProgress;
using TriInspector;
using UI;
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
    private List<TimerDebugInfo> _debugTimers;

    private readonly TimerSet _timerSet = new();
    private readonly Dictionary<TimerType, TimerView> _timerViewSet = new();
    private MoodIndicator _moodIndicator;
    private ISaveLoadStorage _saveLoadStorage;
    private ISaveLoadService _saveLoadService;
    private int _currentGameStateValue;
    private DialogManager _dialogManager;

    [Inject]
    private void Construct(ISaveLoadStorage saveLoadStorage, ISaveLoadService saveLoadService,
        DialogManager dialogManager)
    {
        _saveLoadStorage = saveLoadStorage;
        _dialogManager = dialogManager;

        foreach (SoTimer soTimer in _set.SoCommonTimers)
        {
            Timer timer = soTimer.Type == TimerType.Sleep
                ? new Timer(soTimer, new DurationRevert(soTimer.Duration))
                : new Timer(soTimer, new SimpleRevert());

            _timerSet.AddTimer(timer);
            _debugTimers.Add(new TimerDebugInfo
                {
                    Name = timer.TimerType.ToString(),
                    Duration = soTimer.Duration
                }
            );
        }

        _moodIndicator = new MoodIndicator(_timerSet, saveLoadService, _dialogManager);
        InstantiateMoodIndicatorView();
        _saveLoadStorage.RegisterInSaveLoadRepositories(_moodIndicator);
    }

    public void Initialize() =>
        _moodIndicator.Initialize();

    public void StartTimers()
    {
        foreach (Timer timer in _timerSet.Where(x => x.AwakeStart))
            timer.Start();
    }

    private void Update()
    {
        foreach (Timer timer in _timerSet)
            timer.Tick();
    }

    public Timer GetTimerByType(TimerType type) =>
        _timerSet.FirstOrDefault(x => x.TimerType == type);

    public void AddTimersView(string roomName) =>
        InstantiateTimersViews(roomName);

    private void InstantiateTimersViews(string roomName)
    {
        InstantiateBasicTimers();
        InstantiateRoomAdditionalTimers(roomName);
    }

    private void InstantiateRoomAdditionalTimers(string roomName)
    {
        foreach (SoTimer soTimer in GetRoomTimers(Enum.Parse<RoomsType>(roomName)))
        {
            Timer timer = GetTimerByType(soTimer.Type);

            if (timer is null)
            {
                timer = new Timer(soTimer, new SimpleRevert());
                _timerSet.AddTimer(timer);
                _debugTimers.Add(new TimerDebugInfo { Name = timer.TimerType.ToString(), Duration = soTimer.Duration });
            }
            else
            {
                timer.UpdateDuration(soTimer.Duration);
                _debugTimers.FirstOrDefault(x => x.Name == timer.TimerType.ToString())!.Duration = soTimer.Duration;
            }

            CreateTimerView(soTimer, timer);
        }
    }

    private void InstantiateBasicTimers()
    {
        foreach (Timer timer in _timerSet)
        {
            SoTimer soTimer = _set.SoCommonTimers
                .FirstOrDefault(x => x.TimerPrefab != null && x.Type == timer.TimerType);

            if (soTimer is not null)
                CreateTimerView(soTimer, timer);
        }
    }

    private TimerDebugInfo GetDebugTimerInfoByTimerType(TimerType type)
    {
        return _debugTimers.Find(x => x.Name == type.ToString());
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

        if (soTimer.Type == TimerType.Thermo)
            timerView.Construct(timer, soTimer.TimeColor, new AdditionalThermStrategy());
        else
            timerView.Construct(timer, soTimer.TimeColor, new SimpleThermStrategy());

        _timerViewSet.Add(soTimer.Type, timerView);
    }

    public bool TryGetTimerViewByTimerType(TimerType type, out TimerView timerView)
    {
        return _timerViewSet.TryGetValue(type, out timerView);
    }

    public void ReStartTimerByType(TimerType type) =>
        GetTimerByType(type).Restart();

    public void StopTimerByType(TimerType type) =>
        GetTimerByType(type).Stop();

    public void ResetTimerByType(TimerType type) =>
        GetTimerByType(type).Reset();

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

            if (timerData is not null)
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

[Serializable]
public class TimerDebugInfo
{
    public string Name;
    public float Duration;
}