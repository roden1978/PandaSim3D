using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress
{
    public PlayerState PlayerState;
    public StaticPlayerData StaticPlayerData;
    public InventoryItemsData InventoryItemsData;
    public WalletsData WalletsData;
    public PlayerProgress()
    {
        PlayerState = new PlayerState();
        StaticPlayerData = new StaticPlayerData();
        InventoryItemsData = new InventoryItemsData(new List<InventoryItemData>());
        WalletsData = new WalletsData(new Dictionary<int, long>());
    }
}