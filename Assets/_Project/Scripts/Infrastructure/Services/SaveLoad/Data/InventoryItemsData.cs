using System;
using System.Collections.Generic;

[Serializable]
public class InventoryItemsData
{
    public List<InventoryItemData> InventoryData;
    public InventoryItemsData(List<InventoryItemData> itemsData)
    {
        InventoryData = itemsData;
    }
}