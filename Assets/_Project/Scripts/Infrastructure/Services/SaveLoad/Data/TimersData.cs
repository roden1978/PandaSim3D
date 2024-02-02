using System;
using System.Collections.Generic;

[Serializable]
public class TimersData
{
    public List<TimerData> Timers;
    public float MoodIndicatorValue;
    public TimersData(List<TimerData> timersData)
    {
        Timers = timersData;
        MoodIndicatorValue = 1;
    }

    public void Clear()
    {
        Timers.Clear();
    }
}