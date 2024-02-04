using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    public SnowmanDecor SnowmanDecor;
}