using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using HealthComponent = On.RoR2.HealthComponent;

namespace Sevriukoff.MetaRun.Mod.Utils;

public static class EventMetaDataUtil
{
    private static Run _run;
    private static Guid _cachedRunId;
    private static ulong _cachedPlayerHostId;

    private static Dictionary<uint, ulong> _cachedPlayersId;

    private static bool _isInit;

    static EventMetaDataUtil()
    {
        On.RoR2.Run.Start += InitRun;
    }

    private static void InitRun(On.RoR2.Run.orig_Start orig, Run self)
    {
        _run = self;
        _cachedRunId = self.GetUniqueId();

        HealthComponent.Heal += InitHost;
        
        static float InitHost(HealthComponent.orig_Heal orig, RoR2.HealthComponent self, float amount,
            ProcChainMask procChainMask, bool nonRegen)
        {
            _cachedPlayerHostId = _run.userMasters.First(x => x.Value.isServer)
                .Key.steamId.steamValue;

            _cachedPlayersId = _run.userMasters.ToDictionary(x => x.Value.netId.Value,
                x => x.Key.steamId.steamValue); //TODO: Think about the key.

            HealthComponent.Heal -= InitHost;

            return orig(self, amount, procChainMask, nonRegen);
        }
        
        orig(self);
    }

    public static void Init()
    {
        _isInit = true;
    }

    public static EventMetaData CreateEvent(EventType eventType, uint characterMasterNetId = 0) =>
        CreateEventInternal(eventType, null, characterMasterNetId, 0);

    public static EventMetaData CreateEvent(EventType eventType, IEventData eventData, uint characterMasterNetId = 0) =>
        CreateEventInternal(eventType, eventData, characterMasterNetId, 0);
    
    public static EventMetaData CreateEvent(EventType eventType, IEventData eventData, ulong playerId) =>
        CreateEventInternal(eventType, eventData,0, playerId);

    private static EventMetaData CreateEventInternal(EventType eventType, IEventData eventData, uint characterMasterNetId,
        ulong playerId)
    {
        ulong pId;
        
        if (characterMasterNetId > 0)
            _cachedPlayersId.TryGetValue(characterMasterNetId, out pId);
        else
            pId = playerId;
        
        return new EventMetaData
        (
            eventType,
            TimeSpan.FromSeconds(_run.GetRunStopwatch()),
            _cachedRunId,
            pId == 0 ? _cachedPlayerHostId : pId
        )
        {
            Data = eventData
        };
    }
}