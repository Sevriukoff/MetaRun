using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using R2API.MiscHelpers;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Mod.Base;

namespace Sevriukoff.MetaRun.Mod.Services;

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
    private readonly Dictionary<Type, List<EventMetaData>> _trackerEventByTrackerType;
    private readonly Dictionary<Type, DateTime> _lastTimeBufferClear;
    
    private readonly ConfigManager _configManager;

    public event Action<EventMetaData> OnEventTracked; 

    public TrackerManager(ConfigManager configManager)
    {
        Type[] types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseEventTracker)))
            .ToArray();

        _configManager = configManager;
        
        _activeTrackers = new List<BaseEventTracker>(types.Length);
        _trackersOptions = new Dictionary<Type, TrackerOptions>(types.Length);
        _lastTimeBufferClear = new Dictionary<Type, DateTime>(types.Length);
        _trackerEventByTrackerType = new Dictionary<Type, List<EventMetaData>>(types.Length);

        var dateTimeNow = DateTime.Now;

        foreach (var type in types)
        {
            var trackerOption = new TrackerOptions
            (
                new BindableOption<bool>(type.Name, "IsActive", true)
                {
                    Description = $"Можно выключить отслеживание событий типа: [{type.Name.Remove(type.Name.Length - 7)}]"
                },
                new BindableOption<float>(type.Name, "MaxEventSummation", 0f)
                {
                    Description = "Сколько событий могут быть объедены в одно за время хранения сообщений в буфере перед отправкой на сервер. Позволяет снизить нагрузку при большом количестве собыйтий в секунду. Особенно полезно для трекера передвижения"
                },
                new BindableOption<float>(type.Name, "LingerMs", 0f)
                {
                    Description = "Сколько милисекунд будут храниться события во внутреннем буфере трекера перед отправкой на сервер. Отдельно это параметр практически не имеет значения. Рекомендуется использовать вместе с параметром \"Кол-во событий для суммирования\""
                }
            );
            
            configManager.Configure<bool>(trackerOption.IsActive);
            configManager.Configure<float>(trackerOption.MaxEventSummation);
            configManager.Configure<float>(trackerOption.LingerMs);
            configManager.Configure(trackerOption.TrackOnlyHost);
            
            _trackersOptions.Add(type, trackerOption);
            _lastTimeBufferClear.Add(type, dateTimeNow);
            _trackerEventByTrackerType.Add(type, new List<EventMetaData>());
            
            trackerOption.IsActive.OptionChange += isActive =>
            {
                ChangeTrackerStatus(type, isActive);
            };
        }

        _trackerEventBySummationKey = new Dictionary<string, EventMetaData>(types.Length * 2);
    }

    public void StartTracking()
    {
        foreach (var (type, opt) in _trackersOptions)
        {
            if (!opt.IsActive.Value)
                continue;

            var tracker = CreateTracker(type, opt);
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

    private BaseEventTracker CreateTracker(Type type, TrackerOptions opt)
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
        if (!_trackersOptions.ContainsKey(trackerType))
            return;

        var trackerOption = _trackersOptions[trackerType];

        if (trackerOption.MaxEventSummation.Value <= 0)
        {
            OnEventTracked?.Invoke(trackedEvent);
            return;
        }
        
        var dateTimeNow = DateTime.Now;

        string eventKey = trackedEvent.GetSummationKey();

        if (_trackerEventBySummationKey.ContainsKey(eventKey))
        {
            var storedEvent = _trackerEventBySummationKey[eventKey];
            storedEvent.Add(trackedEvent);
            
            if (storedEvent.CountSummations >= trackerOption.MaxEventSummation.Value)
            {
                OnEventTracked?.Invoke(storedEvent);
                
                _trackerEventBySummationKey.Remove(eventKey);
                _trackerEventByTrackerType[trackerType].Remove(storedEvent);
            }
        }
        else
        {
            _trackerEventBySummationKey.Add(eventKey, trackedEvent);
            _trackerEventByTrackerType[trackerType].Add(trackedEvent);
        }

        if ((dateTimeNow - _lastTimeBufferClear[trackerType]).TotalMilliseconds > trackerOption.LingerMs.Value)
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