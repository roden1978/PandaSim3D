﻿using System;
using CustomEventBus;
using CustomEventBus.Signals;
using Infrastructure;
using Infrastructure.AssetManagement;
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
    [SerializeField] private TMP_Text _petName;
    [SerializeField] private TMP_Text _currencyValueText;
    private ISceneLoader _sceneLoader;
    private DialogManager _dialogManager;
    private EventBus _eventBus;
    [Inject]
    public void Construct(ISceneLoader sceneLoader, DialogManager dialogManager, EventBus eventBus)
    {
        _sceneLoader = sceneLoader;
        _dialogManager = dialogManager;
        _eventBus = eventBus;
    }
    private void OnEnable()
    {
        _shop.Click += OnShopButtonClick;
        _winterRoom.Click += OnWinterRoomButtonClick;
        _room.Click += OnRoomButtonClick;
        _ads.Click += OnAdsButtonClick;
        Debug.Log($"Event bus in hud {_eventBus}");
        _eventBus.Subscribe<CoinsViewTextUpdateSignal>(OnWalletUpdateSignal);
    }

    private void OnWalletUpdateSignal(CoinsViewTextUpdateSignal signal)
    {
        _currencyValueText.text = signal.NewValue.ToString();
    }

    private void OnAdsButtonClick(PointerEventData obj)
    {
      
    }

    private void OnRoomButtonClick(PointerEventData obj)
    {
        throw new NotImplementedException();
    }

    private void OnWinterRoomButtonClick(PointerEventData obj)
    {
        _sceneLoader.LoadScene(AssetPaths.WinterRoomSceneName);
    }

    private void OnShopButtonClick(PointerEventData obj)
    {
        _dialogManager.ShowDialog<ShopDialog>();
    }

    private void OnDisable()
    {
        _shop.Click -= OnShopButtonClick;
        _winterRoom.Click -= OnWinterRoomButtonClick;
        _room.Click -= OnRoomButtonClick;
        _ads.Click -= OnAdsButtonClick;
        _eventBus.Unsubscribe<CoinsViewTextUpdateSignal>(OnWalletUpdateSignal);
    }

    public void UpdatePetName(string petName)
    {
        _petName.text = petName;
    }

    public void LoadProgress(PlayerProgress playerProgress)
    {
        _petName.text = playerProgress.PlayerState.PetName;

        string currentSceneName = SceneManager.GetActiveScene().name;
        RoomsButtonsSwitch(currentSceneName);
    }

    private void RoomsButtonsSwitch(string sceneName)
    {
        switch (sceneName)
        {
            case AssetPaths.WinterRoomSceneName :
                PointerListenerSetActive(_winterRoom, false);
                PointerListenerSetActive(_room, true);
                break;
            case AssetPaths.RoomSceneName:
                PointerListenerSetActive(_winterRoom, true);
                PointerListenerSetActive(_room, false);
                break;
        }
    }

    private void PointerListenerSetActive(Component pointerListener, bool value)
    {
        pointerListener.gameObject.SetActive(value);
    }

    public void SaveProgress(PlayerProgress persistentPlayerProgress)
    {
        
    }
}