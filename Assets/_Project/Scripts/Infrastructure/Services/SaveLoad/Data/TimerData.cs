using System;
using GameObjectsScripts.Timers;

[Serializable]
public class TimerData
{
    public TimerType Type;
    public int Duration;
    public int StartTimerTimeInSeconds;
    public int EndTimerTimeInSeconds;
    public float CurrentTime;
    public float UpdateTime;
    public float IndicatorValue;
    public bool Active;
}