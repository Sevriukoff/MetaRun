using System;
using On.RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Mod.Base;
using CharacterBody = RoR2.CharacterBody;
using ProcChainMask = RoR2.ProcChainMask;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunMobSpawnedTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        On.RoR2.Run.OnServerCharacterBodySpawned += CharacterBodySpawned;
        HealthComponent.Heal += HealthComponentOnHeal;
    }
    
    public override void StopProcessing()
    {
        HealthComponent.Heal -= HealthComponentOnHeal;
    }
    
    private void CharacterBodySpawned(On.RoR2.Run.orig_OnServerCharacterBodySpawned orig, RoR2.Run self,
        CharacterBody characterBody)
    {
        orig(self, characterBody);
    }
    
    private float HealthComponentOnHeal(HealthComponent.orig_Heal orig, RoR2.HealthComponent self,
        float amount, ProcChainMask procChainMask, bool nonRegen)
    {
        if (Math.Abs(self.fullHealth - amount) < 0.001 && !nonRegen && !self.body.isPlayerControlled)
        {
            // Моб заспавнился в self вся про него инфа
        }

        return orig(self, amount, procChainMask, nonRegen);
    }
}