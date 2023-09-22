using System;
using On.RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Domain.Events.Character;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;
using ProcChainMask = RoR2.ProcChainMask;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterHealedTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        HealthComponent.Heal += HealthComponentOnHeal;
    }

    public override void StopProcessing()
    {
        HealthComponent.Heal -= HealthComponentOnHeal;
    }
    
    private float HealthComponentOnHeal(HealthComponent.orig_Heal orig, RoR2.HealthComponent self,
        float amount, ProcChainMask procChainMask, bool nonRegen)
    {
        if (!self.body.isPlayerControlled)
            return orig(self, amount, procChainMask, nonRegen);
        
        var playerId = self.body.master.playerCharacterMasterController.networkUser.id.steamId.steamValue;

        CreateEventMetaData
        (
            EventType.CharacterHealed,
            new CharacterHealedEvent
            {
                HealAmount = amount,
                IsRegen = !nonRegen
            },
            playerId
        );

        return orig(self, amount, procChainMask, nonRegen);
    }
}