using System;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod.Base;

public abstract class BaseEventTracker
{
    public event Action<EventMetaData> EventProcessed; 

    public abstract void StartProcessing();

    public abstract void StopProcessing();

    protected virtual void OnEventProcessed(EventMetaData obj)
    {
        EventProcessed?.Invoke(obj);
    }
}