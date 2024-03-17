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

    private DateTime _timeOrigin;
    private readonly TimerSet _timerSet = new();
    private readonly Dictionary<TimerType, TimerView> _timerViewSet = new();
    private MoodIndicator _moodIndicator;
    private ISaveLoadStorage _saveLoadStorage;
    private ISaveLoadService _saveLoadService;
    private int _currentGameStateValue;
    private DialogManager _dialogManager;
    private GameOverTimerObserver _gameOverTimerObserver;
    private SaveGameTimerObserver _saveGameTimerObserver;
    private IPersistentProgress _persistentProgress;
    private GameOverDialog _gameOverDialog;

    private bool _isGameOverTimerStarted;

    [Inject]
    private void Construct(ISaveLoadStorage saveLoadStorage, ISaveLoadService saveLoadService,
        DialogManager dialogManager, IPersistentProgress persistentProgress)
    {
        _saveLoadStorage = saveLoadStorage;
        _saveLoadService = saveLoadService;
        _dialogManager = dialogManager;
        _persistentProgress = persistentProgress;
        _timeOrigin = new DateTime(1970, 1, 1);
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

            if (soTimer.Type == TimerType.Mood)
            {
                _moodIndicator = new MoodIndicator(_timerSet, saveLoadService, _timeOrigin);
                _moodIndicator.UpdateIndicatorValue += OnMoodIndicatorUpdateValue;
                InstantiateMoodIndicatorView();
            }

            switch (soTimer.Type)
            {
                case TimerType.GameOver:
                    _gameOverTimerObserver = new GameOverTimerObserver(timer, _moodIndicator, _timeOrigin);
                    _gameOverTimerObserver.EndGameOverTimer += OnEndGameOverTimer;
                    break;
                case TimerType.Save:
                    _saveGameTimerObserver = new SaveGameTimerObserver(timer, saveLoadService);
                    break;
                default:
                    Debug.Log($"Timer name: {soTimer.Type}");
                    break;
            }
        }

        _saveLoadStorage.RegisterInSaveLoadRepositories(_moodIndicator);
        _saveLoadStorage.RegisterInSaveLoadRepositories(_gameOverTimerObserver);
    }

    public void Initialize()
    {
        _moodIndicator.Initialize();
        _gameOverTimerObserver.Initialize();
        _saveGameTimerObserver.Initialize();
    }

    private void OnMoodIndicatorUpdateValue(float value)
    {
        //Debug.Log($"<color=green>Mood indicator value changed: {value} game over timer started: {_isGameOverTimerStarted}</color>");

        switch (value)
        {
            case <= 0 when false == _isGameOverTimerStarted:
                _gameOverTimerObserver.StartGameOverTimer();
                _isGameOverTimerStarted = true;
                break;
            case > 0 when _isGameOverTimerStarted:
                _gameOverTimerObserver.StopGameOverTimer();
                _isGameOverTimerStarted = false;
                break;
        }
    }

    private void OnEndGameOverTimer()
    {
        SetGameOverValue(true);
        StopAllTimers();
        ShowGameOverDialog();
    }

    private void ShowGameOverDialog()
    {
        _gameOverDialog = _dialogManager.ShowDialog<GameOverDialog>();
        _gameOverDialog.GameWasContinue += OnGameWasContinue;
    }

    private void StopAllTimers()
    {
        foreach (Timer timer in _timerSet)
        {
            timer.Stop();
        }
    }

    private void OnGameWasContinue()
    {
        SetGameOverValue(false);
        ResetGameOverTimer();
        MoodIndicatorReset();
        RestartBasicTimers();
    }

    private void RestartBasicTimers()
    {
        foreach (Timer timer in _timerSet.Where(x => x.HasRole(TimerRoles.Basic)))
        {
            timer.Restart();
        }
    }

    private void ResetGameOverTimer() =>
        _gameOverTimerObserver.ResetGameOverTimer();

    private void MoodIndicatorReset()
    {
        _moodIndicator.ResetMoodIndicator();
    }

    private void SetGameOverValue(bool value)
    {
        _persistentProgress.PlayerProgress.PlayerState.GameOver = value;
        _isGameOverTimerStarted = false;
        if (false == value)
            _gameOverDialog.GameWasContinue -= OnGameWasContinue;
        SaveProgress();
    }

    private void SaveProgress()
    {
        _saveLoadService.SaveProgress();
    }

    public void StartTimers()
    {
        foreach (Timer timer in _timerSet.Where(x => x.HasRole(TimerRoles.AwakeStart)))
            timer.Start();
    }

    public void SetActiveTimersBySleepPlayerState(bool value)
    {
        foreach (Timer timer in _timerSet.Where(x => x.IndicatorValue > 0 & x.HasRole(TimerRoles.StopToSleep)))
        {
            if (value)
                timer.Start();
            else
                timer.Stop();
        }

        if (value & _moodIndicator.MoodIndicatorValue <= 0)
            _gameOverTimerObserver.StartGameOverTimer();
        else
            _gameOverTimerObserver.StopGameOverTimer();
    }

    private void Update()
    {
        foreach (Timer timer in _timerSet)
            timer.Tick();
    }

    private void OnDisable()
    {
        _moodIndicator.UpdateIndicatorValue -= OnMoodIndicatorUpdateValue;
        _gameOverTimerObserver.Dispose();
        _gameOverTimerObserver.EndGameOverTimer -= OnEndGameOverTimer;

        if (_gameOverDialog is not null)
            _gameOverDialog.GameWasContinue -= OnGameWasContinue;

        _saveGameTimerObserver.Dispose();
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
                timer.UpdateTimerDuration(soTimer.Duration);
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
        var value = CalculateSecondsLastGameSave(playerProgress.TimersData.CurrentWorldTimeInSeconds);
        UpdateTimersProgress(playerProgress, value);
        if (playerProgress.PlayerState.GameOver)
        {
            StopAllTimers();
            ShowGameOverDialog();
        }
    }

    private double CalculateSecondsLastGameSave(double seconds)
    {
        var result = DateTime.Now.Subtract(_timeOrigin).TotalSeconds - seconds;
        Debug.Log($"<color=red>Seconds count {result} </color>");
        return result;
    }

    private static string CurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private void UpdateTimersProgress(PlayerProgress playerProgress, double timeDelta)
    {
        foreach (Timer timer in _timerSet.Where(x =>
                     x.HasRole(TimerRoles.Rewardable) ||
                     x.HasRole(TimerRoles.AwakeStart))
                )
        {
            TimerData timerData = playerProgress.TimersData.Timers
                .FirstOrDefault(x => x.Type == timer.TimerType);

            if (timerData is not null)
            {
                timer.UpdateTimerState(
                    UpdateTimerData(timerData, timeDelta)
                );

                if (timer.Active)
                    timer.Start();
            }
        }
    }

    private TimerData UpdateTimerData(TimerData timerData, double timeDelta)
    {
        double pastTime = timerData.CurrentTime - timeDelta;
        
        if (pastTime <= 0)
            timerData.CurrentTime = 0;
        else
            timerData.CurrentTime = (float)pastTime;

        return timerData;
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        playerProgress.TimersData.CurrentWorldTimeInSeconds =
            DateTime.Now.Subtract(_timeOrigin).TotalSeconds;
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