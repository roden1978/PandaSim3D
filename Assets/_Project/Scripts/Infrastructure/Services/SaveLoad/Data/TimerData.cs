using System;
using System.Collections.Generic;
using GameObjectsScripts.Timers;
using UnityEngine.Serialization;
using Timer = Unity.VisualScripting.Timer;

[Serializable]
public class TimerData
{
    public string Name = string.Empty;
    public TimerType Type = TimerType.None;
    public TimerState State = TimerState.Countdown;
    public float StartTimerTimeInSeconds;
    public float EndTimerTimeInSeconds;
    public float CurrentTime;
    public float UpdateTime;
    public float IndicatorValue;
    public bool Active;
    public List<TimerRoles> TimerRolesList;
}