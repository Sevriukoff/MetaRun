using System;
using On.RoR2;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Entities;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Domain.Events.Character;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;
using CharacterBody = RoR2.CharacterBody;
using DamageDealtMessage = RoR2.DamageDealtMessage;
using DamageInfo = RoR2.DamageInfo;
using DamageType = Sevriukoff.MetaRun.Domain.Enum.DamageType;
using EventType = Sevriukoff.MetaRun.Domain.Enum.EventType;
using HealthComponent = On.RoR2.HealthComponent;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterMinionDamageTracker : BaseEventTracker
{
    private CharacterMinionDamageEvent _damageEvent;
    private EventType _eventType;
    private ulong _playerId;
    
    public override void StartProcessing()
    {
        HealthComponent.TakeDamage += MinionDamage;
        RoR2.GlobalEventManager.onClientDamageNotified += OnCalculatedDamage;
    }
    public override void StopProcessing()
    {
        HealthComponent.TakeDamage -= MinionDamage;
        RoR2.GlobalEventManager.onClientDamageNotified -= OnCalculatedDamage;
    }
    
    private void MinionDamage(HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, DamageInfo damageInfo)
    {
        orig(self, damageInfo);

        var attacker = damageInfo.attacker;

        if (attacker == null)
            return;

        var attackerCharacterBody = attacker.GetComponent<CharacterBody>();
        var playerCharacterMaster = attackerCharacterBody.master.minionOwnership.ownerMaster;
        var enemyCharacterBody = self.body;
        _eventType = EventType.CharacterMinionDealtDamage;

        if (playerCharacterMaster == null)
        {
            playerCharacterMaster = self.body.master.minionOwnership.ownerMaster;
            enemyCharacterBody = attacker.GetComponent<CharacterBody>();
            _eventType = EventType.CharacterMinionTookDamage;

            if (playerCharacterMaster == null)
                return;
        }
        
        _playerId = playerCharacterMaster.playerCharacterMasterController.networkUser.id.steamId.steamValue;
        string inflictorName = string.Empty;

        if (damageInfo.procChainMask.mask > 0)
            inflictorName = damageInfo.procChainMask.ToString();
        else if (damageInfo.inflictor != null)
            inflictorName = damageInfo.inflictor.name;

        _damageEvent = new CharacterMinionDamageEvent
        {
            DamageType = (DamageType) damageInfo.damageType,
            DotType = (DotIndex) damageInfo.dotIndex,
            IsCrit = damageInfo.crit,
            IsRejected = damageInfo.rejected,
            Inflictor = inflictorName,
            Enemy = new Monster(enemyCharacterBody.netId.Value, enemyCharacterBody.name, enemyCharacterBody.isElite,
                enemyCharacterBody.isBoss, (TeamType) (sbyte) enemyCharacterBody.teamComponent.teamIndex)
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