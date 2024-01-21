using System;
using System.Collections.Generic;

[Serializable]
public class TimersData
{
    public List<TimerData> Timers;

    public TimersData(List<TimerData> timersData)
    {
        Timers = timersData;
    }

    public void Clear()
    {
        Timers.Clear();
    }
}