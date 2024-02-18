using System.Collections;
using System.Collections.Generic;
using GameObjectsScripts.Timers;

public class TimerSet : IEnumerable<Timer>
{
    private readonly List<Timer> _timers = new();


    public void AddTimer(Timer timer)
    {
        _timers.Add(timer);
    }

    public void RemoveTimer(Timer timer)
    {
        _timers.Remove(timer);
    }

    public IEnumerator<Timer> GetEnumerator()
    {
        return _timers.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}