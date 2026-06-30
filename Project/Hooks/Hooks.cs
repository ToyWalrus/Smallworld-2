using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Smallworld.Hooks;

public interface IHooks
{
    void Once<T>(Func<T, Task> handler) where T : IHook;
    void Subscribe<T>(Func<T, Task> handler) where T : IHook;
    void Unsubscribe<T>(Func<T, Task> handler) where T : IHook;
    Task Run<T>(T @event) where T : IHook;
}

public class Hooks : IHooks
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Subscribe<T>(Func<T, Task> handler) where T : IHook
    {
        if (!_subscribers.ContainsKey(typeof(T)))
        {
            _subscribers[typeof(T)] = new List<Delegate>();
        }

        _subscribers[typeof(T)].Add(handler);
    }

    public void Unsubscribe<T>(Func<T, Task> handler) where T : IHook
    {
        if (_subscribers.ContainsKey(typeof(T)))
        {
            _subscribers[typeof(T)].Remove(handler);
        }
    }

    public void Once<T>(Func<T, Task> handler) where T : IHook
    {
        async Task Wrapper(T @event)
        {
            await handler(@event);
            Unsubscribe<T>(Wrapper);
        }

        Subscribe<T>(Wrapper);
    }

    public async Task Run<T>(T @event) where T : IHook
    {
        if (_subscribers.ContainsKey(typeof(T)))
        {
            var subscriberList = new List<Delegate>(_subscribers[typeof(T)]);
            foreach (var subscriber in subscriberList)
            {
                await ((Func<T, Task>)subscriber).Invoke(@event);
            }
        }
    }
}