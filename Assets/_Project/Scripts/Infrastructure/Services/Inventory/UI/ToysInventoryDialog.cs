using System.Collections.Generic;
using System.Linq;
using Services.SaveLoad.PlayerProgress;
using StaticData;
using UnityEngine;
using Zenject;

public class ToysInventoryDialog : InventoryDialog
{
    private const string InventoryTitle = "Toys";
    private const int Capacity = 1;
    private ToysDrawer _toysDrawer;

    [Inject]
    public void Construct(IInventory inventory, ISaveLoadService saveLoadService, ToysDrawer toysDrawer,
        ISaveLoadStorage saveLoadStorage)
    {
        Inventory = inventory;
        SaveLoadService = saveLoadService;
        ItemDrawer = _toysDrawer = toysDrawer;
        SaveLoadStorage = saveLoadStorage;
        Debug.Log($"Inventory {Inventory}, SaveLoad {SaveLoadService}");
        UISlots = new List<UIInventorySlot>(Inventory.Capacity);

        for (int i = 0; i < Capacity; i++)
        {
            UIInventorySlot uiInventorySlot = Instantiate(_uiInventorySlot, _slotHolder);
            uiInventorySlot.Construct(this, i);
            UISlots.Add(uiInventorySlot);
            SaveLoadStorage.RegisterInSaveLoadRepositories(Inventory);
        }

        SetInventoryTitle();
    }

    protected override void UseButtonAction()
    {
        _toysDrawer.DrawerSetActive(false);
        base.UseButtonAction();
    }

    protected override void UpdateAllSlots()
    {
        UpdateDebugList();
        ClearUiSlots();
        IEnumerable<Slot> slots = Inventory.GetAllSlots().Where(x => x.IsEmpty == false)
            .Where(x => x.InventoryItem.StuffSpecies == StuffSpecies.Toys);
        int i = 0;
        foreach (Slot slot in slots)
        {
            UISlots[i].UIItem.gameObject.SetActive(true);
            UISlots[i].UIItem.ValueText.text = $"x{slot.ItemAmount}";
            UISlots[i].UIItem.Icon.sprite = slot.InventoryItem.Sprite;
            UISlots[i].UIItem.ItemType = slot.InventoryItem.Type;
            UISlots[i].UIItem.InventorySlotId = slot.Id;
            i++;
        }
    }

    private void SetInventoryTitle()
    {
        _inventoryTitle.text = InventoryTitle;
    }
}