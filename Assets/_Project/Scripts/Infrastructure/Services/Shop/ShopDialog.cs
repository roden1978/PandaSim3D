using UnityEngine;
using UnityEngine.UI;
using UI;
using System.Collections.Generic;
using CustomEventBus.Signals;
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

    [Inject]
    public void Construct(IWalletService walletService, IInventory inventory, IEventBus eventBus,
        ISaveLoadService saveLoadService)
    {
        _walletService = walletService;
        _eventBus = eventBus;
        _inventory = inventory;
        _saveLoadService = saveLoadService;

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
        if (false == CheckEquipment(signal.Item))
            if (_walletService.TrySpend(CurrencyType.Coins, signal.Item.Price))
            {
                _inventory.TryAddItem(this, signal.Item, signal.Value);
                //_eventBus.Invoke(new UpdateInventoryView());
                SaveProgress();
                Debug.Log($"Was spend {signal.Item.Price}");
            }
    }

    private bool CheckEquipment(Item item)
    {
        return item.StuffSpecies == StuffSpecies.Cloths && _inventory.HasItem(item.Type);
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