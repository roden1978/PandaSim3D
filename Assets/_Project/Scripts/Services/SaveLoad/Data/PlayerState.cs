using System;

[Serializable]
public class PlayerState
{
    public string SceneName;
    public int CurrentHealth;
    public int MaxHealth;
    public float Mood;
    public float Meal;
    public float Toilet;
    public float Dream;
    public bool FirstStartGame;

    public PlayerState()
    {
        FirstStartGame = true;
    }

    public void ResetHP() => CurrentHealth = MaxHealth;
}