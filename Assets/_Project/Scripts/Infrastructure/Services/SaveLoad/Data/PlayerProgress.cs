using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProgress
{
    public PlayerState PlayerState;
    public StaticPlayerData StaticPlayerData;
    public InventoryItemsData InventoryItemsData;
    public WalletsData WalletsData;
    public TimersData TimersData;
    public RoomsData RoomsData;

    public PlayerProgress()
    {
        PlayerState = new PlayerState();
        StaticPlayerData = new StaticPlayerData();
        InventoryItemsData = new InventoryItemsData(new List<InventoryItemData>());
        WalletsData = new WalletsData(new Dictionary<int, long>());
        TimersData = new TimersData(new List<TimerData>());
    }
}

public class RoomsData
{
    public List<RoomState> Rooms;

    public RoomsData(List<RoomState> roomsData)
    {
        Rooms = roomsData;
    }

    public void Clear()
    {
        Rooms.Clear();
    }
}

public class RoomState
{
    public string Name;
    public FoodData FoodData;
    public bool Poop;
    public SnowmanDecor SnowmanDecor;
}

public class FoodData
{
    public ItemType Type;
}

public class SnowmanDecor
{
    public ItemType Type;
}