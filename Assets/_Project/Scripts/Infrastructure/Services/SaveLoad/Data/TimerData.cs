using System;
using GameObjectsScripts.Timers;
using UnityEngine.Serialization;

[Serializable]
public class TimerData
{
    public TimerType Type = TimerType.None;
    public TimerState State = TimerState.Countdown;
    public float StartTimerTimeInSeconds;
    public float EndTimerTimeInSeconds;
    public float CurrentTime;
    public float UpdateTime;
    public float IndicatorValue;
    public bool Active;
    public bool AwakeStart;
}