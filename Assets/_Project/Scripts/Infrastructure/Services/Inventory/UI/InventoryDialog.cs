using System;
using System.Collections.Generic;
using System.Linq;
using Services.SaveLoad.PlayerProgress;
using StaticData;
using TriInspector;
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
    [SerializeField] private CanvasGroup _useButtonCanvasGroup;
    [SerializeField] private PointerListener _dropButton;
    [SerializeField] private RectTransform _slotHolder;
    [SerializeField] private UIInventorySlot _uiInventorySlot;
    [SerializeField] private UIDescriptionHolder _uiDescriptionHolder;

    [Header("Debug")] [ReadOnly] [SerializeField]
    private List<DebugItemData> _debugList;

    private IInventory _inventory;
    private ISaveLoadService _saveLoadService;
    private Plate _plate;
    private List<UIInventorySlot> _uiSlots;
    private int _currentSlotId = int.MaxValue;

    private UIInventorySlot _selectedSlot;
    private ISaveLoadStorage _saveLoadStorage;

    [Inject]
    public void Construct(IInventory inventory, ISaveLoadService saveLoadService, Plate plate,
        ISaveLoadStorage saveLoadStorage)
    {
        _inventory = inventory;
        _saveLoadService = saveLoadService;
        _plate = plate;
        _saveLoadStorage = saveLoadStorage;
        Debug.Log($"Inventory {_inventory}, SaveLoad {_saveLoadService}");
        _uiSlots = new List<UIInventorySlot>(_inventory.Capacity);

        for (int i = 0; i < _inventory.Capacity; i++)
        {
            UIInventorySlot uiInventorySlot = Instantiate(_uiInventorySlot, _slotHolder);
            uiInventorySlot.Construct(this, i);
            _uiSlots.Add(uiInventorySlot);

            /*if (_inventory.TryGetSlotById(i, out Slot slot))
            {
                if (!slot.IsEmpty)
                {
                    uiInventorySlot.UIItem.Icon.sprite = slot.InventoryItem.Sprite;
                    uiInventorySlot.UIItem.ValueText.text = $"x{slot.ItemAmount}";
                    uiInventorySlot.UIItem.ItemType = slot.InventoryItem.Type;
                    uiInventorySlot.UIItem.InventorySlotId = slot.Id;
                    uiInventorySlot.UIItem.gameObject.SetActive(true);

                    _debugList.Add(new DebugItemData
                    {
                        Name = slot.InventoryItem.Type.ToString(),
                        Value = slot.ItemAmount,
                    });
                }
            }*/
            _saveLoadStorage.RegisterInSaveLoadRepositories(_inventory);
        }
    }

    private void OnEnable()
    {
        _closeButton.Click += OnCloseButtonClick;
        _dropButton.Click += OnUseButtonClick;
        _useButton.Click += OnUseButtonClick;
    }

    public void UpdateInventoryView()
    {
        UpdateAllSlots();
    }

    private void UpdateAllSlots()
    {
        UpdateDebugList();
        ClearUiSlots();
        IEnumerable<Slot> slots = _inventory.GetAllSlots().Where(x => x.IsEmpty == false)
            .Where(x => x.InventoryItem.StuffSpecies == StuffSpecies.Meal);
        int i = 0;
        foreach (Slot slot in slots)
        {
            _uiSlots[i].UIItem.gameObject.SetActive(true);
            _uiSlots[i].UIItem.ValueText.text = $"x{slot.ItemAmount}";
            _uiSlots[i].UIItem.Icon.sprite = slot.InventoryItem.Sprite;
            _uiSlots[i].UIItem.ItemType = slot.InventoryItem.Type;
            _uiSlots[i].UIItem.InventorySlotId = slot.Id;
            i++;
        }
    }

    private void UpdateDebugList()
    {
        IEnumerable<Slot> slots = _inventory.GetAllSlots().Where(x => x.IsEmpty == false);
        foreach (Slot slot in slots)
        {
            _debugList.Add(new DebugItemData
            {
                Name = slot.InventoryItem.Type.ToString(),
                Value = slot.ItemAmount,
            });
        }
    }

    private void OnUseButtonClick(PointerEventData data)
    {
        int slotId = GetActiveSlotId();

        if (slotId == int.MaxValue) return;

        InstantiateMeal();

        if (_inventory.RemoveItem(_uiSlots[slotId].UIItem.InventorySlotId))
        {
            UpdateUiSlot(slotId);
            UpdateDescription(slotId);
            UpdateAllSlots();
            SaveProgress();
            Hide();
            Debug.Log(data.pointerClick.gameObject.name == "UseButton" ? "Item was used" : "Item was dropped");
        }
    }

    private void InstantiateMeal()
    {
        if (TryGetActiveSlot(out UIInventorySlot uiSlot))
            _plate.InstantiateMeal(uiSlot.UIItem.ItemType);
        else
            Debug.Log("No active slots");
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

    private void ClearUiSlots()
    {
        foreach (UIInventorySlot uiSlot in _uiSlots)
        {
            uiSlot.UIItem.ValueText.text = string.Empty;
            uiSlot.UIItem.Icon.sprite = null;
            uiSlot.UIItem.ItemType = ItemType.None;
            uiSlot.UIItem.InventorySlotId = Int32.MaxValue;
            uiSlot.IsActive = false;
            uiSlot.ActivateFrame(false);
            uiSlot.UIItem.gameObject.SetActive(false);
        }
    }

    private void UpdateUseButtonCanvasGroup(float alpha, bool interactable, bool blocksRaycast)
    {
        _useButtonCanvasGroup.alpha = alpha;
        _useButtonCanvasGroup.interactable = interactable;
        _useButtonCanvasGroup.blocksRaycasts = blocksRaycast;
    }

    public void ChangeActiveSlot(int id)
    {
        if (_inventory.HasSlotById(_uiSlots[id].UIItem.InventorySlotId))
        {
            if (_currentSlotId != int.MaxValue)
            {
                UpdateUseButtonCanvasGroup(.5f, false, false);
                _uiSlots[_currentSlotId].ActivateFrame(false);
                _uiSlots[_currentSlotId].IsActive = false;
            }

            _uiSlots[id].ActivateFrame(true);
            _uiSlots[id].IsActive = true;
            UpdateUseButtonCanvasGroup(1f, true, true);

            _currentSlotId = id;

            UpdateDescription(id);
        }
    }

    private void UpdateDescription(int id)
    {
        if (_inventory.TryGetSlotById(_uiSlots[id].UIItem.InventorySlotId, out Slot slot))
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

    private bool TryGetActiveSlot(out UIInventorySlot uiSlot)
    {
        uiSlot = _uiSlots.FirstOrDefault(x => x.IsActive);
        return uiSlot is not null;
    }

    private void OnDisable()
    {
        _closeButton.Click -= OnCloseButtonClick;
        _dropButton.Click -= OnUseButtonClick;
        _useButton.Click -= OnUseButtonClick;
    }

    [Serializable]
    public class DebugItemData
    {
        public string Name;
        public int Value;
    }

}