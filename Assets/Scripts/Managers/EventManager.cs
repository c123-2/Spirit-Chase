using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// \u5168\u5c40\u4e8b\u4ef6\u7cfb\u7edf\uff0c\u57fa\u4e8e\u5b57\u7b26\u4e32+\u53c2\u6570\u7684\u4e8b\u4ef6\u603b\u7ebf\uff0c\u89e3\u8026\u6a21\u5757\u95f4\u901a\u4fe1\u3002
/// \u4efb\u52a1 1.3\uff1aEventManager \u4e8b\u4ef6\u7cfb\u7edf
/// </summary>
public class EventManager : Singleton<EventManager>
{
    public delegate void EventCallback(params object[] args);

    private readonly Dictionary<string, EventCallback> _events = new Dictionary<string, EventCallback>();
    private readonly Dictionary<string, EventCallback> _onceEvents = new Dictionary<string, EventCallback>();

    /// <summary>\u6ce8\u518c\u4e8b\u4ef6\u76d1\u542c</summary>
    public void On(string eventName, EventCallback callback)
    {
        if (_events.TryGetValue(eventName, out var existing))
            _events[eventName] = existing + callback;
        else
            _events[eventName] = callback;
    }

    /// <summary>\u53ea\u76d1\u542c\u4e00\u6b21\uff0c\u89e6\u53d1\u540e\u81ea\u52a8\u6ce8\u9500</summary>
    public void Once(string eventName, EventCallback callback)
    {
        if (_onceEvents.TryGetValue(eventName, out var existing))
            _onceEvents[eventName] = existing + callback;
        else
            _onceEvents[eventName] = callback;
    }

    /// <summary>\u79fb\u9664\u4e8b\u4ef6\u76d1\u542c</summary>
    public void Off(string eventName, EventCallback callback)
    {
        if (_events.TryGetValue(eventName, out var existing))
        {
            existing -= callback;
            if (existing == null)
                _events.Remove(eventName);
            else
                _events[eventName] = existing;
        }
    }

    /// <summary>\u89e6\u53d1\u4e8b\u4ef6</summary>
    public void Emit(string eventName, params object[] args)
    {
        if (_events.TryGetValue(eventName, out var callback))
            callback?.Invoke(args);

        if (_onceEvents.TryGetValue(eventName, out var onceCallback))
        {
            onceCallback?.Invoke(args);
            _onceEvents.Remove(eventName);
        }
    }

    /// <summary>\u6e05\u9664\u6240\u6709\u4e8b\u4ef6\u76d1\u542c</summary>
    public void Clear()
    {
        _events.Clear();
        _onceEvents.Clear();
    }

    protected override void OnDestroy()
    {
        Clear();
        base.OnDestroy();
    }
}
