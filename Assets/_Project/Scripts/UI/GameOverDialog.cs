using System;
using Infrastructure;
using Infrastructure.AssetManagement;
using Services.SaveLoad.PlayerProgress;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class GameOverDialog : Dialog
{
    [SerializeField] private PointerListener _continue;
    [SerializeField] private PointerListener _restart;
    [SerializeReference] private GameOverView _gameOverView;

    public event Action GameWasContinue;  
    
    private const int PetPrice = 2000;
    private ISceneLoader _sceneLoader;
    private IWalletService _wallet;
    private IPersistentProgress _persistentProgress;
    private ISaveLoadService _saveLoadService;

    [Inject]
    public void Construct(ISceneLoader sceneLoader, IWalletService wallet, IPersistentProgress persistentProgress,
        ISaveLoadService saveLoadService)
    {
        _sceneLoader = sceneLoader;
        _wallet = wallet;
        _saveLoadService = saveLoadService;
        _persistentProgress = persistentProgress;
        _gameOverView.Construct(wallet, PetPrice);
    }

    private void OnEnable()
    {
        _continue.Click += OnContinue;
        _restart.Click += OnRestart;
    }

    private void OnDisable()
    {
        _continue.Click -= OnContinue;
        _restart.Click -= OnRestart;
    }

    private void OnRestart(PointerEventData data)
    {
        Hide();
        RestartGame();
    }

    private void RestartGame()
    {
        _saveLoadService.Delete();
        _persistentProgress.PlayerProgress = new PlayerProgress();
        _persistentProgress.Settings = new Settings();
        _sceneLoader.LoadScene(AssetPaths.CurtainSceneName);
    }

    private void OnContinue(PointerEventData data)
    {
#if UNITY_EDITOR == false
        _wallet.TrySpend(CurrencyType.Coins, PetPrice);
#endif
        GameWasContinue?.Invoke();
        Hide();
    }
}