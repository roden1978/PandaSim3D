using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventBus : IEventBus
{
    private readonly Dictionary<string, List<CallbackWithPriority>> _signalCallbacks;

    public EventBus()
    {
        _signalCallbacks = new Dictionary<string, List<CallbackWithPriority>>();
    }

    public void Subscribe<T>(Action<T> callback, int priority = 0)
    {
        string key = typeof(T).Name;
        if (_signalCallbacks.TryGetValue(key, out List<CallbackWithPriority> signalCallback))
        {
            signalCallback.Add(new CallbackWithPriority(priority, callback));
        }
        else
        {
            _signalCallbacks.Add(key, new List<CallbackWithPriority>() { new(priority, callback) });
        }

        _signalCallbacks[key] = _signalCallbacks[key].OrderByDescending(x => x.Priority).ToList();
    }

    public void Invoke<T>(T signal)
    {
        string key = typeof(T).Name;
        if (_signalCallbacks.TryGetValue(key, out List<CallbackWithPriority> signalCallback))
        {
            foreach (CallbackWithPriority obj in signalCallback)
            {
                Action<T> callback = obj.Callback as Action<T>;
                callback?.Invoke(signal);
            }
        }
    }

    public void Unsubscribe<T>(Action<T> callback)
    {
        string key = typeof(T).Name;
        if (_signalCallbacks.ContainsKey(key))
        {
            CallbackWithPriority callbackToDelete =
                _signalCallbacks[key].FirstOrDefault(x => x.Callback.Equals(callback));

            if (callbackToDelete != null)
                _signalCallbacks[key].Remove(callbackToDelete);
        }
        else
        {
            Debug.LogErrorFormat("Trying to unsubscribe for not existing key! {0} ", key);
        }
    }
}