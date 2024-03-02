using System;
using CustomEventBus.Signals;
using Infrastructure;
using Infrastructure.AssetManagement;
using Infrastructure.Services.EventBus.Signals.PlayerSignals;
using PlayerScripts;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Zenject;

public class Hud : MonoBehaviour, ISavedProgress
{
    [SerializeField] private PointerListener _shop;
    [SerializeField] private PointerListener _winterRoom;
    [SerializeField] private PointerListener _room;
    [SerializeField] private PointerListener _ads;
    [SerializeField] private PointerListener _menu;
    [SerializeField] private TMP_Text _petName;
    [SerializeField] private TMP_Text _currencyValueText;
    [SerializeField] private CanvasGroup _winterRoomButtonCanvasGroup;
    private ISceneLoader _sceneLoader;
    private DialogManager _dialogManager;
    private EventBus _eventBus;
    private ISaveLoadService _saveLoadService;

    [Inject]
    public void Construct(ISceneLoader sceneLoader, DialogManager dialogManager, EventBus eventBus,
       ISaveLoadService saveLoadService)
    {
        _sceneLoader = sceneLoader;
        _dialogManager = dialogManager;
        _eventBus = eventBus;
        _saveLoadService = saveLoadService;
    }

    private void OnEnable()
    {
        _shop.Click += OnShopButtonClick;
        _winterRoom.Click += OnWinterRoomButtonClick;
        _room.Click += OnRoomButtonClick;
        _ads.Click += OnAdsButtonClick;
        _menu.Click += OnMenuButtonClick;
        _eventBus.Subscribe<ChangePlayerStateSignal>(OnPlayerChangePlayerState);
        Debug.Log($"Event bus in hud {_eventBus}");
        _eventBus.Subscribe<CoinsViewTextUpdateSignal>(OnWalletUpdateSignal);
    }
    
    private void OnDisable()
    {
        _shop.Click -= OnShopButtonClick;
        _winterRoom.Click -= OnWinterRoomButtonClick;
        _room.Click -= OnRoomButtonClick;
        _ads.Click -= OnAdsButtonClick;
        _menu.Click -= OnMenuButtonClick;
        _eventBus.Unsubscribe<ChangePlayerStateSignal>(OnPlayerChangePlayerState);
        _eventBus.Unsubscribe<CoinsViewTextUpdateSignal>(OnWalletUpdateSignal);
    }

    private void OnPlayerChangePlayerState(ChangePlayerStateSignal signal)
    {
        SwitchingWinterRoomButton(signal.PlayerState);
    }
    
    private void OnWalletUpdateSignal(CoinsViewTextUpdateSignal signal)
    {
        _currencyValueText.text = signal.NewValue.ToString();
    }

    private void OnMenuButtonClick(PointerEventData obj)
    {
        _dialogManager.ShowDialog<MenuDialog>();
    }

    private void OnAdsButtonClick(PointerEventData obj)
    {
    }

    private void OnRoomButtonClick(PointerEventData obj)
    {
        SaveProgress();
        LoadScene(AssetPaths.RoomSceneName.ToString());
    }

    private void OnWinterRoomButtonClick(PointerEventData obj)
    {
        SaveProgress();
        LoadScene(AssetPaths.WinterRoomSceneName.ToString());
    }

    private void OnShopButtonClick(PointerEventData obj)
    {
        _dialogManager.ShowDialog<ShopDialog>();
    }

    public void UpdatePetName(string petName)
    {
        _petName.text = petName;
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        UpdatePetName(playerProgress.PlayerState.PetName);
        UpdateHudButtons(playerProgress.PlayerState.State);
    }


    private void UpdateHudButtons(State state)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        RoomsType type = Enum.Parse<RoomsType>(currentSceneName);
        RoomsButtonsSwitch(type, state);
    }

    private void RoomsButtonsSwitch(RoomsType type, State state)
    {
        switch (type)
        {
            case AssetPaths.WinterRoomSceneName:
                PointerListenerSetActive(_winterRoom, false);
                PointerListenerSetActive(_room, true);
                break;
            case AssetPaths.RoomSceneName:
                PointerListenerSetActive(_winterRoom, true);
                PointerListenerSetActive(_room, false);
                SwitchingWinterRoomButton(state);
                break;
            case RoomsType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    private void SwitchingWinterRoomButton(State state)
    {
        if (state == State.Sleep)
            SetCanvasGroupValues(_winterRoomButtonCanvasGroup, .7f, false, false);
        else
            SetCanvasGroupValues(_winterRoomButtonCanvasGroup, 1f, true, true);
    }

    private void PointerListenerSetActive(Component pointerListener, bool value)
    {
        pointerListener.gameObject.SetActive(value);
    }

    private void SetCanvasGroupValues(CanvasGroup canvasGroup, float alphaValue, bool interactable, bool blockRaycast)
    {
        canvasGroup.alpha = alphaValue;
        canvasGroup.interactable = interactable;
        canvasGroup.blocksRaycasts = blockRaycast;
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        //Debug.Log("Break Point");
    }

    private void SaveProgress()
    {
        _saveLoadService.SaveProgress();
    }

    private void LoadScene(string sceneName)
    {
        _sceneLoader.LoadScene(sceneName);
    }
}