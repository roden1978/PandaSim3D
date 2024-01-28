using System;
using System.Collections.Generic;
using UnityEngine;

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
        RoomsData = new RoomsData(new List<RoomState>());
    }
}

[Serializable]
public class RoomsData
{
    public List<RoomState> Rooms = new();

    public RoomsData(List<RoomState> roomsData)
    {
        Rooms = roomsData;
    }

    public void Clear()
    {
        Rooms.Clear();
    }
}


[Serializable]
public class RoomState
{
    public RoomState()
    {
        Debug.Log("Create new room");
    }

    public string Name;
    public MealData MealData;
    public bool Poop;
    public SnowmanDecor SnowmanDecor;
}

[Serializable]
public class MealData
{
    public ItemType Type;
}

[Serializable]
public class SnowmanDecor
{
    public ItemType Type;
}