using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod.Base;

public class TrackerContainer
{
    private readonly List<BaseEventTracker> _trackers = new();

    public event Action<EventMetaData> OnEventTracked; 

    public TrackerContainer()
    {
        Type[] types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseEventTracker)))
            .ToArray();

        foreach (var type in types)
        {
            _trackers.Add((BaseEventTracker)Activator.CreateInstance(type));
        }
        
        _trackers.ForEach(x => x.EventProcessed += EventTracked);
    }

    public void StartTracking()
    {
        _trackers.ForEach(x => x.StartProcessing());
    }

    public void StopTracking()
    {
        _trackers.ForEach(x => x.StopProcessing());
    }

    private void EventTracked(EventMetaData trackedEvent)
    {
        OnEventTracked?.Invoke(trackedEvent);
    }
}