using System;
using System.Collections.Generic;
using On.RoR2;
using R2API.MiscHelpers;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Domain.Events.Character;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;
using UnityEngine;
using EventType = Sevriukoff.MetaRun.Domain.Enum.EventType;
using ProcChainMask = RoR2.ProcChainMask;
using Util = RoR2.Util;
using Vector3 = UnityEngine.Vector3;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterMovedTracker : BaseEventTracker
{
    private Dictionary<ulong, Vector3> _charactersLastPos;
    private Dictionary<ulong, Transform> _charactersTransform;

    private int _totalCharacterRun = 0;

    public CharacterMovedTracker()
    {
        SupportedEvent = new Dictionary<EventType, bool>()
        {
            {EventType.CharacterMoved, true}
        };
    }
    
    public override void StartProcessing()
    {
        _charactersTransform = new Dictionary<ulong, Transform>(4);
        _charactersLastPos = new Dictionary<ulong, Vector3>(4);
        
        HealthComponent.Heal += Initialization;
        On.RoR2.Run.Update += Run_Update;
    }
    
    public override void StopProcessing()
    {
        HealthComponent.Heal -= Initialization;
        On.RoR2.Run.Update -= Run_Update;
        
        _charactersTransform = null;
        _charactersLastPos = null;
    }
    
    private float Initialization(HealthComponent.orig_Heal orig, RoR2.HealthComponent self,
        float amount, ProcChainMask procChainMask, bool nonRegen)
    {
        _totalCharacterRun = RoR2.Run.instance.userMasters.Count;
        
        if (_charactersTransform.Count == _totalCharacterRun)
        {
            HealthComponent.Heal -= Initialization;
            return orig(self, amount, procChainMask, nonRegen);
        }
        
        if (self.body.isPlayerControlled && !self.body.characterMotor.lastGroundedTime.isInfinity)
        {
            var id = Util.LookUpBodyNetworkUser(self.body).id.steamId.steamValue;
            
            if (!_charactersTransform.ContainsKey(id))
                _charactersTransform.Add(id, self.body.coreTransform);
        }
        
        return orig(self, amount, procChainMask, nonRegen);
    }
    
    private void Run_Update(On.RoR2.Run.orig_Update orig, RoR2.Run self)
    {
        foreach (var (player, currentTransform) in _charactersTransform)
        {
            var currentPos = currentTransform.position; // TODO: FIX after first stage appears the NullReferenceException
            
            if (!_charactersLastPos.ContainsKey(player))
                _charactersLastPos.Add(player, currentPos);

            var lastPos = _charactersLastPos[player];
            var distance = Vector3.Distance(lastPos, currentPos);

            if (distance == 0)
                return;

            _charactersLastPos[player] = currentPos;
            
            CreateEventMetaData
            (
                EventType.CharacterMoved,
                new CharacterMovedEvent
                {
                    Distance = distance,
                    PrevPos = new Domain.Base.Vector3(lastPos.x, lastPos.y, lastPos.z),
                    CurrentPos = new Domain.Base.Vector3(currentPos.x, currentPos.y, currentPos.z)
                },
                player
            );
        }
    }
}