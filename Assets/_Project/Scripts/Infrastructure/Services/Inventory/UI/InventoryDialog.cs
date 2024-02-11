using System;
using System.Collections.Generic;
using System.Linq;
using Services.SaveLoad.PlayerProgress;
using TMPro;
using TriInspector;
using UI;
using UI.Dialogs.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class InventoryDialog : Dialog, ISlotChanger
{
    [SerializeField] private PointerListener _closeButton;
    [SerializeField] private PointerListener _useButton;
    [SerializeField] private CanvasGroup _useButtonCanvasGroup;
    [SerializeField] private PointerListener _dropButton;
    [SerializeField] protected RectTransform _slotHolder;

    [SerializeField] [PropertyTooltip("Ui inventory slot prefab")]
    protected UIInventorySlot _uiInventorySlot;

    [SerializeField] private UIDescriptionHolder _uiDescriptionHolder;
    [SerializeField] protected TMP_Text _inventoryTitle;

    [Header("Debug")] [ReadOnly] [SerializeField]
    private List<DebugItemData> _debugList;

    protected IInventory Inventory;
    protected List<UIInventorySlot> UISlots;

    protected ISaveLoadService SaveLoadService;
    protected ItemDrawer ItemDrawer;
    private int _currentSlotId = int.MaxValue;

    private UIInventorySlot _selectedSlot;
    protected ISaveLoadStorage SaveLoadStorage;
    protected abstract void UpdateAllSlots();

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


    private void OnUseButtonClick(PointerEventData data)
    {
        UseButtonAction();
    }

    protected virtual void UseButtonAction()
    {
        int slotId = TryGetActiveSlotId(out UIInventorySlot uiInventorySlot);

        if (slotId == int.MaxValue) return;

        InstantiateItem(uiInventorySlot);

        if (Inventory.RemoveItem(UISlots[slotId].UIItem.InventorySlotId))
        {
            UpdateUiSlot(slotId);
            UpdateDescription(slotId);
            UpdateAllSlots();
            SaveProgress();
            Hide();
        }
    }

    private void InstantiateItem(UIInventorySlot uiSlot)
    {
        ItemDrawer.InstantiateItemByType(uiSlot.UIItem.ItemType);
    }


    private void SaveProgress()
    {
        SaveLoadService.SaveProgress();
    }

    private void OnCloseButtonClick(PointerEventData data)
    {
        Hide();
        Debug.Log("Inventory was closed");
    }

    private void UpdateUiSlot(int slotId)
    {
        if (Inventory.TryGetSlotById(slotId, out Slot slot))
        {
            if (slot.IsEmpty)
            {
                UISlots[slotId].UIItem.gameObject.SetActive(false);
                UISlots[slotId].ActivateFrame(false);
                return;
            }

            UISlots[slotId].UIItem.ValueText.text = $"x{slot.ItemAmount}";
            UISlots[slotId].UIItem.Icon.sprite = slot.InventoryItem.Sprite;
        }
    }

    protected void ClearUiSlots()
    {
        foreach (UIInventorySlot uiSlot in UISlots)
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
        if (Inventory.HasSlotById(UISlots[id].UIItem.InventorySlotId))
        {
            if (_currentSlotId != int.MaxValue)
            {
                UpdateUseButtonCanvasGroup(.5f, false, false);
                UISlots[_currentSlotId].ActivateFrame(false);
                UISlots[_currentSlotId].IsActive = false;
            }

            UISlots[id].ActivateFrame(true);
            UISlots[id].IsActive = true;
            UpdateUseButtonCanvasGroup(1f, true, true);

            _currentSlotId = id;

            UpdateDescription(id);
        }
    }

    private void UpdateDescription(int id)
    {
        if (Inventory.TryGetSlotById(UISlots[id].UIItem.InventorySlotId, out Slot slot))
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

    private int TryGetActiveSlotId(out UIInventorySlot uiInventorySlot)
    {
        uiInventorySlot = UISlots.FirstOrDefault(x => x.IsActive);
        return uiInventorySlot == null ? int.MaxValue : uiInventorySlot.Id;
    }

    private bool TryGetActiveSlot(out UIInventorySlot uiSlot)
    {
        uiSlot = UISlots.FirstOrDefault(x => x.IsActive);
        return uiSlot is not null;
    }

    protected void UpdateDebugList()
    {
        IEnumerable<Slot> slots = Inventory.GetAllSlots().Where(x => x.IsEmpty == false);
        foreach (Slot slot in slots)
        {
            _debugList.Add(new DebugItemData
            {
                Name = slot.InventoryItem.Type.ToString(),
                Value = slot.ItemAmount,
            });
        }
    }

    [Serializable]
    public class DebugItemData
    {
        public string Name;
        public int Value;
    }

    private void OnDisable()
    {
        _closeButton.Click -= OnCloseButtonClick;
        _dropButton.Click -= OnUseButtonClick;
        _useButton.Click -= OnUseButtonClick;
    }
}