using System;
using System.Collections.Generic;
using System.Linq;
using R2API.MiscHelpers;
using RoR2;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Entities;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Domain.Events.Character;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;
using CharacterBody = RoR2.CharacterBody;
using DamageInfo = RoR2.DamageInfo;
using DamageType = Sevriukoff.MetaRun.Domain.Enum.DamageType;
using HealthComponent = On.RoR2.HealthComponent;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterDamageTracker : BaseEventTracker
{
    private CharacterDamageEvent _damageEvent;
    private EventType _eventType;
    private uint _playerId;

    public override void StartProcessing()
    {
        HealthComponent.TakeDamage += HealthComponentOnTakeDamage;
        GlobalEventManager.onClientDamageNotified += OnCalculatedDamage;
    }

    public override void StopProcessing()
    {
        HealthComponent.TakeDamage -= HealthComponentOnTakeDamage;
        GlobalEventManager.onClientDamageNotified -= OnCalculatedDamage;
    }

    private void HealthComponentOnTakeDamage(HealthComponent.orig_TakeDamage orig,
        RoR2.HealthComponent self, DamageInfo damageInfo)
    {
        orig(self, damageInfo);
        
        var attacker = damageInfo.attacker;

        if (attacker == null)
            return;

        var playerCharacterBody = attacker.GetComponent<CharacterBody>();
        var enemyCharacterBody = self.body;
        var eventType = EventType.CharacterDealtDamage;
        var inflictorName = string.Empty;
        
        if (damageInfo.inflictor != null)
            inflictorName = damageInfo.inflictor.name;
        else if (damageInfo.procChainMask.mask > 0)
            inflictorName = damageInfo.procChainMask.ToString();

        if (!playerCharacterBody.isPlayerControlled)
        {
            eventType = EventType.CharacterTookDamage;
            playerCharacterBody = self.body;
            enemyCharacterBody = attacker.GetComponent<CharacterBody>();
        }

        if (!playerCharacterBody.isPlayerControlled)
            return;

        _eventType = eventType;
        _playerId = playerCharacterBody.master.netId.Value;

        _damageEvent = new CharacterDamageEvent
        {
            DamageType = (DamageType) (uint) damageInfo.damageType,
            DotType = (DotIndex) (int) damageInfo.dotIndex,
            IsCrit = damageInfo.crit,
            IsRejected = damageInfo.rejected,
            Inflictor = inflictorName,
            Enemy = new Monster(enemyCharacterBody.netId.Value, enemyCharacterBody.name, enemyCharacterBody.isBoss,
                enemyCharacterBody.isElite, (TeamType) (sbyte) enemyCharacterBody.teamComponent.teamIndex)
        };
    }
    
    private void OnCalculatedDamage(DamageDealtMessage obj)
    {
        if (_damageEvent == null)
            return;
        
        _damageEvent.Damage = obj.damage;

        CreateEventMetaData(_eventType, _damageEvent, _playerId);
    }
}

