using System;
using System.Collections.Generic;

namespace Smallworld.Events;

public interface IEventAggregator
{
    void Subscribe<T>(Action<T> handler) where T : IEvent;
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;
    void Publish<T>(T @event) where T : IEvent;
}

public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, List<Delegate>> _subscribers = new();

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        if (!_subscribers.ContainsKey(typeof(T)))
        {
            _subscribers[typeof(T)] = new List<Delegate>();
        }

        _subscribers[typeof(T)].Add(handler);
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        if (_subscribers.ContainsKey(typeof(T)))
        {
            _subscribers[typeof(T)].Remove(handler);
        }
    }

    public void Publish<T>(T @event) where T : IEvent
    {
        if (_subscribers.ContainsKey(typeof(T)))
        {
            var subscriberList =  new List<Delegate>(_subscribers[typeof(T)]);
            foreach (var subscriber in subscriberList)
            {
                ((Action<T>)subscriber).Invoke(@event);
            }
        }
    }
}