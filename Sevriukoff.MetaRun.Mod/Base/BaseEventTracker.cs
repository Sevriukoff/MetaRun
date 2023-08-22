using System;

namespace Sevriukoff.MetaRun.Mod.Base;

public abstract class BaseEventTracker
{
    public event Action<object> EventProcessed; 

    public abstract void StartProcessing();

    public abstract void StopProcessing();

    protected virtual void OnEventProcessed(object obj)
    {
        EventProcessed?.Invoke(obj);
    }
}