using System;
using Data;
using PlayerScripts;

[Serializable]
public class PlayerState
{
    public string SceneName;
    public bool FirstStartGame;
    public string PetName;
    public PlayerDecor PlayerDecor;
    public State State;
    public bool GameOver;
    public Vector3Data Position;
    public QuaternionData Rotation;

    public PlayerState()
    {
        FirstStartGame = true;
        PetName = string.Empty;
        PlayerDecor = new PlayerDecor(ItemType.None, new Vector3Data());
        State = State.Awake;
        GameOver = false;
        Position = new Vector3Data();
        Rotation = new QuaternionData();
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