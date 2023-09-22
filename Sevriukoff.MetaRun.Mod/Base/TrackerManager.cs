using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using R2API.MiscHelpers;
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
    private readonly Dictionary<string, EventMetaData> _trackerEventBySummationKey;
    private readonly Dictionary<Type, List<EventMetaData>> _trakcerEventByTrackerType;
    private readonly Dictionary<Type, DateTime> _lastTimeBufferClear;

    public event Action<EventMetaData> OnEventTracked; 

    public TrackerManager()
    {
        Type[] types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseEventTracker)))
            .ToArray();

        _activeTrackers = new List<BaseEventTracker>(types.Length);
        _trackersOptions = new Dictionary<Type, TrackerOptions>(types.Length);
        _lastTimeBufferClear = new Dictionary<Type, DateTime>(types.Length);
        _trakcerEventByTrackerType = new Dictionary<Type, List<EventMetaData>>(types.Length);

        var dateTimeNow = DateTime.Now;

        foreach (var type in types)
        {
            _trackersOptions.Add(type, TrackerOptions.CreateDefault());
            _lastTimeBufferClear.Add(type, dateTimeNow);
            _trakcerEventByTrackerType.Add(type, new List<EventMetaData>());
        }

        _trackerEventBySummationKey = new Dictionary<string, EventMetaData>(types.Length * 2);
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
                activeTracker.EventProcessed -= EventTrackedInternal;
                //activeTracker.EventProcessed -= trackerOptions.OnEventTracked ?? EventTrackedInternal;
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
        tracker.EventProcessed += EventTrackedInternal;
        //tracker.EventProcessed += opt.OnEventTracked ?? EventTrackedInternal;

        return tracker;
    }

    private void EventTrackedInternal(EventMetaData trackedEvent, Type trackerType)
    {
        if (!_trackersOptions.ContainsKey(trackerType))
            return;

        var trackerOption = _trackersOptions[trackerType];

        if (trackerOption.MaxEventSummation == 0)
        {
            OnEventTrackedInternal(trackedEvent);
            return;
        }
        
        var dateTimeNow = DateTime.Now;

        string eventKey = trackedEvent.GetSummationKey();

        if (_trackerEventBySummationKey.ContainsKey(eventKey))
        {
            var storedEvent = _trackerEventBySummationKey[eventKey];
            storedEvent.Add(trackedEvent);
            
            if (storedEvent.CountSummations >= trackerOption.MaxEventSummation)
            {
                OnEventTrackedInternal(storedEvent);
                
                _trackerEventBySummationKey.Remove(eventKey);
                _trakcerEventByTrackerType[trackerType].Remove(storedEvent);
            }
        }
        else
        {
            _trackerEventBySummationKey.Add(eventKey, trackedEvent);
            _trakcerEventByTrackerType[trackerType].Add(trackedEvent);
        }

        if ((dateTimeNow - _lastTimeBufferClear[trackerType]).TotalMilliseconds > trackerOption.LingerMs)
        {
            var listEventsToSend = _trakcerEventByTrackerType[trackerType];
            
            listEventsToSend.ForEach(x =>
            {
                OnEventTrackedInternal(x);
                
                _trackerEventBySummationKey.Remove(x.GetSummationKey());
            });
            
            listEventsToSend.Clear();
            
            _lastTimeBufferClear[trackerType] = DateTime.Now;
        }

        void OnEventTrackedInternal(EventMetaData eventMetaData)
        {
            if (trackerOption.OnEventTracked != null)
                trackerOption.OnEventTracked(eventMetaData);
            else
                OnEventTracked?.Invoke(eventMetaData);
        }
    }
}