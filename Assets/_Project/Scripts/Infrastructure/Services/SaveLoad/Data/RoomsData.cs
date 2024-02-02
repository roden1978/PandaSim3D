using System;
using System.Collections.Generic;

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