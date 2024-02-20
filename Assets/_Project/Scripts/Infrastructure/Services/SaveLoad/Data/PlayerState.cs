using System;
using Data;

[Serializable]
public class PlayerState
{
    public string SceneName;
    public bool FirstStartGame;
    public string PetName;
    public PlayerDecor PlayerDecor;
    public bool Sleep;

    public PlayerState()
    {
        FirstStartGame = true;
        PetName = string.Empty;
        PlayerDecor = new PlayerDecor(ItemType.None, new Vector3Data());
        Sleep = false;
    }
}

[Serializable]
public class PlayerDecor
{
    public ItemType Type;
    public Vector3Data StartPosition;

    public PlayerDecor(ItemType type, Vector3Data startPosition)
    {
        Type = type;
        StartPosition = startPosition;
    }
}