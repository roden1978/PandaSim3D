using System;
using GameObjectsScripts.Timers;

[Serializable]
public class TimerData
{
    public TimerType Type;
    public float Duration;
    public float StartTimerTimeInSeconds;
    public float EndTimerTimeInSeconds;
    public float CurrentTime;
    public float UpdateTime;
    public float IndicatorValue;
    public bool Active;
    public bool CanStart;
}