using System;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Entities;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Domain.Events.Character;
using Sevriukoff.MetaRun.Mod.Base;
using CharacterBody = RoR2.CharacterBody;
using DamageInfo = RoR2.DamageInfo;
using DamageType = Sevriukoff.MetaRun.Domain.Enum.DamageType;
using EventType = Sevriukoff.MetaRun.Domain.Enum.EventType;
using HealthComponent = On.RoR2.HealthComponent;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterMinionDamageTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        HealthComponent.TakeDamage += MinionDamage;
    }

    public override void StopProcessing()
    {
        HealthComponent.TakeDamage -= MinionDamage;
    }
    
    private void MinionDamage(HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, DamageInfo damageInfo)
    {
        orig(self, damageInfo);

        var attacker = damageInfo.attacker;

        if (attacker == null)
            return;

        var currentRun = RoR2.Run.instance;
        var currentDamage = damageInfo.crit ? damageInfo.damage * 2 : damageInfo.damage;
        var attackerCharacterBody = attacker.GetComponent<CharacterBody>();
        var playerCharacterMaster = attackerCharacterBody.master.minionOwnership.ownerMaster;
        var enemyCharacterBody = self.body;
        var eventType = EventType.CharacterMinionDealtDamage;

        if (playerCharacterMaster == null)
        {
            playerCharacterMaster = self.body.master.minionOwnership.ownerMaster;
            enemyCharacterBody = attacker.GetComponent<CharacterBody>();
            eventType = EventType.CharacterMinionTookDamage;

            if (playerCharacterMaster == null)
                return;
        }
        
        var playerId = playerCharacterMaster.playerCharacterMasterController.networkUser.id.steamId.steamValue;
        string inflictorName = string.Empty;

        if (damageInfo.procChainMask.mask > 0)
            inflictorName = damageInfo.procChainMask.ToString();
        else if (damageInfo.inflictor != null)
            inflictorName = damageInfo.inflictor.name;
        
        var eventMetaData = new EventMetaData(eventType, TimeSpan.FromSeconds(currentRun.GetRunStopwatch()),
            currentRun.GetUniqueId(), playerId)
        {
            Data = new CharacterMinionDamageEvent
            {
                Damage = currentDamage,
                DamageType = (DamageType) damageInfo.damageType,
                DotType = (DotIndex) damageInfo.dotIndex,
                IsCrit = damageInfo.crit,
                IsRejected = damageInfo.rejected,
                Inflictor = inflictorName,
                Enemy = new Monster(enemyCharacterBody.name, enemyCharacterBody.isElite, enemyCharacterBody.isBoss,
                    (TeamType) (sbyte) enemyCharacterBody.teamComponent.teamIndex)
            }
        };
        
        OnEventProcessed(eventMetaData);
    }
}