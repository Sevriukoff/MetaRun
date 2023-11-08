using System;
using System.Collections.Generic;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Mod.Utils;

namespace Sevriukoff.MetaRun.Mod.Base;

public abstract class BaseEventTracker
{
    public event Action<EventMetaData, Type> EventProcessed; //TODO: Is property Type necessary?
    protected Dictionary<EventType, bool> SupportedEvent { get; init; }

    public abstract void StartProcessing();

    public abstract void StopProcessing();

    protected void CreateEventMetaData(EventType type, IEventData data, ulong playerId = 0)
    {
        var eventMetaData = EventMetaDataUtil.CreateEvent(type, data, playerId);
        
        EventProcessed?.Invoke(eventMetaData, GetType());
    }
    
    protected void CreateEventMetaData(EventType type)
    {
        var eventMetaData = EventMetaDataUtil.CreateEvent(type);
        
        EventProcessed?.Invoke(eventMetaData, GetType());
    }

    public virtual void ChangeSupportedEvent(EventType eventType, bool value)
    {
        SupportedEvent[eventType] = value;
    }
}