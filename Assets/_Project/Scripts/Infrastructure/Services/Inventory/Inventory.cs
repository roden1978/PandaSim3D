using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using UnityEngine;
using Zenject;


public interface IInventory : ISavedProgress
{
    bool TryAddItem(object sender, Item item, int amount);
    bool TryGetSlotById(int id, out Slot slot);
    bool HasSlotById(int id);
    IEnumerable<Slot> GetAllSlots();
    bool RemoveItem(int id);
    bool HasItem(ItemType type);
    int Capacity { get; }
}

public class Inventory : IInitializable, IInventory
{
    public int Capacity { get; }
    private readonly IAssetProvider _assetProvider;
    private readonly List<Slot> _slots;
    private readonly List<InventoryItemData> _items = new();

    public Inventory(IAssetProvider assetProvider)
    {
        Capacity = 15;
        _assetProvider = assetProvider;
        _slots = new List<Slot>(Capacity);
    }

    public void Initialize()
    {
        for (int i = 0; i < Capacity; i++)
        {
            _slots.Add(new Slot { Id = i });
        }

        Debug.Log($"SLots count {_slots.Count}");
    }

    private async UniTask<Item> LoadInventoryItem(string itemName)
    {
        ScriptableObject result = await _assetProvider.LoadAsync<ScriptableObject>(itemName); 
        _assetProvider.ReleaseAssetsByLabel(itemName);
        return result as Item;
    }

    public bool TryAddItem(object sender, Item item, int amount)
    {
        Slot slot = _slots.Find(x =>
                        x.InventoryItem is not null &&
                        x.InventoryItem.Type == item.Type)
                    ?? _slots.Find(x => x.IsEmpty);
        if (slot is null)
        {
            Debug.Log("Inventory is full");
            return false;
        }

        if (slot.IsEmpty)
        {
            slot.ItemAmount = amount;
            slot.InventoryItem = item;
            slot.InventoryItem.Sprite = item.Sprite;
            return true;
        }

        slot.ItemAmount += amount;

        return true;
    }

    public bool TryGetSlotById(int id, out Slot slot)
    {
        if (id == Int32.MaxValue)
        {
            slot = default;
            return false;
        }
        slot = _slots.FirstOrDefault(x => x.Id == id);
        return slot is not null;
    }
    
    public bool HasSlotById(int id)
    {
        return _slots.FirstOrDefault(x => x.Id == id) is not null;
    }

    public bool HasItem(ItemType type)
    {
        var slot = _slots.Where(x => x.IsEmpty == false).FirstOrDefault(x => x.InventoryItem.Type == type);
        return slot is not null;
    }


    public IEnumerable<Slot> GetAllSlots()
    {
        return _slots;
    }

    public bool RemoveItem(int id)
    {
        if (TryGetSlotById(id, out Slot slot))
        {
            slot.ItemAmount -= 1;

            if (slot.ItemAmount == 0)
                ClearSlot(slot);
            return true;
        }

        return false;
    }


    private void ClearSlot(Slot slot)
    {
        slot.ItemAmount = 0;
        slot.InventoryItem = null;
    }

    public async void LoadProgress(PlayerProgress playerProgress)
    {
        var progressInventoryData = playerProgress.InventoryItemsData.InventoryData.ToList();
            Debug.Log($"Inventory item loaded {string.Join(", ",progressInventoryData.Select(x => x.Name))}");
        
        foreach (InventoryItemData item in progressInventoryData)
        {
            Item inventoryItem = await LoadInventoryItem(item.Name + "Data");
            if (inventoryItem is null) return;

            if (TryGetSlotById(item.SlotId, out Slot slot))
            {
                slot.ItemAmount = item.Amount;
                slot.InventoryItem = inventoryItem;
            }
        }
        Debug.Log("Stop");
    }

    public void SaveProgress(PlayerProgress playerProgress)
    {
        _items.Clear();

        _slots.Where(slot => slot.ItemAmount > 0)
            .ToList()
            .ForEach(slot => _items
                .Add(new InventoryItemData
                {
                    SlotId = slot.Id,
                    ItemType = (int)slot.InventoryItem.Type,
                    Name = slot.InventoryItem.Name,
                    Amount = slot.ItemAmount
                }));

        playerProgress.InventoryItemsData = new InventoryItemsData(_items);
    }
}