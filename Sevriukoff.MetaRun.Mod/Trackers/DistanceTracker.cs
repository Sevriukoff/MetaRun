using System;
using System.Collections.Generic;
using System.Linq;
using On.RoR2;
using R2API.MiscHelpers;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Mod.Base;
using UnityEngine;
using EventType = Sevriukoff.MetaRun.Domain.Enum.EventType;
using NetworkUserId = RoR2.NetworkUserId;
using ProcChainMask = RoR2.ProcChainMask;
using Run = RoR2.Run;
using Util = RoR2.Util;
using Vector3 = UnityEngine.Vector3;

namespace Sevriukoff.MetaRun.Mod.Trackers;

public class DistanceTracker : BaseEventTracker
{
    private Dictionary<ulong, Vector3> _charactersLastPos;
    private Dictionary<ulong, Transform> _charactersTransform;

    private int _totalCharacterRun = 0;
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
        _totalCharacterRun = Run.instance.userMasters.Count;
        
        if (_charactersTransform.Count == _totalCharacterRun)
        {
            HealthComponent.Heal -= Initialization;
            return orig(self, amount, procChainMask, nonRegen);
        }
        
        if (self.body.isPlayerControlled && self.body.characterMotor.isGrounded)
        {
            var id = Util.LookUpBodyNetworkUser(self.body).id.steamId.steamValue;
            
            if (!_charactersTransform.ContainsKey(id))
                _charactersTransform.Add(id, self.body.coreTransform);
        }
        
        return orig(self, amount, procChainMask, nonRegen);
    }
    
    private void Run_Update(On.RoR2.Run.orig_Update orig, Run self)
    {
        foreach (var (player, currentTransform) in _charactersTransform)
        {
            var currentPos = currentTransform.position;
            
            if (!_charactersLastPos.ContainsKey(player))
                _charactersLastPos.Add(player, currentPos);

            var lastPos = _charactersLastPos[player];
            var distance = Vector3.Distance(lastPos, currentPos);
            
            _charactersLastPos[player] = currentPos;

            var eventMetadata = new EventMetaData(EventType.CharacterMove, TimeSpan.FromSeconds(self.time),
                self.GetUniqueId())
            {
                PlayerId = player,
                Data = new MoveEvent
                {
                    Distance = distance,
                    PrevPos = new Domain.Base.Vector3(lastPos.x, lastPos.y, lastPos.z),
                    CurrentPos = new Domain.Base.Vector3(currentPos.x, currentPos.y, currentPos.z)
                }
            };
            
            //OnEventProcessed(eventMetadata);
        }
    }
}