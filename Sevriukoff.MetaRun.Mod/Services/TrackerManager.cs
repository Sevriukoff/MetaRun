using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using R2API.MiscHelpers;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Base.Interfaces;

namespace Sevriukoff.MetaRun.Mod.Services;

/// <summary>
/// Класс, управляющий всеми трекерами игровых событий. Менеджер может конфигурировать трекеры прямо в забеге.
/// Но лучше настроить все трекеры до начала забега.
/// </summary>
public class TrackerManager
{
    private bool _isTrackersWorking;
    private readonly List<BaseEventTracker> _activeTrackers;
    private readonly Dictionary<Type, BaseTrackerOptions> _trackersOptions;
    private readonly Dictionary<string, EventMetaData> _trackerEventBySummationKey;
    private readonly Dictionary<Type, List<EventMetaData>> _trackerEventByTrackerType;
    private readonly Dictionary<Type, DateTime> _lastTimeBufferClear;
    
    private readonly ConfigManager _configManager;

    public event Action<EventMetaData> OnEventTracked; 

    public TrackerManager(ConfigManager configManager)
    {
        Type[] trackerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseEventTracker)))
            .ToArray();

        _configManager = configManager;
        
        _activeTrackers = new List<BaseEventTracker>(trackerTypes.Length);
        _trackersOptions = new Dictionary<Type, BaseTrackerOptions>(trackerTypes.Length);
        _lastTimeBufferClear = new Dictionary<Type, DateTime>(trackerTypes.Length);
        _trackerEventByTrackerType = new Dictionary<Type, List<EventMetaData>>(trackerTypes.Length);

        var dateTimeNow = DateTime.Now;

        foreach (var type in trackerTypes)
        {
            var trackerInterfaceOptionType = typeof(IOptionsFor<>).MakeGenericType(type);
            var trackerOptionType = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(x => x.IsClass && trackerInterfaceOptionType.IsAssignableFrom(x));

            var trackerOption = trackerOptionType == null
                ? new BaseTrackerOptions(type)
                : (BaseTrackerOptions) Activator.CreateInstance(trackerOptionType);

            configManager.ConfigureGroup(trackerOption, !type.Name.Contains("Run"));
            
            _trackersOptions.Add(type, trackerOption);
            _lastTimeBufferClear.Add(type, dateTimeNow);
            _trackerEventByTrackerType.Add(type, new List<EventMetaData>());

            trackerOption.IsActive.OptionChange += value => ChangeTrackerStatus(type, value);
        }

        _trackerEventBySummationKey = new Dictionary<string, EventMetaData>(trackerTypes.Length * 2);
    }

    public void StartTracking()
    {
        foreach (var (type, opt) in _trackersOptions)
        {
            if (!opt.IsActive.Value)
                continue;
            
            var tracker = CreateTracker(type, opt);
            _trackersOptions[type].ConfiguredObject = tracker;
            _activeTrackers.Add(tracker);
        }

        _isTrackersWorking = true;
    }

    public void StopTracking()
    {
        _activeTrackers.ForEach(x => DeleteTracker(ref x));
        _activeTrackers.Clear();

        _isTrackersWorking = false;
    }
    private void ChangeTrackerStatus(Type type, bool isActive)
    {
        if (!_isTrackersWorking)
            return;
        
        if (isActive)
        {
            var tracker = CreateTracker(type, _trackersOptions[type]);
            _activeTrackers.Add(tracker);
        }
        else
        {
            var activeTracker = _activeTrackers.First(x => x.GetType() == type);

            activeTracker.StopProcessing();
            activeTracker.EventProcessed -= EventTrackedInternal;
            //activeTracker.EventProcessed -= trackerOptions.OnEventTracked ?? EventTrackedInternal;
            _activeTrackers.Remove(activeTracker);
        }
    }

    private BaseEventTracker CreateTracker(Type type, BaseTrackerOptions opt)
    {
        var tracker = (BaseEventTracker)Activator.CreateInstance(type);
        tracker.StartProcessing();
        tracker.EventProcessed += EventTrackedInternal;
        //tracker.EventProcessed += opt.OnEventTracked ?? EventTrackedInternal;

        return tracker;
    }

    private void DeleteTracker(ref BaseEventTracker tracker)
    {
        tracker.StopProcessing();
        tracker.EventProcessed -= EventTrackedInternal;
        tracker = null;
    }

    private void EventTrackedInternal(EventMetaData trackedEvent, Type trackerType)
    {
        if (!_trackersOptions.TryGetValue(trackerType, out var trackerOption))
            return;
        
        var dateTimeNow = DateTime.Now;
        var (maxEventSummation, lingerMs) = (trackerOption.MaxEventSummation.Value, trackerOption.LingerMs.Value);
        string eventKey = trackedEvent.GetSummationKey();

        if (maxEventSummation > 0)
        {
            if (_trackerEventBySummationKey.ContainsKey(eventKey))
            {
                var storedEvent = _trackerEventBySummationKey[eventKey];
                storedEvent.Add(trackedEvent);

                if (storedEvent.CountSummations >= maxEventSummation)
                {
                    _trackerEventBySummationKey.Remove(eventKey);
                    _trackerEventByTrackerType[trackerType].Remove(storedEvent);
                
                    OnEventTracked?.Invoke(storedEvent);
                }
            }
            else
            {
                _trackerEventBySummationKey.Add(eventKey, trackedEvent);
                _trackerEventByTrackerType[trackerType].Add(trackedEvent);
            }
        }
        else
        {
            _trackerEventByTrackerType[trackerType].Add(trackedEvent);
        }

        if (lingerMs > 0)
        {
            if ((dateTimeNow - _lastTimeBufferClear[trackerType]).TotalMilliseconds > lingerMs)
            {
                var listEventsToSend = _trackerEventByTrackerType[trackerType];
            
                listEventsToSend.ForEach(x =>
                {
                    OnEventTracked?.Invoke(x);
                
                    _trackerEventBySummationKey.Remove(x.GetSummationKey());
                });
            
                listEventsToSend.Clear();
            
                _lastTimeBufferClear[trackerType] = DateTime.Now;
            }
        }
    }
}