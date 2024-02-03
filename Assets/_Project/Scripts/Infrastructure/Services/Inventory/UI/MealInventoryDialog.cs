using System;
using System.Collections.Generic;
using System.Linq;
using StaticData;
using TriInspector;
using UnityEngine;

public class MealInventoryDialog : InventoryDialog
{
    private const string InventoryTitle = "Meal";
    
    [Header("Debug")] [ReadOnly] [SerializeField]
    private List<DebugItemData> _debugList;
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

    protected override void SetInventoryTitle()
    {
        _inventoryTitle.text = InventoryTitle;
    }

    private void UpdateDebugList()
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

}