using System.Collections.Generic;
using System.Linq;
using Services.SaveLoad.PlayerProgress;
using StaticData;
using UnityEngine;
using Zenject;

public class MealInventoryDialog : InventoryDialog
{
    private const string InventoryTitle = "Meal";
    
    [Inject]
    public void Construct(IInventory inventory, ISaveLoadService saveLoadService, MealDrawer maelDrawer,
        ISaveLoadStorage saveLoadStorage)
    {
        Inventory = inventory;
        SaveLoadService = saveLoadService;
        ItemDrawer = maelDrawer;
        SaveLoadStorage = saveLoadStorage;
        Debug.Log($"Inventory {Inventory}, SaveLoad {SaveLoadService}");
        UISlots = new List<UIInventorySlot>(Inventory.Capacity);

        for (int i = 0; i < Inventory.Capacity; i++)
        {
            UIInventorySlot uiInventorySlot = Instantiate(_uiInventorySlot, _slotHolder);
            uiInventorySlot.Construct(this, i);
            UISlots.Add(uiInventorySlot);
            SaveLoadStorage.RegisterInSaveLoadRepositories(Inventory);
        }
        
        SetInventoryTitle();
    }
    protected override void UpdateAllSlots()
    {
        UpdateDebugList();
        ClearUiSlots();
        IEnumerable<Slot> slots = Inventory.GetAllSlots().Where(x => x.IsEmpty == false)
            .Where(x => x.InventoryItem.StuffSpecies == StuffSpecies.Meal);
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