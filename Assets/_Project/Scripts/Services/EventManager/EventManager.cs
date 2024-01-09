using System;
using System.Collections.Generic;
using UnityEngine.Events;


internal static class EventManager
{
    private static readonly Dictionary<Type, UnityAction<GameEvent>> Events = new Dictionary<Type, UnityAction<GameEvent>>();

    private static readonly Dictionary<Delegate, UnityAction<GameEvent>> EventLookups =
        new Dictionary<Delegate, UnityAction<GameEvent>>();

    internal static void Subscribe<T>(UnityAction<T> evt) where T : GameEvent
    {
        if (!EventLookups.ContainsKey(evt))
        {
            void NewAction(GameEvent e) => evt((T) e);
                
            EventLookups[evt] = NewAction;

            if (Events.ContainsKey(typeof(T)))
                Events[typeof(T)] += NewAction;
            else
                Events[typeof(T)] = NewAction;
        }
    }

    internal static void UnSubscribe<T>(UnityAction<T> evt) where T : GameEvent
    {
        if (EventLookups.TryGetValue(evt, out var action))
        {
            if (Events.TryGetValue(typeof(T), out var tempAction))
            {
                tempAction -= action;
                if (tempAction == null)
                    Events.Remove(typeof(T));
                else
                    Events[typeof(T)] = tempAction;
            }

            EventLookups.Remove(evt);
        }
    }

    internal static void Broadcast(GameEvent evt)
    {
        if (Events.TryGetValue(evt.GetType(), out var action))
            action.Invoke(evt);
    }

    internal static void Clear()
    {
        Events.Clear();
        EventLookups.Clear();
    }
}