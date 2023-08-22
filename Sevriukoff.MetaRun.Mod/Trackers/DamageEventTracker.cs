using On.RoR2;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Mod.Base;
using CharacterBody = RoR2.CharacterBody;
using DamageInfo = RoR2.DamageInfo;

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
            var damageEventAttacker = new GameObject();
            var damageEventTarget = new GameObject();
            var damageEvent = new DamageEvent()
            {
                Damage = currentDamage,
                DamageType = (DamageType)(uint)damageInfo.damageType,
                DotType = (DotIndex)(int)damageInfo.dotIndex,
                IsCrit = damageInfo.crit,
                IsRejected = damageInfo.rejected,
                Attacker = damageEventAttacker,
                Target = damageEventTarget
            };

            if (damageInfo.inflictor != null)
            {
                damageEvent.Inflictor = damageInfo.inflictor.name;
            }
            else if (damageInfo.procChainMask.mask > 0)
            {
                damageEvent.Inflictor = damageInfo.procChainMask.ToString();
            }

            if (attacker != null && attackerCharacterBody != null && attackerCharacterBody.isPlayerControlled)
            {
                damageEventAttacker.PlayerId = attacker.GetComponent<UnityEngine.Networking.NetworkIdentity>()
                    .netId.Value;
                damageEventAttacker.Name = attacker.name;

                damageEventTarget.Name = self.name;
                damageEventTarget.IsElite = self.GetComponent<CharacterBody>().isElite;
                damageEventTarget.IsBoss = self.GetComponent<CharacterBody>().isBoss;
            }
            else
            {
                damageEventAttacker.Name = attacker.name;
                damageEventAttacker.IsElite = attackerCharacterBody.isElite;
                damageEventAttacker.IsBoss = attackerCharacterBody.isBoss;

                damageEventTarget.PlayerId = self.netId.Value;
                damageEventTarget.Name = self.name;
            }
            
            OnEventProcessed(damageEvent);
        }
        finally
        {
            orig(self, damageInfo);
        }
    }
}