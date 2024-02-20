using System;
using UnityEngine;

[Serializable]
public class RoomState
{
    public RoomState()
    {
        Debug.Log("Create new room");
    }

    public string Name;
    public ItemData ItemData;
    public ClothsData ClothsData;
    public bool Poop;
    public bool Ball;
    public bool ToysDrawer;
    public SnowmanDecor SnowmanDecor;
}