using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UI.Dialogs.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public interface ISlotChanger
{
    void ChangeActiveSlot(int id);
}

public class InventoryDialog : Dialog, ISlotChanger
{
    [SerializeField] private PointerListener _closeButton;
    [SerializeField] private PointerListener _useButton;
    [SerializeField] private PointerListener _dropButton;
    [SerializeField] private RectTransform _slotHolder;
    [SerializeField] private UIInventorySlot _uiInventorySlot;
    [SerializeField] private UIDescriptionHolder _uiDescriptionHolder;
    private IInventory _inventory;
    private IEventBus _eventBus;
    private ISaveLoadService _saveLoadService;
    private List<UIInventorySlot> _uiSlots;
    private int _currentSlotId = int.MaxValue;

    [Inject]
    public void Construct(IInventory inventory, IEventBus eventBus, ISaveLoadService saveLoadService)
    {
        _inventory = inventory;
        _eventBus = eventBus;
        _saveLoadService = saveLoadService;
        Debug.Log($"Inventory {_inventory}, EventBus {_eventBus}, SaveLoad {_saveLoadService}");
        _uiSlots = new List<UIInventorySlot>(_inventory.Capacity);

        for (int i = 0; i < _inventory.Capacity; i++)
        {
            UIInventorySlot uiInventorySlot = Instantiate(_uiInventorySlot, _slotHolder);
            uiInventorySlot.Construct(this, i);
            _uiSlots.Add(uiInventorySlot);

            if (_inventory.TryGetSlotById(i, out Slot slot))
            {
                if (!slot.IsEmpty)
                {
                    uiInventorySlot.UIItem.Icon.sprite = slot.InventoryItem.Sprite;
                    uiInventorySlot.UIItem.ValueText.text = $"x{slot.ItemAmount}";
                    uiInventorySlot.UIItem.gameObject.SetActive(true);
                }
            }
        }
    }

    private void OnEnable()
    {
        _closeButton.Click += OnCloseButtonClick;
        _dropButton.Click += OnUseButtonClick;
        _useButton.Click += OnUseButtonClick;
        _eventBus.Subscribe<UpdateInventoryView>(OnUpdateInventoryView);
    }

    private void OnUpdateInventoryView(UpdateInventoryView obj)
    {
        UpdateAllSlots();
    }

    private void UpdateAllSlots()
    {
        IEnumerable<Slot> slots = _inventory.GetAllSlots();
        foreach (Slot slot in slots)
        {
            if (slot.IsEmpty)
            {
                _uiSlots[slot.Id].UIItem.gameObject.SetActive(false);
                continue;
            }

            _uiSlots[slot.Id].UIItem.gameObject.SetActive(true);
            _uiSlots[slot.Id].UIItem.ValueText.text = $"x{slot.ItemAmount}";
            _uiSlots[slot.Id].UIItem.Icon.sprite = slot.InventoryItem.Sprite;
        }
    }

    private void OnUseButtonClick(PointerEventData data)
    {
        int slotId = GetActiveSlotId();

        if (slotId == int.MaxValue) return;

        if (_inventory.RemoveItem(slotId))
        {
            UpdateUiSlot(slotId);
            UpdateDescription(slotId);
            SaveProgress();
            Debug.Log(data.pointerClick.gameObject.name == "UseButton" ? "Item was used" : "Item was dropped");
        }
    }

    private void SaveProgress()
    {
        _saveLoadService.SaveProgress();
    }

    private void OnCloseButtonClick(PointerEventData data)
    {
        Hide();
        Debug.Log("Inventory was closed");
    }

    private void UpdateUiSlot(int slotId)
    {
        if (_inventory.TryGetSlotById(slotId, out Slot slot))
        {
            if (slot.IsEmpty)
            {
                _uiSlots[slotId].UIItem.gameObject.SetActive(false);
                _uiSlots[slotId].ActivateFrame(false);
                return;
            }

            _uiSlots[slotId].UIItem.ValueText.text = $"x{slot.ItemAmount}";
            _uiSlots[slotId].UIItem.Icon.sprite = slot.InventoryItem.Sprite;
        }
    }

    public void ChangeActiveSlot(int id)
    {
        if (_inventory.TryGetSlotById(id, out Slot slot))
        {
            if (slot.IsEmpty) return;

            if (_currentSlotId != int.MaxValue)
            {
                _uiSlots[_currentSlotId].ActivateFrame(false);
                _uiSlots[_currentSlotId].IsActive = false;
            }

            _uiSlots[slot.Id].ActivateFrame(true);
            _uiSlots[slot.Id].IsActive = true;


            _currentSlotId = slot.Id;

            UpdateDescription(slot.Id);
        }
    }

    private void UpdateDescription(int id)
    {
        if (_inventory.TryGetSlotById(id, out Slot slot))
        {
            if (slot.IsEmpty)
            {
                _uiDescriptionHolder.DescriptionText.text = string.Empty;
                _uiDescriptionHolder.Icon.sprite = null;
            }
            else
            {
                _uiDescriptionHolder.Icon.sprite = slot.InventoryItem.Sprite;
                _uiDescriptionHolder.DescriptionText.text = slot.InventoryItem.Description;
            }
        }
    }

    private int GetActiveSlotId()
    {
        UIInventorySlot uiSlot = _uiSlots.FirstOrDefault(x => x.IsActive);
        return uiSlot == null ? int.MaxValue : uiSlot.Id;
    }

    private void OnDisable()
    {
        _closeButton.Click -= OnCloseButtonClick;
        _dropButton.Click -= OnUseButtonClick;
        _useButton.Click -= OnUseButtonClick;
        _eventBus.Unsubscribe<UpdateInventoryView>(OnUpdateInventoryView);
    }
}