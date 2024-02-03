using System.Collections.Generic;
using System.Linq;
using StaticData;

public class ClothsInventoryDialog : InventoryDialog
{
    private const string InventoryTitle = "Cloths";
    protected override void UpdateAllSlots()
    {
        UpdateDebugList();
        ClearUiSlots();
        IEnumerable<Slot> slots = Inventory.GetAllSlots().Where(x => x.IsEmpty == false)
            .Where(x => x.InventoryItem.StuffSpecies == StuffSpecies.Cloths);
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
}