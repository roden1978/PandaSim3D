using System;
using System.Collections.Generic;
using System.Linq;
using GameObjectsScripts.Timers;

[Serializable]
public class TimersData
{
    public List<TimerData> Timers;
    public float MoodIndicatorValue;
    public double CurrentWorldTimeInSeconds;
    public TimersData(List<TimerData> timersData)
    {
        Timers = timersData;
        MoodIndicatorValue = 1;
        CurrentWorldTimeInSeconds = 0;
    }

    public void Clear()
    {
        Timers.Clear();
    }

    public TimerData GetTimerDataByTimerType(TimerType type)
    {
        return Timers.FirstOrDefault(x => x.Type == type);
    }
}