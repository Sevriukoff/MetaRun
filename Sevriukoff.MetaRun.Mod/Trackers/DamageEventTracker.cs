using System;
using RoR2;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Mod.Base;
using CharacterBody = RoR2.CharacterBody;
using DamageInfo = RoR2.DamageInfo;
using DamageType = Sevriukoff.MetaRun.Domain.Enum.DamageType;
using HealthComponent = On.RoR2.HealthComponent;
using Run = RoR2.Run;

namespace Sevriukoff.MetaRun.Mod.Trackers;

public class DamageEventTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        HealthComponent.TakeDamage += HealthComponentOnTakeDamage;
    }

    public override void StopProcessing()
    {
        HealthComponent.TakeDamage -= HealthComponentOnTakeDamage;
    }
    
    private void HealthComponentOnTakeDamage(HealthComponent.orig_TakeDamage orig,
        RoR2.HealthComponent self, DamageInfo damageInfo)
    {
        try
        {
            var attacker = damageInfo.attacker;
            var attackerCharacterBody = attacker.GetComponent<CharacterBody>();
            
            var currentDamage = damageInfo.crit ? damageInfo.damage * 2 : damageInfo.damage;
            var currentRun = Run.instance;
            var runTime = TimeSpan.FromSeconds(currentRun.time);

            EventMetaData eventMetadata = null;
            var eventData = new DamageEvent
            {
                Damage = currentDamage,
                DamageType = (DamageType)(uint)damageInfo.damageType,
                DotType = (DotIndex)(int)damageInfo.dotIndex,
                IsCrit = damageInfo.crit,
                IsRejected = damageInfo.rejected
            };
            
            if (damageInfo.inflictor != null)
                eventData.Inflictor = damageInfo.inflictor.name;
            else if (damageInfo.procChainMask.mask > 0)
                eventData.Inflictor = damageInfo.procChainMask.ToString();
            
            if (attacker != null && attackerCharacterBody != null && attackerCharacterBody.isPlayerControlled)
            {
                var networkUser = Util.LookUpBodyNetworkUser(attacker);

                eventData.Enemy = new Monster(self.name, self.body.isElite, self.body.isBoss);
                
                eventMetadata = new EventMetaData(EventType.CharacterDamageDeal, runTime, currentRun.GetUniqueId())
                {
                    PlayerId = networkUser.id.steamId.steamValue,
                    Data = eventData
                };
            }
            else
            {
                var networkUser = Util.LookUpBodyNetworkUser(self.body);

                eventData.Enemy = new Monster(attackerCharacterBody.name, attackerCharacterBody.isElite,
                    attackerCharacterBody.isBoss);
                
                eventMetadata = new EventMetaData(EventType.CharacterDamageTake, runTime, currentRun.GetUniqueId())
                {
                    PlayerId = networkUser.id.steamId.steamValue,
                    Data = eventData
                };
            }

            OnEventProcessed(eventMetadata);
        }
        finally
        {
            orig(self, damageInfo);
        }
    }
}

