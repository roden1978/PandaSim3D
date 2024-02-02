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
    public MealData MealData;
    public bool Poop;
    public SnowmanDecor SnowmanDecor;
}