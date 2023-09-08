using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using R2API.MiscHelpers;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod.Base;

/// <summary>
/// Класс, управляющий всеми трекерами игровых событий. Менеджер может конфигурировать трекеры прямо в забеге.
/// Но лучше настроить все трекеры до начала забега.
/// </summary>
public class TrackerManager
{
    private bool _isTrackersWorking;
    private readonly List<BaseEventTracker> _activeTrackers;
    private readonly Dictionary<Type, TrackerOptions> _trackersOptions;

    public event Action<EventMetaData> OnEventTracked; 

    public TrackerManager()
    {
        Type[] types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseEventTracker)))
            .ToArray();

        _activeTrackers = new List<BaseEventTracker>(types.Length);
        _trackersOptions = new Dictionary<Type, TrackerOptions>(types.Length);

        foreach (var type in types)
        {
            _trackersOptions.Add(type, new TrackerOptions{IsActive = true, Priority = "standard", TrackOnlyHost = false});
        }
    }

    public void StartTracking()
    {
        foreach (var (type, opt) in _trackersOptions)
        {
            if (!opt.IsActive)
                continue;

            var tracker = CreateTracker(type, opt);
            _activeTrackers.Add(tracker);
        }

        _isTrackersWorking = true;
    }

    public void StopTracking()
    {
        _activeTrackers.ForEach(x => x.StopProcessing());

        _isTrackersWorking = false;
    }

    public bool ConfigureTracker(string name, TrackerOptions options)
    {
        if (string.IsNullOrEmpty(name))
            return false;
        
        if (!name.Contains("Tracker"))
            name = name.Insert(name.Length - 1, "Tracker");

        try
        {
            var trackerType = Type.GetType(name);
            return ConfigureTrackerInternal(trackerType, options);
        }
        catch (Exception e)
        {
            return false;
        }
    }
    
    public bool ConfigureTracker(Type type, TrackerOptions options)
    {
        return ConfigureTrackerInternal(type, options);
    }
    
    public bool ConfigureTracker<T>(TrackerOptions options) where T : BaseEventTracker
    {
        var type = typeof(T);
        
        return ConfigureTrackerInternal(type, options);
    }

    private bool ConfigureTrackerInternal(Type type, TrackerOptions opt)
    {
        if (opt == null || !_trackersOptions.ContainsKey(type))
            return false;

        if (_isTrackersWorking)
        {
            if (opt.IsActive)
            {
                var tracker = CreateTracker(type, opt);
                _activeTrackers.Add(tracker);
            }
            else
            {
                var trackerOptions = _trackersOptions[type];
                _trackersOptions[type] = opt;
                
                if (!trackerOptions.IsActive)
                    return false;
                
                var activeTracker = _activeTrackers.First(x => x.GetType() == type);

                activeTracker.StopProcessing();
                activeTracker.EventProcessed -= trackerOptions.OnEventTracked ?? EventTracked;
                _activeTrackers.Remove(activeTracker);
            }
        }
        else
        {
            _trackersOptions[type] = opt;
        }
        
        
        return true;
    }

    private BaseEventTracker CreateTracker(Type type, TrackerOptions opt)
    {
        var tracker = (BaseEventTracker)Activator.CreateInstance(type);
        tracker.StartProcessing();
        tracker.EventProcessed += opt.OnEventTracked ?? EventTracked;

        return tracker;
    }

    private void EventTracked(EventMetaData trackedEvent)
    {
        OnEventTracked?.Invoke(trackedEvent);
    }
}