using Infrastructure;
using Infrastructure.AssetManagement;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class GameOverDialog : Dialog
{
    [SerializeField] private PointerListener _continue;
    [SerializeField] private CanvasGroup _continueCanvasGroup;
    [SerializeField] private PointerListener _restart;
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
    }

    private void OnEnable()
    {
        _continue.Click += OnContinue;
        _restart.Click += OnRestart;
        ValidateCurrencyCount();
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
        Hide();
    }

    private void SetCanvasGroupValues(CanvasGroup canvasGroup, float alphaValue, bool interactable, bool blockRaycast)
    {
        canvasGroup.alpha = alphaValue;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = blockRaycast;
    }

    private void ValidateCurrencyCount()
    {
        if (_wallet.GetAmount(CurrencyType.Coins) < PetPrice)
            SetCanvasGroupValues(_continueCanvasGroup, .5f, false, false);
        else
            SetCanvasGroupValues(_continueCanvasGroup, 1f, true, true);
    }
}