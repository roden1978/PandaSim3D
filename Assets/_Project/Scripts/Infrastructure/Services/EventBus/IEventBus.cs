using System;

public interface IEventBus
{
    void Subscribe<T>(Action<T> callback, int priority = 0);
    void Unsubscribe<T>(Action<T> callback);
    void Invoke<T>(T signal);
}