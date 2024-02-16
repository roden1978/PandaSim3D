using UnityEngine;
using UnityEngine.UI;
using UI;
using System.Collections.Generic;
using System.Linq;
using CustomEventBus.Signals;
using Infrastructure.AssetManagement;
using StaticData;
using TMPro;
using UnityEngine.EventSystems;
using Zenject;


public class ShopDialog : Dialog
{
    [SerializeField] private PointerListener _exitButton;
    [SerializeField] private GridLayoutGroup _elementsGrid;
    [SerializeField] private ShopSlot _slotPrefab;
    [SerializeField] private List<Item> _items;
    [SerializeField] private TMP_Text _gold;

    private readonly List<ShopSlot> _slots = new();
    private IWalletService _walletService;
    private IEventBus _eventBus;
    private IInventory _inventory;
    private ISaveLoadService _saveLoadService;
    private IPersistentProgress _persistentProgress;

    [Inject]
    public void Construct(IWalletService walletService, IInventory inventory, IEventBus eventBus,
        ISaveLoadService saveLoadService, IPersistentProgress persistentProgress)
    {
        _walletService = walletService;
        _eventBus = eventBus;
        _inventory = inventory;
        _saveLoadService = saveLoadService;
        _persistentProgress = persistentProgress;

        foreach (Item item in _items)
        {
            ShopSlot go = Instantiate(_slotPrefab, _elementsGrid.transform);
            go.Construct(_eventBus, item);
            _slots.Add(go);
        }

        long amount = _walletService.GetAmount(CurrencyType.Coins);
        UpdateSlots(amount);
        UpdateGoldAmount(amount);
    }

    private void OnEnable()
    {
        _exitButton.Click += Exit;
        Debug.Log($"Wallet service {_walletService}");
        _eventBus.Subscribe<CoinsViewTextUpdateSignal>(OnUpdateGoldAmount);
        _eventBus.Subscribe<BuyItemSignal>(OnBuyItem);
    }

    private void OnBuyItem(BuyItemSignal signal)
    {
        if (false == CanBuyItem(signal.Item)) return;

        bool result = _walletService.TrySpend(CurrencyType.Coins, signal.Item.Price);

        if (result)
        {
            _inventory.TryAddItem(this, signal.Item, signal.Value);
            Debug.Log($"Was spend {signal.Item.Price}");
            
            if (signal.Item.Type == ItemType.Ball)
                _eventBus.Invoke(new BoughtBallSignal());
            
            SaveProgress();
        }
    }

    private bool CanBuyItem(Item item)
    {
        if (ConfirmMeal(item)) return true;
        if (ConfirmHat(item)) return true;
        if (ConfirmBall(item)) return true;
        if (ConfirmCarrot(item)) return true;
        
        return false;
    }

    private bool ConfirmMeal(Item item)
    {
        return item.StuffSpecies is StuffSpecies.Meal;
    }

    private bool ConfirmHat(Item item)
    {
        var a = item.StuffSpecies is StuffSpecies.Cloths;
        var b = _inventory.HasItem(item.Type);
        if (a && !b)
        {
            return _persistentProgress.PlayerProgress.PlayerState.PlayerDecor.Type == ItemType.None;
        }

        return false;
    }
    
    private bool ConfirmCarrot(Item item)
    {
        var a = item.StuffSpecies is StuffSpecies.Decor;
        var b = _inventory.HasItem(item.Type);
        if (a && !b)
        {
            RoomState room = _persistentProgress.PlayerProgress.RoomsData.Rooms.FirstOrDefault(x =>
                x.Name == AssetPaths.WinterRoomSceneName.ToString());
            if (room is null)
                return true;

            return room.SnowmanDecor.Type != ItemType.Carrot;
        }

        return false;
    }

    private bool ConfirmBall(Item item)
    {
        var a = item.StuffSpecies is StuffSpecies.Toys;
        var b = _inventory.HasItem(item.Type);
        if (a && !b)
        {
            RoomState room = _persistentProgress.PlayerProgress.RoomsData.Rooms.FirstOrDefault(x =>
                x.Name == AssetPaths.RoomSceneName.ToString());
            if (room is null)
                return true;

            return !room.Ball;
        }

        return false;
    }

    private void SaveProgress()
    {
        _saveLoadService.SaveProgress();
    }

    private void OnUpdateGoldAmount(CoinsViewTextUpdateSignal signal)
    {
        Debug.Log($"Update wallet signal, new coins value: {signal.NewValue}");
        UpdateGoldAmount(signal.NewValue);
        UpdateSlots(signal.NewValue);
    }

    private void UpdateGoldAmount(long amount)
    {
        _gold.text = amount.ToString();
    }

    private void OnDisable()
    {
        _exitButton.Click -= Exit;
        _eventBus.Unsubscribe<CoinsViewTextUpdateSignal>(OnUpdateGoldAmount);
        _eventBus.Unsubscribe<BuyItemSignal>(OnBuyItem);
        _slots.Clear();
    }

    private void Exit(PointerEventData obj) => Hide();

    private void UpdateSlots(long amount)
    {
        foreach (ShopSlot slot in _slots)
        {
            if (amount >= slot.Price)
                slot.EnableSlot();
            else
                slot.DisableSlot();
        }
    }
}